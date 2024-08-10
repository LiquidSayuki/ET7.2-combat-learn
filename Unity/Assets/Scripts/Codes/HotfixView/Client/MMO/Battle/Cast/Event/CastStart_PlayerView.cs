using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class CastStart_PlayerView : AEvent<CastStart>
    {
        protected override async ETTask Run(Scene scene, CastStart a)
        {
            Unit unit = scene.GetComponent<UnitComponent>().Get(a.CasterId);
            if (unit == null)
            {
                return;
            }
            
            CastConfig castConfig = CastConfigCategory.Instance.Get((int)a.CastConfigId);
            
            // 起手动画
            unit.GetComponent<AnimatorComponent>()?.Play((MotionType) castConfig.StartAnimation);
            
            // 起手特效
            foreach (int effectID in castConfig.StartEffect)
            {
                ParticleEffectHelper.CreateParticle(unit, effectID);
            }
            
            await ETTask.CompletedTask;
        }
    }
}