using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBattleEffectEnum
{
    DashTrail,
}
public class PlayerBattleEffectController : MonoBehaviour
{
    private static readonly string RESOURCE_EFFECT_PATH = "BattleEffect/";

    private static Dictionary<PlayerBattleEffectEnum, string> EffectString = new Dictionary<PlayerBattleEffectEnum, string>()
    {
        {PlayerBattleEffectEnum.DashTrail, "DashTrail"}
    };

    public static GameObject CreateEffect(PlayerBattleEffectEnum _effect, Transform _parent)
    {
        GameObject newEffect = Resources.Load<GameObject>(RESOURCE_EFFECT_PATH + EffectString[_effect]);
        if (_parent)
        {
            return Instantiate(newEffect, _parent);
        }
        return Instantiate(newEffect, Vector3.zero, Quaternion.identity);
    }

}
