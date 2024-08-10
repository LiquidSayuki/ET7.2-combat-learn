namespace ET.Server
{
    [FriendOf(typeof(Cast))]
    public static class CastHelper
    {
        /// <summary>
        /// 创建并释放一个Cast
        /// </summary>
        public static int CreateAndCast(this Unit caster, int castConfigId)
        {
            return CreateCast(caster, castConfigId).Cast();
        }

        /// <summary>
        /// 创建一个Cast
        /// </summary>
        public static Cast CreateCast(this Unit caster, int castConfigId)
        {
            CastComponent castComponent = caster.GetComponent<CastComponent>();
            if (castComponent == null)
            {
                return null;
            }

            Cast cast = castComponent.Create(castConfigId);
            cast.Caster = caster;

            // 施法者的技能状态组件进入释放技能状态
            caster.GetComponent<SkillStatusComponent>()?.StartSkill(cast);

            return cast;
        }
    }
}