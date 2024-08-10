using System.Collections.Generic;

namespace ET.Server
{
    public class SkillStatusComponentDestroySystem: DestroySystem<SkillStatusComponent>
    {
        protected override void Destroy(SkillStatusComponent self)
        {
            self.CurSkillCastInstanceId = default;
            self.CurSkillCastId = default;
            self.CurSkillStartTime = default;
            self.CurSkillStatusType = SkillStatusType.New;
        }
    }
    [FriendOf(typeof(SkillStatusComponent))]
    [FriendOf(typeof(Cast))]
    public static class SkillStatusComponentSystem
    {
        /// <summary>
        /// 检查技能释放状态
        /// </summary>
        public static int CanCastSkill(this SkillStatusComponent self, int castConfigId)
        {
            Unit unit = self.GetParent<Unit>();
            if (unit == null)
            {
                return ErrorCode.ERR_Cast_UnitIsNull;
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            if (numericComponent == null)
            {
                return ErrorCode.ERR_Cast_NumIsNull;
            }

            // 不可释放技能
            if (numericComponent[NumericType.ForbidSkill] > 0)
            {
                return ErrorCode.ERR_Cast_ForbidSkill;
            }

            // 冷却状态
            if (self.CoolDowns.TryGetValue(castConfigId, out long tarTime))
            {
                if (TimeHelper.ServerNow() <= tarTime)
                {
                    return ErrorCode.ERR_Cast_SkillCoolDown;
                }
            }

            return ErrorCode.ERR_Success;
        }
        
        /// <summary>
        /// 技能开始释放
        /// </summary>
        public static bool StartSkill(this SkillStatusComponent self, Cast cast)
        {
            if (self.CanCastSkill(cast.ConfigId) != ErrorCode.ERR_Success)
            {
                return false;
            }

            int castConfigId = cast.ConfigId;

            if (cast.Config.StatusSkill == 0)
            {
                return true;
            }

            long now = TimeHelper.ServerNow();

            self.CurSkillCastId = castConfigId;
            self.CurSkillCastInstanceId = cast.InstanceId;
            self.CurSkillStartTime = now;
            self.CurSkillStatusType = SkillStatusType.Init;

            int coolDown = CastConfigCategory.Instance.Get(castConfigId).CoolDown;

            if (coolDown > 0)
            {
                self.CoolDowns[castConfigId] = now + coolDown;

                Unit unit = self.GetParent<Unit>();

                M2C_CoolDownChange m2CCoolDownChange = new M2C_CoolDownChange()
                {
                    CastConfigIds = new List<int>(), CoolDownTimes = new List<long>(), CoolDownStartTimes = new List<long>(),
                };
                
                m2CCoolDownChange.CastConfigIds.Add(castConfigId);
                m2CCoolDownChange.CoolDownTimes.Add(self.CoolDowns[castConfigId]);
                m2CCoolDownChange.CoolDownStartTimes.Add(now);
                
                MMOMessageHelper.SendClient(unit, m2CCoolDownChange, NoticeClientType.Self);
            }

            return true;
        }

        /// <summary>
        /// 技能从起手转入执行
        /// </summary>
        public static bool RunningSkill(this SkillStatusComponent self, Cast cast)
        {
            // 非状态类技能直接通过
            if (cast.Config.StatusSkill == 0)
            {
                return true;
            }

            // 当前技能需要在起手阶段才能转入运行阶段
            if (self.CurSkillStatusType != SkillStatusType.Init || self.CurSkillCastInstanceId != cast.InstanceId)
            {
                return false;
            }

            self.CurSkillStatusType = SkillStatusType.Running;

            return true;
        }

        /// <summary>
        /// 技能从执行状态转入结束
        /// </summary>
        public static bool FinishSkill(this SkillStatusComponent self, Cast cast)
        {
            // 非状态类技能直接通过
            if (cast.Config.StatusSkill == 0)
            {
                return true;
            }
            
            if (self.CurSkillStatusType != SkillStatusType.Running || self.CurSkillCastInstanceId != cast.InstanceId)
            {
                return false;
            }
            
            self.CurSkillStatusType = SkillStatusType.Finish;

            return true;
        }

        /// <summary>
        /// 技能的打断
        /// </summary>
        public static bool BreakSkill(this SkillStatusComponent self)
        {
            //todo: 判断一些不会被打断技能的情况，例如霸体，移动施法
            
            self.ClearCurSkillInfo();
            return true;
        }

        /// <summary>
        /// 重置当前技能状态
        /// </summary>
        public static void ClearCurSkillInfo(this SkillStatusComponent self)
        {
            self.CurSkillCastInstanceId = default;
            self.CurSkillCastId = default;
            self.CurSkillStartTime = default;
            self.CurSkillStatusType = SkillStatusType.New;
        }
    }
}