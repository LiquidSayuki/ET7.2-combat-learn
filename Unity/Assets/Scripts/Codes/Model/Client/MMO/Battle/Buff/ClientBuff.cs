namespace ET.Client
{
    [ChildOf(typeof(ClientBuffComponent))]
    public class ClientBuff : Entity, IAwake<int>, IDestroy
    {
        public int ConfigId;

        public BuffConfig Config
        {
            get
            {
                return BuffConfigCategory.Instance.Get(this.ConfigId);
            }
        }

        public Unit Owner;

        public long CreateTime;

        public long ExpireTime;
    }
}