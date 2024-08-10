namespace ET.Server
{
    public class ActionsAwakeSystem: AwakeSystem<Actions, int>
    {
        protected override void Awake(Actions self, int a)
        {
            self.ConfigId = a;
        }
    }

    public class ActionsDestroySystem: DestroySystem<Actions>
    {
        protected override void Destroy(Actions self)
        {
            self.ConfigId = default;
            self.Caster = default;
            self.Owner = default;
        }
    }
    
    public static class ActionsSystem
    {
        
    }
}