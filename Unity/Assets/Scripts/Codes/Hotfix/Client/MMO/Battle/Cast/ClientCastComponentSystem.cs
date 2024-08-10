using System;

namespace ET.Client
{
    public class ClientCastComponentDestroySystem : DestroySystem<ClientCastComponent>
    {
        protected override void Destroy(ClientCastComponent self)
        {
            foreach (var value in self.Casts.Values)
            {
                value?.Dispose();
            }
            self.Casts.Clear();
        }
    }
    [FriendOf(typeof(ClientCastComponent))]
    public static class ClientCastComponentSystem
    {
        public static void Add(this ClientCastComponent self, ClientCast clientCast)
        {
            if (self.Casts.ContainsKey(clientCast.Id))
            {
                return;
            }
            
            self.Casts.Add(clientCast.Id, clientCast);
        }

        public static ClientCast Get(this ClientCastComponent self, long Id)
        {
            if (self.Casts.TryGetValue(Id, out ClientCast cast))
            {
                return cast;
            }

            return null;
        }

        public static void Remove(this ClientCastComponent self, long Id)
        {
            ClientCast clientCast = self.Get(Id);

            if (clientCast != null)
            {
                self.Casts.Remove(Id);
                clientCast?.Dispose();
            }
        }
    }
}