using System;
using Unity.Mathematics;

namespace ET.Server
{
    [FriendOf(typeof(BulletComponent))]
    public static class UnitFactory
    {
        public static Unit Create(Scene scene, long id, UnitType unitType)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            switch (unitType)
            {
                case UnitType.Player:
                    {
                        Unit unit = unitComponent.AddChildWithId<Unit, int>(id, 1001);
                        unit.AddComponent<MoveComponent>();
                        unit.Position = new float3(-10, 0, -10);

                        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
                        numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
                        numericComponent.Set(NumericType.AOI, 15000); // 视野15米

                        unitComponent.Add(unit);
                        // 加入aoi
                        unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);
                        // 技能组件
                        unit.AddComponent<CastComponent>();
                        unit.AddComponent<BuffComponent>();
                        unit.AddComponent<SkillStatusComponent>();
                        
                        return unit;
                    }
                default:
                    throw new Exception($"not such unit type: {unitType}");
            }
        }

        /// <summary>
        /// 创建子弹
        /// </summary>
        public static Unit CreateBullet(Scene scene, long ownerId, int unitConfigId, int bulletId, float3 pos, quaternion quaternion)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChild<Unit, int>(unitConfigId);
            
            unit.Position = pos;
            unit.Rotation = quaternion;

            // 子弹需要的功能组件
            unit.AddComponent<CastComponent>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<PathfindingComponent, string>(scene.Name);
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            
            numericComponent.Set(NumericType.Speed, 6f); // todo: 移动速度读取配置表
            numericComponent.Set(NumericType.AOI, 15000); // todo： 可视范围读取配置表
            
            BulletComponent bulletComponent = unit.AddComponent<BulletComponent, int>(bulletId);
            bulletComponent.OwnerId = ownerId;
            
            unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);
            
            unitComponent.Add(unit);
            return unit;
        }

        /// <summary>
        /// 创建怪物
        /// </summary>
        public static Unit CreateMonster(Scene scene, int unitConfigId, float3 pos)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChild<Unit, int>(unitConfigId);
            
            // 添加怪物需要的功能组件
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<PathfindingComponent, string>(scene.Name);
            unit.Position = pos;

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            numericComponent.Set(NumericType.Speed, 6.0f);
            numericComponent.Set(NumericType.AOI, 15000);
            numericComponent.Set(NumericType.MaxHp, 1000);
            numericComponent.Set(NumericType.Hp, 1000);

            unit.AddComponent<ReviveComponent>();
            unit.AddComponent<CastComponent>();
            unit.AddComponent<BuffComponent>();
            
            // 怪物的AOI组件在外面挂载了
            
            unitComponent.Add(unit);
            return unit;
        }
    }
}