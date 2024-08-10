using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    public static class MMOMessageHelper
    {
        /// <summary>
        /// 向客户端发送消息
        /// </summary>
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
                case NoticeClientType.Self: // 仅向自身发送消息
                    SendClientSelf(unit, message);
                    break;
                case NoticeClientType.Broadcast: // 向AOI范围内广播
                    SendClientBroadcast(unit, message);
                    break;
                case NoticeClientType.BroadcastWithoutSelf: // 向AOI范围广播，但自身除外
                    SendClientBroadcastWithoutSelf(unit, message);
                    break;
            }
        }
        
        /// <summary>
        /// 强制设置Unit的位置并下发同步消息
        /// </summary>
        public static void ForceSetPosition(this Unit unit, float3 newPos, bool sendMsg = false)
        {
            // 通过寻路网格寻找一个最近的合理位置
            unit.Position = unit.GetComponent<PathfindingComponent>().RecastFindNearestPoint(newPos);
            
            // 将新位置和朝向同步给客户端
            if (sendMsg)
            {
                M2C_SetPosition msg = new M2C_SetPosition() { UnitId = unit.Id, Position = unit.Position, Rotation = unit.Rotation, };
                SendClient(unit, msg, NoticeClientType.Broadcast);
            }
        }

        private static void SendClientBroadcastWithoutSelf(Unit unit, IActorMessage message)
        {
            if (unit.GetComponent<AOIEntity>() == null)
            {
                return;
            }
            
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
        
        private static void SendClientBroadcast(Unit unit, IActorMessage message)
        {
            if (unit.GetComponent<AOIEntity>() == null)
            {
                return;
            }
            
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
        
        private static void SendClientSelf(Unit unit, IActorMessage message)
        {
            UnitGateComponent unitGateComponent = unit.GetComponent<UnitGateComponent>();
            
            // 非在线玩家类的Unit，不发送网络消息
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