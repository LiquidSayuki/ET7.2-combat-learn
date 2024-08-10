using ET.Server;

namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_CastFinishHandler : AMHandler<M2C_CastFinish>
    {
        protected override async ETTask Run(Session session, M2C_CastFinish message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.CasterId} Cast {message.CastId} End");
            
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
            
            // 技能结束，播放技能后摇，回到Idle状态，回收技能特效、UI、模型资源
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.CastFinish()
            {
                CasterId = message.CasterId,
                CastId = message.CastId
            });
            
            // 技能结束，销毁技能实体
            caster.GetComponent<ClientCastComponent>().Remove(message.CastId);
            
            await ETTask.CompletedTask;
        }
    }
}