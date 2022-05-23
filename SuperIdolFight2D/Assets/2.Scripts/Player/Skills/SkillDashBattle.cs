using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDashBattle : PlayerBattlePersonalSkill
{
    public override void PlaySkill(SkillParameterBattle _param)
    {
        PlayerBattleEffectController.CreateEffect(PlayerBattleEffectEnum.DashTrail, _param.PlayerObj.transform);
    }
}
