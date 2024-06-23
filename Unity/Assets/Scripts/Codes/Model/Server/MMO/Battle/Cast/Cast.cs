using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET.Server
{
    [ChildOf(typeof(CastComponent))]
    public class Cast : Entity, IAwake<int>, IDestroy
    {
        public int ConfigId;

        [BsonIgnore]
        public CastConfig Config
        {
            get
            {
                return CastConfigCategory.Instance.Get(this.ConfigId);
            }
        }

        [BsonIgnore]
        public Unit Caster;

        [BsonIgnore]
        public List<long> Target = new List<long>(); // 技能目标

        public long StartTime;
    }
}

