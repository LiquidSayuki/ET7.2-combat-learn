using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class MoveStart_PlayerView : AEvent<MoveStart>
    {
        protected override async ETTask Run(Scene scene, MoveStart a)
        {
            Unit unit = a.Unit;
            if (unit == null)
            {
                return;
            }
            
            unit.GetComponent<AnimatorComponent>().SetFloatValue("Speed", 1f);
            
            await ETTask.CompletedTask;
        }
    }
}