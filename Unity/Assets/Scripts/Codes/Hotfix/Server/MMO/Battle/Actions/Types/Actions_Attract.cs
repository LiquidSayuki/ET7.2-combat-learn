using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    [Actions(ActionsType.Attract)]
    [FriendOf(typeof(Actions))]
    [FriendOf(typeof(BulletComponent))]
    public class Actions_Attract : IActions
    {
        public void Run(Actions actions, ActionsRunType actionsRunType)
        {
            if (actionsRunType != ActionsRunType.BulletTick)
            {
                return;
            }

            Unit unit = actions.Caster;
            List<long> target = actions.BulletSelf.Target;

            if (target.Count <= 0)
            {
                return;
            }

            ActionsConfig actionsConfig = actions.Config;

            // 吸引的距离
            float dir = float.Parse(actionsConfig.Param[0]);
            UnitComponent unitComponent = actions.DomainScene().GetComponent<UnitComponent>();

            foreach (var tId in target)
            {
                Unit u = unitComponent.Get(tId);
                if (u == null || u.IsDisposed)
                {
                    continue;
                }

                // todo: 最小有效吸引距离，可配置
                if (math.distance(u.Position, unit.Position) < 0.3f)
                {
                    continue;
                }

                // todo: 修改逻辑使目标只牵引到Bullet所在点，不会过度牵引
                float3 newPos = u.Position + math.normalize(unit.Position - u.Position) * dir;
                u.ForceSetPosition(newPos, true);
                
                Log.Console($"[Server] {u.Id} 被吸引 newPos:{newPos}");
            }
        }
    }
}