using MongoDB.Bson.Serialization.Attributes;

namespace ET.Server
{
    [ChildOf(typeof(ActionsTempComponent))]
    public class Actions : Entity, IAwake<int>, IDestroy, ISerializeToEntity
    {
        public int ConfigId;

        [BsonIgnore]
        public ActionsConfig Config
        {
            get
            {
                return ActionsConfigCategory.Instance.Get(this.ConfigId);
            }
        }

        [BsonIgnore]
        public Unit Caster;

        [BsonIgnore]
        public Unit Owner;

        [BsonIgnore]
        public Cast CastSelf
        {
            get
            {
                return this.parent.GetParent<Cast>();
            }
        }

        [BsonIgnore]
        public Buff BuffSelf
        {
            get
            {
                return this.parent.GetParent<Buff>();
            }
        }

        [BsonIgnore]
        public BulletComponent BulletSelf
        {
            get
            {
                return this.parent.GetParent<BulletComponent>();
            }
        }
    }
}