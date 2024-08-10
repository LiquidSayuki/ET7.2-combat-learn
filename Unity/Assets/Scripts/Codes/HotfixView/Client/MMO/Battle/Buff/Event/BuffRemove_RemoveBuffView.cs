using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class BuffRemove_RemoveBuffView : AEvent<BuffRemove>
    {
        protected override async ETTask Run(Scene scene, BuffRemove a)
        {
            
            
            await ETTask.CompletedTask;
        }
    }
}