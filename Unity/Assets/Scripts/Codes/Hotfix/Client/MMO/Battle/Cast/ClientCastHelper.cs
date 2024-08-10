namespace ET.Client
{
    public static class ClientCastHelper
    {
        public static async ETTask<int> CastSkill(Scene zoneScene, int castConfigId)
        {
            M2C_TestCast m2CTestCast = (M2C_TestCast) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_TestCast() { CastConfigId = castConfigId });

            return m2CTestCast.Error;
        }
    }
}