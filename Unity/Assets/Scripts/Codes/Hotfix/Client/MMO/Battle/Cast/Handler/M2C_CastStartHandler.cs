using ET.EventType;

namespace ET.Client
{
    [FriendOf(typeof(ClientCast))]
    [MessageHandler(SceneType.Client)]
    public class M2C_CastStartHandler : AMHandler<M2C_CastStart>
    {
        protected override async ETTask Run(Session session, M2C_CastStart message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Debug($"Zone {zoneScene.SceneType} -> Player {message.CasterId} StartCast {message.CastConfigId} Skill {message.CastId}");

            Unit caster = zoneScene.CurrentScene().GetComponent<UnitComponent>().Get(message.CasterId);
            if (caster == null)
            {
                return;
            }
            ClientCast clientCast = CastFactory.Create(caster, message.CastId, (int)message.CastConfigId);
            clientCast.TargetsId.AddRange(message.TargetId);
            caster.GetComponent<ClientCastComponent>().Add(clientCast);

            // 向表现层抛出事件 是技能的开始
            // 在此处接入行为树或状态机
            // 开始播放技能的前摇，特效，音效
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new CastStart()
            {
                CasterId = message.CasterId,
                CastConfigId = message.CastConfigId,
                CastId = message.CastId
            });

            await ETTask.CompletedTask;
        }
    }
}