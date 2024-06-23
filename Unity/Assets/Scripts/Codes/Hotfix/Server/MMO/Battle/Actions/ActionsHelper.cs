namespace ET.Server
{
    [FriendOf(typeof(Actions))]
    [FriendOf(typeof(Cast))]
    [FriendOf(typeof(Buff))]
    public static class ActionsHelper
    {
        public static Actions CreateActions(this ActionsTempComponent self, int configId)
        {
            return self.AddChild<Actions, int>(configId);
        }

        /// <summary>
        /// 技能命中创建的Action
        /// </summary>
        /// <param name="configId"> 行为效果的Id </param>
        /// <param name="owner"> 行为效果的目标 </param>
        /// <param name="actionsRunType"></param>
        /// <param name="autoRun"></param>
        /// <param name="autoDispose"></param>
        /// <returns></returns>
        public static Actions CreateActions(this Cast self, int configId, Unit owner, ActionsRunType actionsRunType, bool autoRun = true, bool autoDispose = true)
        {
            Actions actions = self.GetComponent<ActionsTempComponent>().CreateActions(configId);
            actions.Caster = self.Caster;
            actions.Owner = owner;

            RunActions(actions, actionsRunType, autoRun, autoDispose);

            if (actions.IsDisposed)
            {
                return null;
            }

            return actions;
        }

        /// <summary>
        /// Buff运算创建的Action
        /// </summary>
        public static Actions CreateActions(this Buff self, int configId, ActionsRunType actionsRunType, bool autoRun = true, bool autoDispose = true)
        {
            Actions actions = self.GetComponent<ActionsTempComponent>().CreateActions(configId);
            actions.Caster = self.Owner;
            
            RunActions(actions, actionsRunType, autoRun, autoDispose);
            
            if (actions.IsDisposed)
            {
                return null;
            }

            return actions;
        }

        public static void RunActions(Actions actions, ActionsRunType actionsRunType, bool autoRun = true, bool autoDispose = true)
        {
            if (autoRun)
            {
                if (autoDispose)
                {
                    using (actions)
                    {
                        RunActions(actions, actionsRunType);
                    }
                }
                else
                {
                    RunActions(actions, actionsRunType);
                }
            }
        }

        public static void RunActions(Actions actions, ActionsRunType actionsRunType)
        {
            IActions actionsHandle = ActionsDispatcherComponent.Instance.Get(actions.Config.Type);

            if (actionsHandle == null)
            {
                Log.Error($"Error Actions type not found, UnitId : {actions.Owner?.Id}, ActionsConfigId: {actions.ConfigId}");
                actions.Dispose();
                return;
            }

            actionsHandle.Run(actions, actionsRunType);
        }
    }
}