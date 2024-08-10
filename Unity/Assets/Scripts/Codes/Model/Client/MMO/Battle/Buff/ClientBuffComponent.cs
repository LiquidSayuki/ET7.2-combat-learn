using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class ClientBuffComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<long, ClientBuff> Buffs = new Dictionary<long, ClientBuff>();
    }
}