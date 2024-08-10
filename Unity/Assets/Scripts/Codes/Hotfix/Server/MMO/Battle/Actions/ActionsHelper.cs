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
        /// 由技能Cast创建的Action
        /// </summary>
        public static Actions CreateActions(this Cast self, int configId, Unit owner, ActionsRunType actionsRunType, bool autoRun = true, bool autoDispose = true)
        {
            Actions actions = self.GetComponent<ActionsTempComponent>().CreateActions(configId);
            
            // 在Cast创建的Action中
            // Caster是Cast的Caster
            // Owner是Action命中的目标，有时是技能目标，有时也可以是自身
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
        /// 由Buff运算创建的Action
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

        /// <summary>
        /// Bullet创建出的Action
        /// </summary>
        public static Actions CreateActions(this BulletComponent self, int configId, Unit owner ,Unit caster, ActionsRunType actionsRunType, bool autoRun = true, bool autoDispose = true)
        {
            Actions actions = self.GetComponent<ActionsTempComponent>().CreateActions(configId);
            
            // Bullet创建Action比较复杂
            // Bullet Awake时caster和owner都是Bullet的Caster的Unit
            // Bullet Tick时caster和owner都是Bullet自身的Unit
            // Bullet Destroy时的caster和owner都是Bullet自身的Unit
            actions.Owner = owner;
            actions.Caster = caster;
            
            RunActions(actions, actionsRunType, autoRun, autoDispose);
            
            if (actions.IsDisposed)
            {
                return null;
            }

            return actions;
        }

        private static void RunActions(Actions actions, ActionsRunType actionsRunType, bool autoRun = true, bool autoDispose = true)
        {
            if (autoRun)
            {
                if (autoDispose)
                {
                    using (actions)
                    {
                        RunActionsInner(actions, actionsRunType);
                    }
                }
                else
                {
                    RunActionsInner(actions, actionsRunType);
                }
            }
        }

        private static void RunActionsInner(Actions actions, ActionsRunType actionsRunType)
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