using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class BuffAdd_CreateBuffView : AEvent<BuffAdd>
    {
        protected override async ETTask Run(Scene scene, BuffAdd a)
        {
            ClientBuff buff = a.Unit.GetComponent<ClientBuffComponent>().Get(a.BuffId);
            if (buff == null)
            {
                return;
            }

            BuffConfig config = buff.Config;
            foreach (var effectId in config.OwnerEffect)
            {
                ParticleEffectHelper.CreateParticle(a.Unit, effectId);
            }
            
            await ETTask.CompletedTask;
        }
    }
}