namespace ET.Server
{
    [Actions(ActionsType.NumericChange)]
    [FriendOf(typeof(Actions))]
    public class Actions_NumericChange : IActions
    {
        public void Run(Actions actions, ActionsRunType actionsRunType)
        {
            Unit owner = actions.Owner;

            if (owner == null)
            {
                return;
            }

            int numericType = int.Parse(actions.Config.Param[0]);
            int numericValue = int.Parse(actions.Config.Param[1]);
            
            // 根据Actions配置参数，改变数值
            switch (actionsRunType)
            {
                case ActionsRunType.CastHit:
                case ActionsRunType.BuffAdd:
                    owner.GetComponent<NumericComponent>()[numericType] += numericValue;
                    break;
                
                case ActionsRunType.BuffRemove:
                    owner.GetComponent<NumericComponent>()[numericType] -= numericValue;
                    break;
            }
        }
    }
}