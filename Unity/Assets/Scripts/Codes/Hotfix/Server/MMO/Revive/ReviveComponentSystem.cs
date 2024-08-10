namespace ET.Server
{
    public class ReviveComponentAwakeSystem : AwakeSystem<ReviveComponent>
    {
        protected override void Awake(ReviveComponent self)
        {
            self.Alive = true;
        }
    }
    
    public class ReviveComponentDestroySystem : DestroySystem<ReviveComponent>
    {
        protected override void Destroy(ReviveComponent self)
        {
            self.Alive = default;
        }
    }
    
    public static class ReviveComponentSystem
    {
        
    }
}