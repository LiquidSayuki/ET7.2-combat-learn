namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_BuffUpdateHandler : AMHandler<M2C_BuffUpdate>
    {
        protected override async ETTask Run(Session session, M2C_BuffUpdate message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.UnitId} Buff {message.BuffData.Id} ConfigID {message.BuffData.ConfigId} Update");
            
            Unit unit = zoneScene.CurrentScene().GetComponent<UnitComponent>().Get(message.UnitId);
            if (unit == null)
            {
                return;
            }
            ClientBuff clientBuff = unit.GetComponent<ClientBuffComponent>().Get(message.BuffData.Id);
            if (clientBuff == null)
            {
                return;
            }
            unit.GetComponent<ClientBuffComponent>().Update(message.BuffData);
            
            // Buff信息更新
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.BuffUpdate(){Unit = unit, BuffId = message.BuffData.Id, BuffConfigId = message.BuffData.ConfigId});
            
            await ETTask.CompletedTask;
        }
    }
}