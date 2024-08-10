using Unity.Mathematics;

namespace ET.Server
{
    public class MonsterMapComponentAwakeSystem: AwakeSystem<MonsterMapComponent>
    {
        protected override void Awake(MonsterMapComponent self)
        {
            if (self.DomainScene().Name.Equals("GateMap"))
            {
                return;
            }
            
            // 游戏服务器启动时所有怪物都生成
            foreach (int monsterId in MonsterConfigCategory.Instance.GetAll().Keys)
            {
                self.CreateMonster(monsterId);
            }
        }
    }
    
    public class MonsterFlagAwakeSystem: AwakeSystem<MonsterFlag, int, int>
    {
        protected override void Awake(MonsterFlag self, int a, int b)
        {
            self.ConfigId = a;
            self.GroupConfigId = b;
        }
    }

    public class MonsterFlagDestroySystem: DestroySystem<MonsterFlag>
    {
        protected override void Destroy(MonsterFlag self)
        {
            self.DomainScene().GetComponent<MonsterMapComponent>().OnMonsterDead(self.ConfigId, self.GroupConfigId);
        }
    }

    public class CreateMonsterInfoAwakeSystem: AwakeSystem<CreateMonsterInfo, int>
    {
        protected override void Awake(CreateMonsterInfo self, int a)
        {
            self.monsterId = a;
        }
    }
    
    [Invoke(TimerInvokeType.CreateMonster)]
    [FriendOf(typeof(CreateMonsterInfo))]
    public class CreateMonster_TimerHandler : ATimer<CreateMonsterInfo>
    {
        protected override void Run(CreateMonsterInfo t)
        {
            t.GetParent<MonsterMapComponent>().CreateMonster(t.monsterId);
        }
    }

    [Invoke(TimerInvokeType.MonsterDead)]
    public class MonsterDead_TimerHandler: ATimer<Unit>
    {
        protected override void Run(Unit t)
        {
            t?.Dispose();
        }
    }

    public static class MonsterMapComponentSystem
    {
        public static void OnMonsterDead(this MonsterMapComponent self, int id, int groupId)
        {
            // 怪物死亡时，启用一个定时器
            // 在一定的时间后，重新刷新出一个怪物
            TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 3000, TimerInvokeType.CreateMonster, self.AddChild<CreateMonsterInfo, int>(id));
        }
        
        public static Unit CreateMonster(this MonsterMapComponent self, int id)
        {
            MonsterConfig monsterConfig = MonsterConfigCategory.Instance.Get(id);
            MonsterGroupConfig groupConfig = MonsterGroupConfigCategory.Instance.Get(monsterConfig.GroupId);

            // 根据groupConfig随机确定一个位置
            int h_range = groupConfig.Range / 2;
            float3 pos = new float3(groupConfig.PosX, groupConfig.PosY, groupConfig.PosZ)
            + new float3(RandomGenerator.RandomNumber(-h_range, h_range), 0, RandomGenerator.RandomNumber(-h_range, h_range));
            
            // 创建怪物
            Unit unit = UnitFactory.CreateMonster(self.DomainScene(), monsterConfig.UnitConfigId, pos);
            unit.AddComponent<MonsterFlag, int, int>(id, monsterConfig.GroupId);
            unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);
            
            return unit;
        }
    }
}