using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET.Server
{
    public enum SkillStatusType
    {
        New = 0,
        Init = 1,
        Running = 2,
        Finish = 3
    }
    
    [ComponentOf(typeof(Unit))]
    public class SkillStatusComponent : Entity, IAwake, IDestroy
    {
        public long CurSkillCastInstanceId = default;

        public long CurSkillCastId = default;

        public long CurSkillStartTime = default;

        public SkillStatusType CurSkillStatusType = SkillStatusType.New;

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, long> CoolDowns = new Dictionary<int, long>();
    }
}