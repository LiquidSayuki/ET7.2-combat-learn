namespace ET.Server
{
    [FriendOf(typeof(Cast))]
    public static class CastHelper
    {
        public static Cast Create(this Unit caster, int castConfigId)
        {
            CastComponent castComponent = caster.GetComponent<CastComponent>();

            if (castComponent == null)
            {
                return null;
            }

            Cast cast = castComponent.Create(castConfigId);
            cast.Caster = caster;

            return cast;
        }

        /// <summary>
        /// 创建并释放一个Cast
        /// </summary>
        public static int CreateAndCast(this Unit caster, int castConfigId)
        {
            return Create(caster, castConfigId).Cast();
        }
    }
}