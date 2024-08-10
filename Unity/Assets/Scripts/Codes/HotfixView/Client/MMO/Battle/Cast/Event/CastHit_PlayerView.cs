using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class CastHit_PlayerView : AEvent<CastHit>
    {
        protected override async ETTask Run(Scene scene, CastHit a)
        {
            Unit unit = scene.GetComponent<UnitComponent>().Get(a.TargetId);
            if (unit == null)
            {
                return;
            }
            
            Unit caster = scene.GetComponent<UnitComponent>().Get(a.CasterId);
            if (caster == null)
            {
                return;
            }


            ClientCast cast = caster.GetComponent<ClientCastComponent>().Get(a.CastId);
            if (cast == null)
            {
                return;
            }

            CastConfig castConfig = cast.Config;
            
            unit.GetComponent<AnimatorComponent>()?.Play((MotionType) castConfig.HitAnimation);

            foreach (var hitEffectId in castConfig.HitEffect)
            {
                ParticleEffectHelper.CreateParticle(unit, hitEffectId);
            }
            
            await ETTask.CompletedTask;
        }
    }
}