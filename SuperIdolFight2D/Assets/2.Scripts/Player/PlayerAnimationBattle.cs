using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBattleAnimation
{
    Idle, IdleCrawl, Jump, Run, Crawl, Cling
}
public class PlayerAnimationBattle : MonoBehaviour
{
    private Animator myAnimatorController;
    private Dictionary<PlayerBattleAnimation, string> AnimString;

    private void OnEnable()
    {
        myAnimatorController = GetComponent<Animator>();
        AnimString = new Dictionary<PlayerBattleAnimation, string>() {
            { PlayerBattleAnimation.Idle, "Idle_Battle"},
            { PlayerBattleAnimation.IdleCrawl, "IdleCrawl_Battle"},
            { PlayerBattleAnimation.Jump, "Jump_Battle"},
            { PlayerBattleAnimation.Run, "Run_Battle"},
            { PlayerBattleAnimation.Crawl, "Crawl_Battle"},
            { PlayerBattleAnimation.Cling, "Cling_Battle"},
        };
    }
    public void PlayAnimation(PlayerBattleAnimation anim)
    {
        myAnimatorController.Play(AnimString[anim]);
    }
}
