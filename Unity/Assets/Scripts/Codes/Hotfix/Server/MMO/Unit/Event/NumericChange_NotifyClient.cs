using System.Collections.Generic;
using ET.EventType;

namespace ET.Server
{
    [Event(SceneType.None)]
    public class NumericChange_NotifyClient : AEvent<NumbericChange>
    {
        protected override async ETTask Run(Scene scene, NumbericChange a)
        {
            M2C_NumericChange m2c_numericChange = new M2C_NumericChange() { UnitId = a.Unit.Id, KV = new Dictionary<int, long>(), };
            m2c_numericChange.KV.Add(a.NumericType, a.New);
            MMOMessageHelper.SendClient(a.Unit, m2c_numericChange, NoticeClientType.Broadcast);
            await ETTask.CompletedTask;
        }
    }
}