using MongoDB.Bson.Serialization.Attributes;

namespace ET.Server
{
    [ChildOf(typeof(BuffTempComponent))]
    public class BuffCreateInfo: Entity, IAwake<int>, IDestroy, ISerializeToEntity
    {
        // Buff的创建和迭代需要用到的资料都可以添加在这里
        public int ConfigId;
    }
    
    [ChildOf(typeof(BuffComponent))]
    public class Buff : Entity, IAwake<int>, IDestroy, ISerializeToEntity, IDeserialize
    {
        public int ConfigId;

        [BsonIgnore]
        public BuffConfig Config
        {
            get
            {
                return BuffConfigCategory.Instance.Get(this.ConfigId);
            }
        }

        [BsonIgnore]
        public Unit Owner;

        public long CreateTime;

        public int TickTime;

        public long TickBeginTime;

        [BsonIgnore]
        public long TickTimer;

        [BsonIgnore]
        public long WaitTickTimer;

        public long ExpireTime;

        [BsonIgnore]
        public long ExpireTimer;
    }
}