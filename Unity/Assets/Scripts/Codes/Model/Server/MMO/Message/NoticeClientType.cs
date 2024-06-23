namespace ET
{
    public enum NoticeClientType
    {
        NoNotice = 0, //不通知
        Self = 1, //通知自己客户端
        Broadcast = 2, // 通知AOI
        BroadcastWithoutSelf = 3, // 通知AOI，除了自身
    }
}