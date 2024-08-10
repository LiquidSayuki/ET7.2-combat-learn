namespace ET.Server
{
    [ComponentOf(typeof(Unit))]
    public class ReviveComponent : Entity, IAwake, IDestroy
    {
        public bool Alive = true;
    }
}