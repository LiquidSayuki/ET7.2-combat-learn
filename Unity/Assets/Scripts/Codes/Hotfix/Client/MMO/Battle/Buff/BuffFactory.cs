namespace ET.Client
{
    [FriendOf(typeof(ClientBuff))]
    public static class BuffFactory
    {
        public static ClientBuff Create(Unit owner, BuffProto buffData)
        {
            ClientBuff clientBuff = owner.GetComponent<ClientBuffComponent>().AddChildWithId<ClientBuff, int>(buffData.Id, buffData.ConfigId);

            clientBuff.CreateTime = buffData.CreateTime;
            clientBuff.ExpireTime = buffData.ExpireTime;
            clientBuff.Owner = owner;

            return clientBuff;
        }
    }
}