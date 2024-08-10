namespace ET.Server
{
    [Actions(ActionsType.Damage)]
    [FriendOf(typeof(Cast))]
    public class Actions_Damage : IActions
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

            UnitComponent unitComponent = actions.DomainScene().GetComponent<UnitComponent>();

            foreach (long unitId  in cast.Target)
            {
                Unit unit = unitComponent.Get(unitId);

                if (unit == null || unit.IsDisposed)
                {
                    continue;
                }
                
                BattleHelper.CalAttack(cast.Caster, unit, actions);
            }
        }
    }
}