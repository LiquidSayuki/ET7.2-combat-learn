using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    public class CastAwakeSystem: AwakeSystem<Cast, int>
    {
        protected override void Awake(Cast self, int a)
        {
            self.ConfigId = a;
            self.AddComponent<ActionsTempComponent>();
        }
    }

    public class CastDestroySystem: DestroySystem<Cast>
    {
        protected override void Destroy(Cast self)
        {
            self.ConfigId = default;
            self.Caster = default;
            self.Target.Clear();
            self.StartTime = default;
        }
    }
    [FriendOf(typeof(Cast))]
    public static class CastSystem
    {
        /// <summary>
        /// 释放技能
        /// </summary>
        public static int Cast(this Cast self)
        {
            // 判断是否可以施法
            int err = self.CastCheck();
            if (err != ErrorCode.ERR_Success)
            {
                return err;
            }

            // 选择目标
            self.SelectTarget();

            // 二次确认
            err = self.CastCheckBeforeBegin();
            if (err != ErrorCode.ERR_Success)
            {
                return err;
            }

            // 执行技能
            self.CastBeginAsync().Coroutine();

            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 判断是否可以释放技能
        /// </summary>
        public static int CastCheck(this Cast self)
        {
            // 无自身
            if (self == null || self.IsDisposed)
            {
                return ErrorCode.ERR_Cast_ArgsError;
            }

            // 无释放者
            if (self.Caster == null || self.Caster.IsDisposed)
            {
                return ErrorCode.ERR_Cast_CasterIsNull;
            }

            return ErrorCode.ERR_Success;
        }
        
        /// <summary>
        /// 选择目标
        /// </summary>
        public static void SelectTarget(this Cast self)
        {
            Unit caster = self.Caster;
            CastConfig castConfig = self.Config;

            int range = 0;
            switch (castConfig.SelectType)
            {
                case 1: // 选择身边一定范围内的一个人
                    range = int.Parse(castConfig.SelectParam[0]);
                    foreach (AOIEntity aoiEntity in caster.GetBeSeePlayers().Values)
                    {
                        Unit unit = aoiEntity.GetParent<Unit>();

                        if (unit == caster)
                        {
                            continue;
                        }

                        if (math.length(unit.Position - caster.Position) < range)
                        {
                            self.Target.Add(unit.Id);
                            break;
                        }
                    }
                    break;
                case 2:
                    range = int.Parse(castConfig.SelectParam[0]);
                    foreach (AOIEntity aoiEntity in caster.GetBeSeePlayers().Values)
                    {
                        Unit unit = aoiEntity.GetParent<Unit>();

                        if (unit == caster)
                        {
                            continue;
                        }

                        if (math.length(unit.Position - caster.Position) < range)
                        {
                            self.Target.Add(unit.Id);
                        }
                    }                    
                    break;
            }
        }

        public static int CastCheckBeforeBegin(this Cast self)
        {
            switch (self.Config.SelectType)
            {
                case 1:
                case 2:
                    if (self.Target.Count <= 0)
                    {
                        return ErrorCode.ERR_Cast_TargetIsNull;
                    }
                    break;
            }
            
            return ErrorCode.ERR_Success;
        }

        public static async ETTask CastBeginAsync(this Cast self)
        {
            self.StartTime = TimeHelper.ServerNow();

            // 建立技能开始释放的消息
            M2C_CastStart m2CCastStart = new M2C_CastStart() { CastId = self.Id, CasterId = self.Caster.Id, TargetId = new List<long>() };
            m2CCastStart.TargetId.AddRange(self.Target);
            
            // 消息下发与同步
            MMOMessageHelper.SendClient(self.Caster, m2CCastStart, (NoticeClientType)self.Config.NoticeClientType);
            
            // 技能行为逻辑
            CastConfig config = self.Config;
            if (config.Times.Count < 0)
            {
                return;
            }

            long castInstanceId = 0;
            long casterInstanceId = 0;

            // Config初始化时，根据配置表将需要发生技能效果的时间建立一个有序的List
            // 遍历这个List可以在每一个配置的需要发生技能效果的时间触发对应的技能效果
            foreach (int time in config.Times)
            {
                casterInstanceId = self.InstanceId;
                casterInstanceId = self.Caster.InstanceId;
                
                // 等待直到下一个要产生技能效果的时间
                await TimerComponent.Instance.WaitTillAsync(self.StartTime + time);

                // 进行技能和施法者的检测
                if (!self.CheckAsyncInvalid(castInstanceId, casterInstanceId))
                {
                    Log.Error($"Cast Async Invalid {castInstanceId} {casterInstanceId}");
                    return;
                }
                
                // 技能行为的发生
                // Config初始化时，根据配置表 以时间节点time为key 对应时间节点要发生的所有技能效果为value 创建了MultiMap
                // 获取这个Map中的一个List，进行遍历，可以得到所有要在此刻生效的技能效果
                foreach (CastActionTimes castActionTimes in config.TimesDict[time])
                {
                    // 技能效果是命中自身还是命中他人
                    if (castActionTimes.IsSelfHit)
                    {
                        self.HandleSelfHit(castActionTimes.Index);
                    }
                    else
                    {
                        self.HandleTargetHit(castActionTimes.Index);
                    }
                }
            }

            // 非瞬发技能
            if (config.TotalTime > 0)
            {
                castInstanceId = self.InstanceId;
                casterInstanceId = self.Caster.InstanceId;

                await TimerComponent.Instance.WaitTillAsync(self.StartTime + config.TotalTime);

                if (!self.CheckAsyncInvalid(castInstanceId,casterInstanceId))
                {
                    Log.Error($"Cast Async Invalid {castInstanceId} {casterInstanceId}");
                    return;
                }
            }
            
            self.CastFinish();
        }

        /// <summary>
        /// 检测技能和技能释放者是否变更了状态
        /// </summary>
        public static bool CheckAsyncInvalid(this Cast self, long castInstanceId, long casterInstanceId)
        {
            if (self == null)
            {
                return false;
            }

            if (self.InstanceId != castInstanceId || self.Caster.InstanceId != casterInstanceId)
            {
                return false;
            }

            return true;
        }

        public static void CastFinish(this Cast self)
        {
            // 只有持续时长大于0的技能需要同步状态，顺发技能无需同步结束状态
            if (self.Config.TotalTime > 0)
            {
                M2C_CastFinish m2CCastFinish = new M2C_CastFinish() { CastId = self.InstanceId, CasterId = self.Caster.InstanceId };
                MMOMessageHelper.SendClient(self.Caster, m2CCastFinish, (NoticeClientType)self.Config.NoticeClientType );
            }
            
            self?.Dispose();
        }

        /// <summary>
        /// 技能命中自身的行为
        /// </summary>
        public static void HandleSelfHit(this Cast self, int index)
        {
            CastConfig config = self.Config;
            
            self.SelectTarget();
            if (self.Target.Count <= 0)
            {
                return;
            }
            
            // 通知客户端技能命中事件
            M2C_CastHit m2CCastHit = new M2C_CastHit() { CastId = self.Id, CasterId = self.Caster.Id, TargetId = new List<long>() };
            m2CCastHit.TargetId.AddRange(self.Target);
            MMOMessageHelper.SendClient(self.Caster, m2CCastHit, (NoticeClientType)config.NoticeClientType);
            
            // 通过index去取配置表中的命中Action的配置
            if (config.SelfHitAction.Length > index)
            {
                int actionId = config.SelfHitAction[index];
                if (actionId != 0)
                {
                    // Action的目标是Caster自身
                    self.CreateActions(actionId, self.Caster, ActionsRunType.CastHit);
                }
            }

            if (config.SelfBuffs.Length > index)
            {
                int buffId = config.SelfBuffs[index];
                if (buffId != 0)
                {
                    // Buff的目标是Caster自身
                    self.Caster.GetComponent<BuffComponent>()?.CreateAndAdd(buffId);
                }
            }
        }
        
        /// <summary>
        /// 技能命中他人的行为
        /// </summary>
        public static void HandleTargetHit(this Cast self, int index)
        {
            CastConfig config = self.Config;
            
            self.SelectTarget();
            if (self.Target.Count <= 0)
            {
                return;
            }

            // 通知客户端技能命中事件
            M2C_CastHit m2CCastHit = new M2C_CastHit() { CastId = self.Id, CasterId = self.Caster.Id, TargetId = new List<long>() };
            m2CCastHit.TargetId.AddRange(self.Target);
            MMOMessageHelper.SendClient(self.Caster, m2CCastHit, (NoticeClientType)config.NoticeClientType);

            UnitComponent unitComponent = self.DomainScene().GetComponent<UnitComponent>();

            foreach (var unitId in self.Target)
            {
                // 获取Cast命中的目标
                Unit unit = unitComponent.Get(unitId);

                if (unit == null || unit.IsDisposed)
                {
                    continue;
                }

                // 通过index去取配置表中的命中Action的配置
                if (config.HitAction.Length > index)
                {
                    int actionId = config.HitAction[index];
                    if (actionId != 0)
                    {
                        // 创建Action的时候Owner应该是 Cast命中的目标
                        self.CreateActions(actionId, unit, ActionsRunType.CastHit);
                    }
                }

                // 通过index去寻找产生的Buff效果
                if (config.Buffs.Length > index)
                {
                    int buffId = config.Buffs[index];
                    if (buffId != 0)
                    {
                        // 创建Buff的时候Owner应该是 Cast命中的目标
                        unit.GetComponent<BuffComponent>()?.CreateAndAdd(buffId);
                    }
                }
            }
        }
    }
}

