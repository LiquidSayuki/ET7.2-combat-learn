namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_BuffAddHandler :AMHandler<M2C_BuffAdd>
    {
        protected override async ETTask Run(Session session, M2C_BuffAdd message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.UnitId} AddBuff {message.BuffData.Id} ConfigId {message.BuffData.ConfigId}");

            //逻辑层创建buff
            Unit unit = zoneScene.CurrentScene().GetComponent<UnitComponent>().Get(message.UnitId);
            if (unit == null)
            {
                return;
            }
            ClientBuff clientBuff = BuffFactory.Create(unit, message.BuffData);
            unit.GetComponent<ClientBuffComponent>().Add(clientBuff);
            
            // buff添加，客户端Buff Component记录Buff信息，显示图标，信息，特效
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.BuffAdd(){Unit = unit, BuffConfigId = message.BuffData.ConfigId, BuffId = message.BuffData.Id});
            
            await ETTask.CompletedTask;
        }
    }
}