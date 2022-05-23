using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBattlePersonalSkill
{
    public virtual void PlaySkill(SkillParameterBattle _param) { }

}


public class SkillParameterBattle
{
    public GameObject PlayerObj;
    public PlayerAnimationBattle AnimatorController;
    public SpriteRenderer MyRenderer;
    public Rigidbody2D PlayerBody;
    public Collider2D PlayerMainCollider;
}