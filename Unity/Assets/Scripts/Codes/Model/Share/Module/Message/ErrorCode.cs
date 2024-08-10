namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误
        
        // 110000以下的错误请看ErrorCore.cs
        
        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        public const int ERR_ArgsError = 200101;
        public const int ERR_Cast_CasterIsNull = 200102;
        public const int ERR_Cast_TargetIsNull = 200103;
        public const int ERR_Revive_Alive = 200104;
        public const int ERR_Revive_Dead_Op = 200105;
        public const int ERR_Cast_UnitIsNull = 200106;
        public const int ERR_Cast_NumIsNull = 200107; // 数值组件为空
        public const int ERR_Cast_ForbidSkill = 200108; // 禁止释放技能
        public const int ERR_Cast_SkillCoolDown = 200109; // 冷却
    }
}