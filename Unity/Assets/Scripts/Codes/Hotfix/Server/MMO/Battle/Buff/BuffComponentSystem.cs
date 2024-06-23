using System;

namespace ET.Server
{
    public class BuffComponentAwakeSystem: AwakeSystem<BuffComponent>
    {
        protected override void Awake(BuffComponent self)
        {
            self.AddComponent<BuffTempComponent>();
        }
    }
    
    public class BuffComponentDestroySystem: DestroySystem<BuffComponent>
    {
        protected override void Destroy(BuffComponent self)
        {
            self.ConfigIdBuffs.Clear();
        }
    }
    
    [FriendOf(typeof(Buff))]
    public class BuffComponentDeserializeSystem : DeserializeSystem<BuffComponent>
    {
        protected override void Deserialize(BuffComponent self)
        {
            self.AddComponent<BuffTempComponent>();

            foreach (Buff buff in self.Children.Values)
            {
                self.ConfigIdBuffs.Add(buff.ConfigId, buff);
            }
        }
    }
    [FriendOf(typeof(BuffCreateInfo))]
    [FriendOf(typeof(Buff))]
    [FriendOf(typeof(BuffComponent))]
    public static class BuffComponentSystem
    {
        public static BuffCreateInfo Create(this BuffComponent self, int configId)
        {
            return self.GetComponent<BuffTempComponent>().AddChild<BuffCreateInfo, int>(configId);
        }

        public static bool Add(this BuffComponent self, BuffCreateInfo buffCreateInfo)
        {
            if (buffCreateInfo == null || buffCreateInfo.IsDisposed)
            {
                return false;
            }

            if (self == null || self.IsDisposed)
            {
                return false;
            }

            // 创建Buff
            Buff buff = self.AddChild<Buff, int>(buffCreateInfo.ConfigId);

            // 设置Buff拥有者
            buff.Owner = self.GetParent<Unit>();
            if (buff.Owner == null)
            {
                buff.Dispose();
                return false;
            }

            int configId = buff.ConfigId;
            
            // 如果已有同类Buff的处理方式
            if (self.ConfigIdBuffs.ContainsKey(configId))
            {
                // todo: 根据buff的配置，决定不同buff的同类buff处理方式
                
                // 新Buff顶掉旧Buff
                self.Remove(self.ConfigIdBuffs[configId].Id);
            }

            self.ConfigIdBuffs.Add(configId, buff);
            
            // 向客户端发送Buff添加的事件
            if ((NoticeClientType)buff.Config.NoticeClientType != NoticeClientType.NoNotice)
            {
                M2C_BuffAdd buffAdd = new M2C_BuffAdd() { UnitId = buff.Owner.Id, BuffData = buff.ToBuffAddProto() };
                MMOMessageHelper.SendClient(buff.Owner, buffAdd, (NoticeClientType)buff.Config.NoticeClientType);
            }

            buff.AddActions();
            
            return true;
        }

        public static bool CreateAndAdd(this BuffComponent self, int configId)
        {
            using (BuffCreateInfo buffCreateInfo = self.Create(configId))
            {
                return self.Add(buffCreateInfo);
            }
        }

        /// <summary>
        /// Buff移除
        /// </summary>
        public static void Remove(this BuffComponent self, long buffId)
        {
            if (!self.Children.TryGetValue(buffId, out Entity entity))
            {
                return;
            }
            
            Buff buff = entity as Buff;

            try
            {
                self.ConfigIdBuffs.Remove(buff.ConfigId);

                // 向客户端发送Buff移除的网络消息
                if ((NoticeClientType)buff.Config.NoticeClientType != NoticeClientType.NoNotice)
                {
                    M2C_BuffRemove buffRemove = new M2C_BuffRemove() { BuffId = buff.Id, UnitId = buff.Owner.Id };
                    MMOMessageHelper.SendClient(buff.Owner, buffRemove, (NoticeClientType)buff.Config.NoticeClientType);
                }
                
                buff.RemoveActions();
                
                buff.Dispose();
            }
            catch (Exception e)
            {
                Log.Error($"Buff remove error! Buff componentId {self.Id} | Buff Id {buff.Id} | Buff configId {buff.Config?.Id} | {e}");
            }
        }
    }
}