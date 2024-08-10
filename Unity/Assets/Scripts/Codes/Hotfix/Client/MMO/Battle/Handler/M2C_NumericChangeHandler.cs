namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class M2C_NumericChangeHandler : AMHandler<M2C_NumericChange>
    {
        protected override async ETTask  Run(Session session, M2C_NumericChange message)
        {
            UnitComponent unitComponent = session.DomainScene().CurrentScene().GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                return;
            }

            Unit unit = unitComponent.Get(message.UnitId);
            if (unit == null)
            {
                return;
            }

            foreach (var kv in message.KV)
            {
                unit.GetComponent<NumericComponent>().Set(kv.Key, kv.Value);
            }

            await ETTask.CompletedTask;
        }
    }
}