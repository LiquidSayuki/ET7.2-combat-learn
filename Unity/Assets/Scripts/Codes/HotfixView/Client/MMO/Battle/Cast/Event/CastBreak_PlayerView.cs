using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class CastBreak_PlayerView : AEvent<CastBreak>
    {
        protected override async ETTask Run(Scene scene, CastBreak a)
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
            
            // 技能打断，用户变为idle状态
            unit.GetComponent<AnimatorComponent>()?.Play(MotionType.Idle);
            
            await ETTask.CompletedTask;
        }
    }
}