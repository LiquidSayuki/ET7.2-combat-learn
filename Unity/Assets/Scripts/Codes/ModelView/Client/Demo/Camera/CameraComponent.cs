using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(Unit))]
	public class CameraComponent : Entity, IAwake, IAwake<Unit>, ILateUpdate
	{
		// 战斗摄像机
		public Camera mainCamera;

		public Unit Unit;

		public Camera MainCamera
		{
			get
			{
				return this.mainCamera;
			}
		}
	}
}
