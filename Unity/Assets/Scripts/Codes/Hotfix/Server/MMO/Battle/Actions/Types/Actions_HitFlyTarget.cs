using Unity.Mathematics;

namespace ET.Server
{
    [Actions(ActionsType.HitFlyTarget)]
    [FriendOf(typeof(Buff))]
    [FriendOf(typeof(Cast))]
    public class Actions_HitFlyTarget : IActions
    {
        public void Run(Actions actions, ActionsRunType actionsRunType)
        {
            Unit caster = null;
            switch (actionsRunType)
            {
                case ActionsRunType.BuffTick:
                    caster = actions.BuffSelf.Owner;
                    break;
                case ActionsRunType.CastHit:
                    caster = actions.CastSelf.Caster;
                    break;
            }

            // 通过配置获取击飞技能的属性
            ActionsConfig actionsConfig = actions.Config;
            float range = float.Parse(actionsConfig.Param[0]); // 冲撞范围
            float dir = float.Parse(actionsConfig.Param[1]);
            int buffId = int.Parse(actionsConfig.Param[2]);

            foreach (var aoiEntity in caster.GetBeSeeUnits().Values)
            {
                Unit unit = aoiEntity.GetParent<Unit>();
                
                // todo: 不击飞玩家和怪物以外的目标 可转为配置
                if (unit.Type != UnitType.Player && unit.Type != UnitType.Monster)
                {
                    continue;
                }

                if (unit == caster)
                {
                    continue;
                }
                
                if (math.length(unit.Position - caster.Position) < range)
                {
                    float3 unitPos = new float3(unit.Position.x, 0, unit.Position.z);
                    float3 casterPos = new float3(caster.Position.x, 0, caster.Position.z);
                    float3 targetDir = math.normalize(unitPos - casterPos);
                    float3 forwordDir = caster.Forward;
                    forwordDir.y = 0.0f;

                    float3 newPos = unitPos + forwordDir * dir;
                    
                    unit.FindPathMoveToAsync(newPos + forwordDir * dir).Coroutine();

                    // 对被冲击的目标添加Buff
                    if (buffId != 0)
                    {
                        unit.GetComponent<BuffComponent>()?.CreateAndAdd(buffId);
                    }
                }
            }
        }
    }
}