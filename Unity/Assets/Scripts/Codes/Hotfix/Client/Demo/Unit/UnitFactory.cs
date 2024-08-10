﻿using Unity.Mathematics;

namespace ET.Client
{
    public static class UnitFactory
    {
        public static Unit Create(Scene currentScene, UnitInfo unitInfo, bool isPlayer = false)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
	        unitComponent.Add(unit);
	        
	        unit.Position = unitInfo.Position;
	        unit.Forward = unitInfo.Forward;
	        
	        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

			foreach (var kv in unitInfo.KV)
			{
				numericComponent.Set(kv.Key, kv.Value);
			}
	        
	        unit.AddComponent<MoveComponent>();
	        if (unitInfo.MoveInfo != null)
	        {
		        if (unitInfo.MoveInfo.Points.Count > 0)
				{
					unitInfo.MoveInfo.Points[0] = unit.Position;
					unit.MoveToAsync(unitInfo.MoveInfo.Points).Coroutine();
				}
	        }

	        unit.AddComponent<ObjectWait>();
	        unit.AddComponent<XunLuoPathComponent>();
	        unit.AddComponent<ClientCastComponent>();
	        unit.AddComponent<ClientBuffComponent>();
	        
	        EventSystem.Instance.Publish(unit.DomainScene(), new EventType.AfterUnitCreate() {Unit = unit, isPlayer = isPlayer});
            return unit;
        }

        public static Unit CreateParticleUnit(Scene currentScene)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChild<Unit, int>(10003);
	        unitComponent.Add(unit);

	        unit.AddComponent<ObjectWait>();
	        return unit;
        }
    }
}
