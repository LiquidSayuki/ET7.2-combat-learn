using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET.Server
{
    [ComponentOf(typeof(Unit))]
    public class BuffComponent : Entity, IAwake, IDestroy, IDeserialize, ITransfer
    {
        [BsonIgnore]
        public Dictionary<int, Buff> ConfigIdBuffs = new Dictionary<int, Buff>();
    }
}