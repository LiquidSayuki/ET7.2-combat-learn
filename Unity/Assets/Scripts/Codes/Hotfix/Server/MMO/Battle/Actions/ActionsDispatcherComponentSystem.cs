using System;
using System.Collections.Generic;

namespace ET.Server
{
    public class ActionsDispatcherComponentAwakeSystem: AwakeSystem<ActionsDispatcherComponent>
    {
        protected override void Awake(ActionsDispatcherComponent self)
        {
            ActionsDispatcherComponent.Instance = self;
            self.Load();
        }
    }
    
    /// <summary>
    /// Load生命周期将会在热重载时调用
    /// </summary>
    public class ActionsDispatcherComponentLoadSystem: LoadSystem<ActionsDispatcherComponent>
    {
        protected override void Load(ActionsDispatcherComponent self)
        {
            self.Load();
        }
    }
    
    public class ActionsDispatcherComponentDestroySystem : DestroySystem<ActionsDispatcherComponent>
    {
        protected override void Destroy(ActionsDispatcherComponent self)
        {
            ActionsDispatcherComponent.Instance = null;
            self.Dict.Clear();
        }
    }
    
    [FriendOf(typeof(ActionsDispatcherComponent))]
    public static class ActionsDispatcherComponentSystem
    {
        public static void Load(this ActionsDispatcherComponent self)
        {
            self.Dict.Clear();
            
            // 获取所有具有ActionsAttribute标签的Type
            HashSet<Type> types = EventSystem.Instance.GetTypes(typeof (ActionsAttribute));
            foreach (Type type in types)
            {
                var attrs = type.GetCustomAttributes(typeof (ActionsAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }
                
                // 对于每一个拥有ActionsAttribute标签的类型，创建一个对应的实体
                ActionsAttribute actionsAttribute = attrs[0] as ActionsAttribute;
                object obj = Activator.CreateInstance(type);
                
                // 每一个拥有ActionsAttribute标签的类型都应该实现IActions接口
                IActions iActions = obj as IActions;
                if (iActions == null)
                {
                    throw new Exception($"class: {type.Name} not inherit from IActions");
                }

                // 保存每一个ActionType对应的接口实例，以调用其Run函数
                self.Dict[actionsAttribute.ActionsType] = iActions;
            }
        }

        public static IActions Get(this ActionsDispatcherComponent self, int type)
        {
            if (self.Dict.TryGetValue(type, out IActions iActions))
            {
                return iActions;
            }
            return null;
        }
    }
}