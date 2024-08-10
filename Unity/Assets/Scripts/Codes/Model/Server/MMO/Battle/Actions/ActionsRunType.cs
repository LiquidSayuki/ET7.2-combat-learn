namespace ET.Server
{
    /// <summary>
    /// 技能执行的类型
    /// </summary>
    public enum ActionsRunType
    {
        BuffAdd,
        BuffTick,
        BuffRemove,
        
        CastHit,
        CastFinish,
        
        BulletDestroy,
        BulletAwake,
        BulletTick,
    }
}