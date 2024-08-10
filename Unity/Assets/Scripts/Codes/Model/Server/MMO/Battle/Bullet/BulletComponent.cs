using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET.Server
{
    [ComponentOf(typeof(Unit))]
    public class BulletComponent : Entity, IAwake<int>, IDestroy
    {
        public int ConfigId = default;

        [BsonIgnore]
        public BulletConfig Config
        {
            get
            {
                return BulletConfigCategory.Instance.Get(this.ConfigId);
            }
        }

        public int TickCount = default;

        public List<long> Target = new List<long>();
        
        // Bullet的Parent是子弹自身的Unit实体，并不是发射者
        // 子弹发射者的Unit Id单独记录在此处
        public long OwnerId;
        
        public long TickTimer;

        public long TickTimer2 = default;

        public long TickTimer3 = default;

        public long TotalTimer = default;
    }
}