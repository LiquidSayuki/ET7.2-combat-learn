using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [Event(SceneType.Current)]
    [FriendOfAttribute(typeof(ET.Client.CameraComponent))]
    public class AfterUnitCreate_CreateUnitView : AEvent<EventType.AfterUnitCreate>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterUnitCreate args)
        {
            ResourcesComponent.Instance.LoadBundle("unit.unity3d");
            ResourcesComponent.Instance.LoadBundle(args.Unit.Config.PrefabName + ".unity3d");

            GameObject unitGameObject = (GameObject)ResourcesComponent.Instance.GetAsset("Unit.unity3d", "Unit");
            GameObject go = UnityEngine.Object.Instantiate(unitGameObject, GlobalComponent.Instance.Unit, true);

            GameObject gameGameObject = (GameObject)ResourcesComponent.Instance.GetAsset(args.Unit.Config.PrefabName + ".unity3d", args.Unit.Config.PrefabName);
            UnityEngine.Object.Instantiate(gameGameObject, go.transform, true);

            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;

            if (args.isPlayer)
            {
                args.Unit.AddComponent<CameraComponent, Unit>(args.Unit);
            }

            if (args.Unit.Type != UnitType.Bullet)
            {
                args.Unit.AddComponent<AnimatorComponent>();
            }

            args.Unit.Position = args.Unit.Position;
            await ETTask.CompletedTask;
        }
    }
}