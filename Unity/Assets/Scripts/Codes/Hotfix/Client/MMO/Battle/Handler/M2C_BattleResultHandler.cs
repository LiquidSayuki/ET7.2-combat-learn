namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_BattleResultHandler : AMHandler<M2C_BattleResult>
    {
        protected override async ETTask Run(Session session, M2C_BattleResult message)
        {
            Log.Debug("[Client] battle result");
            await ETTask.CompletedTask;
        }
    }
}