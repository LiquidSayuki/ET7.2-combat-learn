namespace ET.Server
{
    [Actions(ActionsType.CreateCast)]
    [FriendOf(typeof(Cast))]
    public class Actions_CreateCast : IActions
    {
        public void Run(Actions actions, ActionsRunType actionsRunType)
        {
            RunAsync(actions, actionsRunType).Coroutine();
        }

        public async ETTask RunAsync(Actions actions, ActionsRunType actionsRunType)
        {
            Cast cast = actions.CastSelf;
            
            // 限制仅有技能结束时附带的Action才能创建出新的技能
            if (cast == null || actionsRunType != ActionsRunType.CastFinish)
            {
                return;
            }

            ActionsConfig actionsConfig = actions.Config;

            int castConfigId = int.Parse(actionsConfig.Param[0]);
            Unit unit = cast.Caster;
            await TimerComponent.Instance.WaitFrameAsync();
            if (unit.IsDisposed)
            {
                return;
            }

            unit.CreateAndCast(castConfigId);
        }
    }
}