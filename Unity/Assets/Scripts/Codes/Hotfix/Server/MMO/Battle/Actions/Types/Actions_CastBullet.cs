using Unity.Mathematics;

namespace ET.Server
{
    [Actions(ActionsType.CastBullet)]
    [FriendOf(typeof(Cast))]
    public class Actions_CastBullet : IActions
    {
        public void Run(Actions actions, ActionsRunType actionsRunType)
        {
            Cast cast = actions.CastSelf;

            if (cast == null || actionsRunType != ActionsRunType.CastHit)
            {
                return;
            }

            if (cast.Target.Count <= 0)
            {
                return;
            }

            ActionsConfig config = actions.Config;

            UnitComponent unitComponent = actions.DomainScene().GetComponent<UnitComponent>();

            foreach (var uid in cast.Target)
            {
                Unit unit = unitComponent.Get(uid);
                if (unit == null)
                {
                    continue;
                }

                int unitId = int.Parse(config.Param[0]); // 子弹的Unit实体要用的ConfigId
                int bulletId = int.Parse(config.Param[1]); // 子弹的子弹配置效果ConfigId

                float3 startPos = unit.Position + (unit.Forward * 1.2f);
                Unit bullet = UnitFactory.CreateBullet(cast.DomainScene(), cast.Caster.Id, unitId, bulletId, startPos, unit.Rotation);
                
                // 启动子弹效果
                bullet.GetComponent<BulletComponent>().Start();
            }
        }
    }
}