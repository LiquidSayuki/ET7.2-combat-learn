using Cinemachine;
using UnityEngine;
using Cinemachine;

namespace ET.Client
{
    [FriendOf(typeof(CameraComponent))]
    [FriendOf(typeof(GlobalComponent))]
    public static class CameraComponentSystem
    {
        [ObjectSystem]
        public class CameraComponentAwakeSystem : AwakeSystem<CameraComponent>
        {
            protected override void Awake(CameraComponent self)
            {
                self.Awake();
            }
        }
        
        public class CameraComponentAwakeSystem2 : AwakeSystem<CameraComponent, Unit>
        {
            protected override void Awake(CameraComponent self, Unit a)
            {
                self.Awake(a);
            }
        }

        [ObjectSystem]
        public class CameraComponentLateUpdateSystem : LateUpdateSystem<CameraComponent>
        {
            protected override void LateUpdate(CameraComponent self)
            {
                self.LateUpdate();
            }
        }

        private static void Awake(this CameraComponent self)
        {
            self.mainCamera = Camera.main;
        }
        
        private static void Awake(this CameraComponent self, Unit unit)
        {
            self.Unit = unit;
            self.mainCamera = Camera.main;
            
            var virtualCamera = GlobalComponent.Instance.Global.transform.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();

            Log.Debug("[View] 设置虚拟摄像机锁定玩家");
            
            virtualCamera.LookAt = self.Unit.GetComponent<GameObjectComponent>().GameObject.transform;
            virtualCamera.Follow = self.Unit.GetComponent<GameObjectComponent>().GameObject.transform;

            var body = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            var lookAt = virtualCamera.AddCinemachineComponent<CinemachineHardLookAt>();

            body.m_FollowOffset = new Vector3(15, 15, 15);
            body.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }

        private static void LateUpdate(this CameraComponent self)
        {
            // 摄像机每帧更新位置
            self.UpdatePosition();
        }

        private static void UpdatePosition(this CameraComponent self)
        {
            Vector3 cameraPos = self.mainCamera.transform.position;
            self.mainCamera.transform.position = new Vector3(self.Unit.Position.x, cameraPos.y, self.Unit.Position.z - 1);
        }
    }
}
