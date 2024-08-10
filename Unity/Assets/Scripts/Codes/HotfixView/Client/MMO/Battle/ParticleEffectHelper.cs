using UnityEngine;

namespace ET.Client
{
    public static class ParticleEffectHelper
    {
        public static Unit CreateParticle(Unit target, int configId)
        {
            ParticleEffectConfig config = ParticleEffectConfigCategory.Instance.Get(configId);
            string name = config.PrefabName;
            
            ResourcesComponent.Instance.LoadBundle($"Effect.unity3d");
            GameObject bundleGameObject = (GameObject)ResourcesComponent.Instance.GetAsset($"Effect.unity3d", name);
            GameObject particleGameObjectPrefab = bundleGameObject.Get<GameObject>($"{name}");
            GameObject particleGameObject = UnityEngine.Object.Instantiate(particleGameObjectPrefab);
            
            if (config.IsFollow != 0)
            {
                particleGameObject.transform.SetParent(target.GetComponent<GameObjectComponent>().GameObject.transform, false);
            }
            else
            {
                particleGameObject.transform.SetParent(GlobalComponent.Instance.Unit, false);
            }

            Unit particleUnit = UnitFactory.CreateParticleUnit(target.DomainScene());
            particleUnit.AddComponent<GameObjectComponent>().GameObject = particleGameObject;
            particleGameObject.transform.localPosition = new Vector3(config.PosX, config.PosY, config.PosZ);
            particleGameObject.transform.localScale = new Vector3(config.ScaleX, config.ScaleY, config.ScaleZ);

            OutDurationTime(particleUnit, config.TotalTime).Coroutine();

            return particleUnit;
        }

        /// <summary>
        /// 传入特效和过期时间，
        /// </summary>
        public static async ETTask OutDurationTime(Unit unit, float time)
        {
            if (time <= 0)
            {
                return;
            }

            long instanceId = unit.InstanceId;
            await TimerComponent.Instance.WaitAsync((long)time);
            if (unit.InstanceId != instanceId)
            {
                return;
            }
            unit.DomainScene()?.GetComponent<UnitComponent>().Remove(unit.Id);
        }
    }
}