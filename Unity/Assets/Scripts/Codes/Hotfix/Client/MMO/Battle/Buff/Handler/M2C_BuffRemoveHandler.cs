namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_BuffRemoveHandler : AMHandler<M2C_BuffRemove>
    {
        protected override async ETTask Run(Session session, M2C_BuffRemove message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.UnitId} RemoveBuff {message.BuffId}");
            
            Unit unit = zoneScene.CurrentScene().GetComponent<UnitComponent>().Get(message.UnitId);
            if (unit == null)
            {
                return;
            }
            ClientBuff clientBuff = unit.GetComponent<ClientBuffComponent>().Get(message.BuffId);
            if (clientBuff == null)
            {
                return;
            }
            
            // buff移除，客户端Buff Component记录Buff信息，回收特效
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.BuffRemove(){Unit = unit, BuffId = message.BuffId});
            
            // 逻辑层移除buff
            unit.GetComponent<ClientBuffComponent>().Remove(message.BuffId);
            
            await ETTask.CompletedTask;
        }
    }
}