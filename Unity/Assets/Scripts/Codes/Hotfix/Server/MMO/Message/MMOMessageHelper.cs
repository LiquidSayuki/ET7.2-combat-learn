using System.Collections.Generic;

namespace ET.Server
{
    public static class MMOMessageHelper
    {
        public static void SendClient(Unit unit, IActorMessage message, NoticeClientType noticeClientType)
        {
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            switch (noticeClientType)
            {
                case NoticeClientType.NoNotice:
                    break;
                case NoticeClientType.Self:
                    SendClientSelf(unit, message);
                    break;
                case NoticeClientType.Broadcast:
                    SendClientBroadcast(unit, message);
                    break;
                case NoticeClientType.BroadcastWithoutSelf:
                    SendClientBroadcastWithoutSelf(unit, message);
                    break;
            }
        }

        public static void SendClientBroadcastWithoutSelf(Unit unit, IActorMessage message)
        {
            // 获取AOI范围内能够观察到Unit的所有人
            Dictionary<long, AOIEntity> dict = unit.GetBeSeePlayers();
            
            if (dict.Count <= 0)
            {
                return;
            }
            
            foreach (var aoiEntity in dict.Values)
            {
                Unit u = aoiEntity.Unit;

                if (u == null || u.IsDisposed)
                {
                    continue;
                }
                
                if (u == unit)
                {
                    continue;
                }
                
                SendClientSelf(u,message);
            }
        }
        
        public static void SendClientBroadcast(Unit unit, IActorMessage message)
        {
            Dictionary<long, AOIEntity> dict = unit.GetBeSeePlayers();

            if (dict.Count <= 0)
            {
                return;
            }

            foreach (var aoiEntity in dict.Values)
            {
                Unit u = aoiEntity.Unit;

                if (u == null || u.IsDisposed)
                {
                    continue;
                }
                
                SendClientSelf(u,message);
            }
        }
        
        public static void SendClientSelf(Unit unit, IActorMessage message)
        {
            UnitGateComponent unitGateComponent = unit.GetComponent<UnitGateComponent>();
            if (unitGateComponent == null)
            {
                return;
            }

            if (unitGateComponent.GateSessionActorId == 0)
            {
                return;
            }
            
            MessageHelper.SendActor(unitGateComponent.GateSessionActorId, message);
        }
    }
}