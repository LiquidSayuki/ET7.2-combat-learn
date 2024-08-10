using System;

namespace ET.Server
{
    [FriendOf(typeof(ReviveComponent))]
    public static class BattleHelper
    {
        public static int CanCastSkill(Unit unit, int castConfigId)
        {
            if (!CastConfigCategory.Instance.Contain(castConfigId))
            {
                return ErrorCode.ERR_ArgsError;
            }

            if (!unit.IsAlive())
            {
                return ErrorCode.ERR_Revive_Dead_Op;
            }

            int error = unit.GetComponent<SkillStatusComponent>()?.CanCastSkill(castConfigId) ?? ErrorCode.ERR_Success;
            return error;
        }
        
        /// <summary>
        /// 战斗结算
        /// 战斗结算必须为同步函数，不能为异步逻辑
        /// </summary>
        public static void CalAttack(Unit attacker, Unit target, Actions actions)
        {
            // todo: 根据战斗时的属性去计算伤害
            long damage = long.Parse(actions.Config.Param[0]);

            NumericComponent numericComponent = target.GetComponent<NumericComponent>();
            long oldHp = numericComponent[NumericType.Hp];
            long tarHp = numericComponent[NumericType.Hp] + damage;
            numericComponent[NumericType.HpBase] = Math.Clamp(tarHp, 0, numericComponent[NumericType.MaxHp]);

            long newHp = numericComponent[NumericType.Hp];
            long res_damage = newHp - oldHp;

            // 被命中者技能被打断
            if (res_damage > 0)
            {
                target.GetComponent<SkillStatusComponent>()?.BreakSkill();
            }
            
            // 广播飘字
            if (res_damage != 0)
            {
                MMOMessageHelper.SendClient(target, new M2C_BattleResult() { AttackerId = attacker.Id, TargetId = target.Id, Damage = res_damage }, NoticeClientType.Broadcast);
            }

            // 死亡
            if (oldHp > 0 && newHp == 0)
            {
                Kill(attacker, target);
            }
        }

        /// <summary>
        /// 发生击杀的逻辑
        /// </summary>
        public static void Kill(Unit killer, Unit killed)
        {
            OnDead(killed);
        }

        /// <summary>
        /// 死亡逻辑
        /// </summary>
        public static void OnDead(Unit killed)
        {
            ReviveComponent reviveComponent = killed.GetComponent<ReviveComponent>();

            if (reviveComponent == null)
            {
                return;
            }

            if (!killed.IsAlive())
            {
                return;
            }

            killed.Stop(0);
            reviveComponent.Alive = false;

            switch (killed.Type)
            {
                // todo : 玩家死亡的逻辑
                case UnitType.Player:
                    break;
                
                // 怪物死后3秒自动消失
                case UnitType.Monster:
                    TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 3000, TimerInvokeType.MonsterDead, killed);
                    break;
            }
        }
    }
}