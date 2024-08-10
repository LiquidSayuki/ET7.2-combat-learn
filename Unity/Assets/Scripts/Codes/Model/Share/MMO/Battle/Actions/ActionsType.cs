namespace ET
{
    public static class ActionsType
    {
        public const int NumericChange = 1; // 改变目标的数值, 如果是Buff, 移除时会还原数值

        public const int Damage = 2; // 造成伤害

        public const int CastBullet = 3; // 创建子弹

        public const int MoveToTarget = 4; // 向目标移动

        public const int CreateCast = 5; // 释放出新的子技能Cast

        public const int Attract = 6; // 把目标向中心吸附

        public const int HitFlyTarget = 7; // 目标击飞
    }
}