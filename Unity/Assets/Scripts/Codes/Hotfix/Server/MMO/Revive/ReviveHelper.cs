using Unity.Mathematics;

namespace ET.Server
{
    [FriendOf(typeof(ReviveComponent))]
    public static class ReviveHelper
    {
        public static bool IsAlive(this Unit self)
        {
            return self.GetComponent<ReviveComponent>()?.Alive ?? true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int OnSiteRevive(this Unit self)
        {
            if (self.IsAlive())
            {
                return ErrorCode.ERR_Revive_Alive;
            }
            
            self.DoRevive(self.Position, 1);
            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 
        /// </summary>

        public static int PointRevive(this Unit self, float3 pos)
        {
            if (self.IsAlive())
            {
                return ErrorCode.ERR_Revive_Alive;
            }
            
            self.DoRevive(pos, 1);
            return ErrorCode.ERR_Success;
        }

        public static void DoRevive(this Unit self, float3 pos, float hpRate)
        {
            if (self.IsAlive())
            {
                return;
            }

            self.Position = pos;
            NumericComponent numericComponent = self.GetComponent<NumericComponent>();
            if (numericComponent != null)
            {
                numericComponent[NumericType.HpBase] =
                        math.clamp((int)(numericComponent[NumericType.MaxHp] * hpRate), 1, numericComponent[NumericType.MaxHp]);
                
            }

            self.GetComponent<ReviveComponent>().Alive = true;
        }
    }
}