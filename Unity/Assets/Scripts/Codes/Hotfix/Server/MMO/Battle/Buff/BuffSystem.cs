using System;
using ET.EventType;

namespace ET.Server
{
    public class BuffCreateInfoAwakeSystem: AwakeSystem<BuffCreateInfo, int>
    {
        protected override void Awake(BuffCreateInfo self, int a)
        {
            self.ConfigId = a;
        }
    }
    
    public class BuffCreateInfoDestroySystem: DestroySystem<BuffCreateInfo>
    {
        protected override void Destroy(BuffCreateInfo self)
        {
            self.ConfigId = default;
        }
    }
    
    public class BuffAwakeSystem: AwakeSystem<Buff, int>
    {
        protected override void Awake(Buff self, int a)
        {
            self.ConfigId = a;
            self.AddComponent<ActionsTempComponent>();
            self.CreateTime = TimeHelper.ServerNow();

            if (self.Config.TotalTime == 0)
            {
                self.SetExpireTime(0);
            }
            else
            {
                long expireTime = self.CreateTime + self.Config.TotalTime;
                self.SetExpireTime(expireTime);
            }
            
            self.SetTickTime(self.Config.TickTime);
        }
    }
    
    public class BuffDeserializeSystem: DeserializeSystem<Buff>
    {
        protected override void Deserialize(Buff self)
        {
            self.AddComponent<ActionsTempComponent>();
            self.Owner = self.Parent.GetParent<Unit>();
        }
    }
    
    public class BuffDestroySystem: DestroySystem<Buff>
    {
        protected override void Destroy(Buff self)
        {
            self.ConfigId = default;
            self.Owner = default;
            self.CreateTime = default;
            self.TickTime = default;
            self.TickBeginTime = default;

            TimerComponent.Instance.Remove(ref self.TickTimer);
            TimerComponent.Instance.Remove(ref self.WaitTickTimer);

            self.ExpireTime = default;
            TimerComponent.Instance.Remove(ref self.ExpireTimer);
        }
    }

    [Invoke(TimerInvokeType.BuffExpireTimer)]
    public class BuffExpireTimer_TimerHandler: ATimer<Buff>
    {
        protected override void Run(Buff t)
        {
            t.TimeOut();
        }
    }

    [Invoke(TimerInvokeType.BuffTickTimer)]
    public class BuffTickTimer_TimerHandler: ATimer<Buff>
    {
        protected override void Run(Buff t)
        {
            t.TickActions();
        }
    }
    
    [FriendOf(typeof(Buff))]
    public static class BuffSystem
    {
        /// <summary>
        /// 设置Buff过期时间
        /// </summary>
        public static void SetExpireTime(this Buff self, long expireTime, bool noticeClient = false)
        {
            // 无持续的Buff
            if (expireTime == 0)
            {
                self.ExpireTime = 0;
                if (noticeClient)
                {
                    self.NoticeClientUpdateInfo();
                }
                return;
            }

            if (self.ExpireTime == expireTime)
            {
                return;
            }

            self.ExpireTime = expireTime;
            if (noticeClient)
            {
                self.NoticeClientUpdateInfo();
            }

            // 设置Buff过期的计时器
            if (self.ExpireTimer != 0)
            {
                TimerComponent.Instance.Remove(ref self.ExpireTimer);
            }
            self.ExpireTimer = TimerComponent.Instance.NewOnceTimer(self.ExpireTime, TimerInvokeType.BuffExpireTimer, self);
        }

        /// <summary>
        /// 设置Buff读秒时间
        /// </summary>
        public static void SetTickTime(this Buff self, int tickTime)
        {
            if (tickTime > 0)
            {
                self.TickBeginTime = TimeHelper.ServerNow();
                self.TickTime = tickTime;
                
                // 设置Buff读秒的计时器
                TimerComponent.Instance.Remove(ref self.TickTimer);
                self.TickTimer = TimerComponent.Instance.NewRepeatedTimer(tickTime, TimerInvokeType.BuffTickTimer, self);
            }
        }
        
        public static void NoticeClientUpdateInfo(this Buff self)
        {
            M2C_BuffUpdate m2CBuffUpdate = new M2C_BuffUpdate() { UnitId = self.Owner.Id, BuffData = self.ToBuffAddProto() };
            MMOMessageHelper.SendClient(self.Owner, m2CBuffUpdate, (NoticeClientType)self.Config.NoticeClientType);
        }
        
        /// <summary>
        /// Buff被添加时触发的Actions
        /// </summary>
        public static void AddActions(this Buff self)
        {
            long instanceId = self.InstanceId;

            foreach (int i in self.Config.AddAction)
            {
                try
                {
                    self.CreateActions(i, ActionsRunType.BuffAdd);

                    // Buff可能会在还没有运行完这个函数就被Dispose掉
                    // 但此时如果Buff进入对象池，又从对象池中取出，只判断IsDispose不能保证Buff实体没有变更过
                    // 会导致Buff效果发生异常
                    if (self.InstanceId != instanceId)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Buff AddActionError, OwnerId {self.Owner?.Id} buffId {self.Id} buffConfigId {self.Config?.Id} actions {i} \n {e}");
                }
            }
        }

        /// <summary>
        /// Buff被移除时触发的Actions
        /// </summary>
        public static void RemoveActions(this Buff self)
        {
            long instanceId = self.InstanceId;

            foreach (int i in self.Config.RemoveAction)
            {
                try
                {
                    self.CreateActions(i, ActionsRunType.BuffRemove);

                    // Buff可能会在还没有运行完这个函数就被Dispose掉
                    // 但此时如果Buff进入对象池，又从对象池中取出，只判断IsDispose不能保证Buff实体没有变更过
                    // 会导致Buff效果发生异常
                    if (self.InstanceId != instanceId)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Buff RemoveActionError, OwnerId {self.Owner?.Id} buffId {self.Id} buffConfigId {self.Config?.Id} actions {i} \n {e}");
                }
            }
        }

        /// <summary>
        /// Buff读秒时触发的Actions
        /// </summary>
        public static void TickActions(this Buff self)
        {
            if (self.IsDisposed)
            {
                return;   
            }

            long instanceId = self.InstanceId;

            foreach (int i in self.Config.TickAction)
            {
                try
                {
                    self.CreateActions(i, ActionsRunType.BuffTick);
                    
                    if (self.InstanceId != instanceId)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    if (self.InstanceId != instanceId)
                    {
                        Log.Error($"Buff TickActionError, OwnerId {self.Owner?.Id} buffId {self.Id} buffConfigId {self.Config?.Id} actions {i} \n {e}");
                    }
                    else
                    {
                        Log.Error($"Buff TickActionError, OwnerId {self.Owner?.Id} buffId {self.Id} actions {i} \n {e}");
                    }
                }
            }
            
            if (self.InstanceId != instanceId)
            {
                return;
            }

            // 向客户端下发Buff读秒的消息
            if (self.Config.TickAction.Length > 0)
            {
                if (self.Owner == null)
                {
                    return;
                }

                M2C_BuffTick m2CBuffTick = new M2C_BuffTick() { BuffId = self.Id, UnitId = self.Owner.Id };
                MMOMessageHelper.SendClient(self.Owner, m2CBuffTick, (NoticeClientType)self.Config.NoticeClientType);
            }
        }

        /// <summary>
        /// Buff到期的事件
        /// </summary>
        public static void TimeOut(this Buff self)
        {
            EventSystem.Instance.Publish(self.DomainScene(), new BuffTimeOut(){Unit = self.Owner, BuffId = self.Id});
        }
        
        public static BuffProto ToBuffAddProto(this Buff self)
        {
            BuffProto buffProto = new BuffProto()
            {
                Id = self.Id, ConfigId = self.ConfigId, CreateTime = self.CreateTime, ExpireTime = self.ExpireTime,
            };
            
            // 如果有额外的属性，可以放在这里传输
            // buffProto.ExtraData

            return buffProto;
        }
    }
}