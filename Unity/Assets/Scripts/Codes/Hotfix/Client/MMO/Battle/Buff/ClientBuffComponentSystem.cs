using System;

namespace ET.Client
{
    public class ClientBuffComponentDestroySystem : DestroySystem<ClientBuffComponent>
    {
        protected override void Destroy(ClientBuffComponent self)
        {
            foreach (var value in self.Buffs.Values)
            {
                value?.Dispose();
            }
            
            self.Buffs.Clear();
        }
    }
    
    [FriendOf(typeof(ClientBuffComponent))]
    [FriendOf(typeof(ClientBuff))]
    public static class ClientBuffComponentSystem
    {
        public static void Add(this ClientBuffComponent self, ClientBuff clientBuff)
        {
            if (self.Buffs.ContainsKey(clientBuff.Id))
            {
                return;
            }

            self.Buffs.Add(clientBuff.Id, clientBuff);
            clientBuff.Owner = self.GetParent<Unit>();
        }

        public static ClientBuff Get(this ClientBuffComponent self, long buffId)
        {
            if (self.Buffs.TryGetValue(buffId, out ClientBuff buff))
            {
                return buff;
            }

            return null;
        }

        public static void Remove(this ClientBuffComponent self, long buffId)
        {
            ClientBuff clientBuff = self.Get(buffId);

            if (clientBuff == null)
            {
                return;
            }

            self.Buffs.Remove(buffId);
            clientBuff?.Dispose();
        }

        public static void Update(this ClientBuffComponent self, BuffProto buffData)
        {
            ClientBuff clientBuff = self.Get(buffData.Id);
            if (clientBuff == null)
            {
                return;
            }

            clientBuff.CreateTime = buffData.CreateTime;
            clientBuff.ExpireTime = buffData.ExpireTime;
        }
    }
}