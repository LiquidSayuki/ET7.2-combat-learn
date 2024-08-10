using Unity.Mathematics;

namespace ET
{
    public static class UnitHelper
    {
        public static void SetRotation(this Unit unit, quaternion quaternion)
        {
            if ((unit.GetComponent<NumericComponent>()?.GetAsInt(NumericType.ForbidRotation) ?? 0) > 0)
            {
                return;
            }

            unit.Rotation = quaternion;
        }
    }
}