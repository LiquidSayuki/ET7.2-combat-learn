using ET.EventType;

namespace ET.Server
{
    [Event(SceneType.Map)]
    public class BuffTimeout_RemoveBuff : AEvent<BuffTimeOut>
    {
        protected override async ETTask Run(Scene scene, BuffTimeOut a)
        {
            a.Unit?.GetComponent<BuffComponent>()?.Remove(a.BuffId);
            await ETTask.CompletedTask;
        }
    }
}