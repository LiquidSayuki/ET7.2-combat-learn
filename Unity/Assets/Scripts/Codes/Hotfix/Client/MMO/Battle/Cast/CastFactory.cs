namespace ET.Client
{
    [FriendOf(typeof(ClientCast))]
    public static class CastFactory
    {
        public static ClientCast Create(Unit caster, long id, int configId)
        {
            ClientCast clientCast = caster.GetComponent<ClientCastComponent>().AddChildWithId<ClientCast, int>(id, configId);
            clientCast.CasterId = caster.Id;

            return clientCast;
        }
    }
}