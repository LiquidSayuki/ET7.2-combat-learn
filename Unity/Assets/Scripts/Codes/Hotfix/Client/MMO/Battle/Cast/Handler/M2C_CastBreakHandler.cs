namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_CastBreakHandler : AMHandler<M2C_CastBreak>
    {
        protected override async ETTask Run(Session session, M2C_CastBreak message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.CasterId} Cast {message.CastId} Break");
            
            Unit caster = zoneScene.CurrentScene().GetComponent<UnitComponent>().Get(message.CasterId);
            if (caster == null)
            {
                return;
            }
            ClientCast clientCast = caster.GetComponent<ClientCastComponent>().Get(message.CastId);
            if (clientCast == null)
            {
                return;
            }
            
            // 技能打断，Caster回到Idle状态，回收技能相关资源
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.CastBreak()
            {
                CasterId = message.CasterId,
                CastId = message.CastId
            });
            
            // 技能打断，销毁技能实体
            caster.GetComponent<ClientCastComponent>().Remove(message.CastId);
            
            await ETTask.CompletedTask;
        }
    }
}