namespace ET.Client
{
    public class ClientBuffAwakeSystem : AwakeSystem<ClientBuff, int>
    {
        protected override void Awake(ClientBuff self, int a)
        {
            self.ConfigId = a;
        }
    }
    
    public class ClientBuffDestroySystem : DestroySystem<ClientBuff>
    {
        protected override void Destroy(ClientBuff self)
        {
            self.ConfigId = default;
            self.Owner = default;
            self.CreateTime = default;
            self.ExpireTime = default;
        }
    }

    public static class ClientBuffSystem
    {
        
    }
}