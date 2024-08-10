namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_BuffTickHandler : AMHandler<M2C_BuffTick>
    {
        protected override async ETTask Run(Session session, M2C_BuffTick message)
        {
            Scene zoneScene = session.ClientScene();
            Log.Console($"Zone {zoneScene.SceneType} -> Player {message.UnitId} Buff {message.BuffId} Tick");
            
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
            
            // buff读秒, 表现层播放buff相关动画, 比如流血飘字
            EventSystem.Instance.Publish(zoneScene.CurrentScene(), new EventType.BuffTick(){Unit = unit, BuffId = message.BuffId});
            
            await ETTask.CompletedTask;
        }
    }
}