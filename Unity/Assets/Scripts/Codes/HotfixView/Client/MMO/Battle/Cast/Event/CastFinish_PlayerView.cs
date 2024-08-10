using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class CastFinish_PlayerView : AEvent<CastFinish>
    {
        protected override async ETTask Run(Scene scene, CastFinish a)
        {
            Unit unit = scene.GetComponent<UnitComponent>().Get(a.CasterId);
            if (unit == null)
            {
                return;
            }
            
            ClientCast cast = unit.GetComponent<ClientCastComponent>().Get(a.CastId);
            if (cast == null)
            {
                return;
            }
            
            unit.GetComponent<AnimatorComponent>()?.Play(MotionType.Idle);
            
            await ETTask.CompletedTask;
        }
    }
}