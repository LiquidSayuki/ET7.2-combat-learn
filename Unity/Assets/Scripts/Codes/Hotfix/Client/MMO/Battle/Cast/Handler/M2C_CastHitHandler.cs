namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    [FriendOfAttribute(typeof(ET.Client.ClientCast))]
    public class M2C_CastHitHandler : AMHandler<M2C_CastHit>
    {
        protected override async ETTask Run(Session session, M2C_CastHit message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.CasterId} Cast {message.CastId} Hit Target{message.TargetId.ListToString()}");

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

            // 清空之前的命中目标列表，重新瞄准服务端下发的命中目标
            clientCast.TargetsId.Clear();
            foreach (var targetId in message.TargetId)
            {
                Unit target = zoneScene.CurrentScene().GetComponent<UnitComponent>().Get(targetId);
                if (target == null || target.IsDisposed)
                {
                    continue;
                }
                clientCast.TargetsId.Add(targetId);
            }

            // 技能命中，播放技能命中的动画，受击动画等
            foreach (long targetId in message.TargetId)
            {
                EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.CastHit()
                {
                    CasterId = message.CasterId,
                    CastId = message.CastId,
                    TargetId = targetId
                });
            }

            await ETTask.CompletedTask;
        }
    }
}