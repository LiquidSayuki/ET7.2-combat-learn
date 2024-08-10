namespace ET.Client
{
    public class ClientCastAwakeSystem: AwakeSystem<ClientCast, int>
    {
        protected override void Awake(ClientCast self, int a)
        {
            self.ConfigId = a;
        }
    }
    
    public class ClientCastDestroySystem: DestroySystem<ClientCast>
    {
        protected override void Destroy(ClientCast self)
        {
            self.ConfigId = default;
            self.CasterId = default;
            self.TargetsId.Clear();
        }
    }

    public static class ClientCastSystem
    {
        
    }
}