using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class MoveStop_PlayerView : AEvent<MoveStop>
    {
        protected override async ETTask Run(Scene scene, MoveStop a)
        {
            Unit unit = a.Unit;
            if (unit == null)
            {
                return;
            }
            
            unit.GetComponent<AnimatorComponent>().SetFloatValue("Speed", 0f);
            
            await ETTask.CompletedTask;
        }
    }
}