using System;
using Unity.Mathematics;

namespace ET.Server
{
    public class BulletComponentAwakeSystem : AwakeSystem<BulletComponent, int>
    {
        protected override void Awake(BulletComponent self, int a)
        {
            self.ConfigId = a;
            self.AddComponent<ActionsTempComponent>();
        }
    }
    
    public class BulletComponentDestroySystem : DestroySystem<BulletComponent>
    {
        protected override void Destroy(BulletComponent self)
        {
            self.ConfigId = default;
            self.TickCount = default;
            self.OwnerId = default;

            TimerComponent.Instance.Remove(ref self.TickTimer);
            TimerComponent.Instance.Remove(ref self.TickTimer2);
            TimerComponent.Instance.Remove(ref self.TickTimer3);
            TimerComponent.Instance.Remove(ref self.TotalTimer);
        }
    }

    #region 计时器Timer
    // 自己设计的计时器 Timer
    // 写一个这样的类就可以实现每次计时结束就来触发一次下面的事件
    [Invoke(TimerInvokeType.BulletTick)]
    public class BulletTickTimerHandler: ATimer<BulletComponent>
    {
        protected override void Run(BulletComponent t)
        {
            t.Tick();
        }
    }

    [Invoke(TimerInvokeType.BulletTick2)]
    public class BulletTickTimerHandler2: ATimer<BulletComponent>
    {
        protected override void Run(BulletComponent t)
        {
            t.Tick2();
        }
    }

    [Invoke(TimerInvokeType.BulletTick3)]
    public class BulletTickTimerHandler3: ATimer<BulletComponent>
    {
        protected override void Run(BulletComponent t)
        {
            t.Tick3();
        }
    }

    [Invoke(TimerInvokeType.BulletTotalTime)]
    public class BulletTickOverTimerHandler: ATimer<BulletComponent>
    {
        protected override void Run(BulletComponent t)
        {
            t.TimeOver();
        }
    }
    #endregion

    [FriendOf(typeof(BulletComponent))]
    [FriendOf(typeof(Cast))]
    public static class BulletComponentSystem
    {
        public static void Start(this BulletComponent self)
        {
            Unit owner = self.GetOwner();

            if (owner == null)
            {
                self.Dispose();
                return;
            }

            Log.Debug($"[Server] 子弹 -> {self.ConfigId} Start");

            BulletConfig config = self.Config;

            if (config.AwakeAction.Length != 0)
            {
                foreach (int actionId in config.AwakeAction)
                {
                    self.CreateActions(actionId, owner, owner, ActionsRunType.BulletAwake);
                }
            }

            // 每隔Interval时间触发的效果
            if (config.Interval > 0)
            {
                int interval = config.Interval;
                if (interval < 100)
                {
                    interval = 100;
                }

                self.TickTimer = TimerComponent.Instance.NewRepeatedTimer(interval, TimerInvokeType.BulletTick, self);
            }

            // 每隔0.1秒触发的效果
            if (config.Tick1.Length > 0)
            {
                self.TickTimer2 = TimerComponent.Instance.NewRepeatedTimer(100, TimerInvokeType.BulletTick2, self);
            }

            // 每隔一秒触发的效果
            if (config.Tick2.Length > 0)
            {
                self.TickTimer3 = TimerComponent.Instance.NewRepeatedTimer(1000, TimerInvokeType.BulletTick3, self);
            }

            // 子弹结束触发的效果
            if (config.TotalTime > 0)
            {
                self.TotalTimer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + config.TotalTime, TimerInvokeType.BulletTotalTime, self);
            }
        }

        /// <summary>
        /// Interval Tick 触发
        /// </summary>
        public static void Tick(this BulletComponent self)
        {
            Unit selfUnit = self.GetParent<Unit>();
            Unit owner = self.GetOwner();
            if (owner == null || owner.IsDisposed)
            {
                self.DoDispose();
                return;
            }

            BulletConfig bulletConfig = self.Config;
            self.SelectTarget();

            if (self.Target.Count <= 0)
            {
                return;
            }

            self.TickCount++;

            // 释放配置的Tick Cast
            if (bulletConfig.TickCastId.Length > 0)
            {
                foreach (var tickCastId in bulletConfig.TickCastId)
                {
                    Cast cast = owner.CreateCast(tickCastId);
                    cast.Target.AddRange(self.Target);
                    int err = cast.Cast();
                    if (err != ErrorCode.ERR_Success)
                    {
                        Log.Console($"[Server] 子弹 {self.ConfigId} 释放Cast {tickCastId} 失败: {err}");
                    }
                }
            }

            // 释放配置的Tick Action
            if (bulletConfig.TickAction.Length > 0)
            {
                foreach (var tickActionId in bulletConfig.TickAction)
                {
                    self.CreateActions(tickActionId, selfUnit, selfUnit, ActionsRunType.BulletTick);
                }
            }

            if (bulletConfig.TickLimit > 0 && self.TickCount >= bulletConfig.TickLimit)
            {
                // 结算次数到达上限，提前结束
                self.TimeOver();
            }
        }

        /// <summary>
        /// 0.1秒触发
        /// </summary>
        public static void Tick2(this BulletComponent self)
        {
            BulletConfig bulletConfig = self.Config;
            
            self.SelectTarget();

            foreach (var actionsId in bulletConfig.Tick1)
            {
                self.CreateActions(actionsId, self.GetParent<Unit>(), self.GetParent<Unit>(), ActionsRunType.BulletTick);
            }
        }

        /// <summary>
        /// 1秒触发
        /// </summary>
        public static void Tick3(this BulletComponent self)
        {
            BulletConfig bulletConfig = self.Config;
            
            self.SelectTarget();

            foreach (var actionsId in bulletConfig.Tick2)
            {
                self.CreateActions(actionsId, self.GetParent<Unit>(), self.GetParent<Unit>(), ActionsRunType.BulletTick);
            }
        }

        /// <summary>
        /// 子弹结束触发
        /// </summary>
        public static void TimeOver(this BulletComponent self)
        {
            TimerComponent.Instance.Remove(ref self.TickTimer);

            Unit owner = self.GetOwner();
            if (owner == null || owner.IsDisposed)
            {
                self.DoDispose();
                return;
            }

            BulletConfig bulletConfig = self.Config;
            if (bulletConfig.DestroyAction.Length > 0)
            {
                foreach (var destroyActionId in bulletConfig.DestroyAction)
                {
                    self.CreateActions(destroyActionId, self.GetParent<Unit>(), self.GetParent<Unit>(), ActionsRunType.BulletDestroy);
                }
            }
            
            self.DoDispose();
        }
        
        public static void SelectTarget(this BulletComponent self)
        {
            self.Target.Clear();
            Unit selfUnit = self.GetParent<Unit>();
            Unit owner = self.GetOwner();
            BulletConfig bulletConfig = self.Config;

            switch (bulletConfig.Shape)
            {
                // 范围内所有的目标
                case 1:
                    {
                        int range = int.Parse(bulletConfig.ShapeParam[0]);

                        foreach (var aoiEntity in selfUnit.GetBeSeeUnits().Values)
                        {
                            Unit unit = aoiEntity.GetParent<Unit>();
                            if (unit.Type != UnitType.Player && unit.Type != UnitType.Monster)
                            {
                                continue;
                            }

                            if (unit == owner)
                            {
                                continue;
                            }

                            if (math.length(unit.Position - selfUnit.Position) < range)
                            {
                                self.Target.Add(unit.Id);
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception($"no such BulletConfig Shape: {bulletConfig.Shape}");
            }
        }

        /// <summary>
        /// 销毁子弹自身
        /// </summary>
        public static void DoDispose(this BulletComponent self)
        {
            self.GetParent<Unit>().Stop(0); // 停止移动
            self.DomainScene().GetComponent<UnitComponent>().Remove(self.Parent.Id); // Unit组件销毁子弹Unit
        }

        /// <summary>
        /// 获取子弹的发射者
        /// </summary>
        public static Unit GetOwner(this BulletComponent self)
        {
            return self.DomainScene().GetComponent<UnitComponent>().Get(self.OwnerId);
        }

        public static void PreDestroy(this BulletComponent self)
        {
            Unit owner = self.GetOwner();

            TimerComponent.Instance.Remove(ref self.TickTimer);

            if (owner == null)
            {
                return;
            }

            BulletConfig config = self.Config;

            if (config.DestroyAction.Length == 0)
            {
                return;
            }

            foreach (int actionId in config.DestroyAction)
            {
                self.CreateActions(actionId, owner, owner, ActionsRunType.BulletDestroy);
            }
        }
    }
}