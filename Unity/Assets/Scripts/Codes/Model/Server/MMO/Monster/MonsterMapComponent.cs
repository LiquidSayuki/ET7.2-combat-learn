namespace ET.Server
{
    [ChildOf(typeof(MonsterMapComponent))]
    public class CreateMonsterInfo: Entity, IAwake<int>
    {
        public int monsterId;
    }
    
    [ComponentOf(typeof(Scene))]
    public class MonsterMapComponent : Entity, IAwake
    {
        
    }
}