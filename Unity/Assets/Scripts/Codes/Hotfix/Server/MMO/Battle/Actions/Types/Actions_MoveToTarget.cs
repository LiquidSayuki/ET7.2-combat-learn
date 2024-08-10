using System.Collections.Generic;
using MongoDB.Libmongocrypt;
using Unity.Mathematics;

namespace ET.Server
{
    [Actions(ActionsType.MoveToTarget)]
    [FriendOf(typeof(Cast))]
    [FriendOf(typeof(BulletComponent))]
    [FriendOf(typeof(Actions))]
    public class Actions_MoveToTarget : IActions
    {
        public void Run(Actions actions, ActionsRunType actionsRunType)
        {
            // 被移动的目标
            Unit unit = null;
            // 移动的Target
            List<long> target = null;

            switch (actionsRunType)
            {
                // 关于Actions中的owner和caster 分别是什么 请参考ActionsHelper
                case ActionsRunType.CastHit:
                    Cast cast = actions.CastSelf;
                    // unit是技能释放者
                    unit = cast.Caster; 
                    target = cast.Target;
                    break;
                case ActionsRunType.BulletTick:
                    BulletComponent bulletComponent = actions.BulletSelf;
                    // Bullet Tick时caster和owner都是Bullet自身的Unit
                    // 则此处会移动Bullet自身
                    unit = actions.Caster; 
                    target = bulletComponent.Target;
                    break;
                default:
                    return;
            }
            
            float3 newPos = float3.zero;
            ActionsConfig config = actions.Config;

            // 移动距离
            int dir = int.Parse(config.Param[0]);
            
            // 寻找目标
            Unit tar = null;
            if (target.Count > 0)
            {
                UnitComponent unitComponent = actions.DomainScene().GetComponent<UnitComponent>();
                Unit u = unitComponent.Get(target[0]);

                if (u != null && !u.IsDisposed && u != unit)
                {
                    tar = u;
                }
            }

            // 如果有目标则向目标移动
            // 如果没有目标则向前移动
            if (tar != null)
            {
                newPos = unit.Position + math.normalize(tar.Position - unit.Position) * dir;
            }
            else
            {
                newPos = unit.Position + math.normalize(unit.Forward) * dir;
            }

            newPos.y = unit.Position.y;
            unit.FindPathMoveToAsync(newPos).Coroutine();
            
            Log.Console($"[Server] unit {unit.Id} 向目标移动 {dir} 米 newPos:{newPos}");
        }
    }
}