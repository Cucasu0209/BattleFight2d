using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBattle : MonoBehaviour
{
    #region Variables
    public enum PlayerState
    {
        Stand,
        Jump,
        Cling,
        Crawl
    }
    [Header("Check Condition Point")]
    [SerializeField] private Transform PointToCheckGround1;
    [SerializeField] private Transform PointToCheckGround2;
    [SerializeField] private Transform PointToCheckForehead;
    [SerializeField] private Transform PointToCheckChin;
    [SerializeField] private Transform PointToCheckHeadVertex;
    [SerializeField] private Transform PointToCheckFront1;
    [SerializeField] private Transform PointToCheckFront2;
    [SerializeField] private Transform PointToCheckKnee;



    private Rigidbody2D MyBody;
    private List<Collider2D> myColliders;
    [SerializeField] private Collider2D MyMainCollider;
    [SerializeField] private SpriteRenderer MySpriteRenderer;
    private PlayerAnimationBattle MyAnimatorController;
    private SkillParameterBattle SkillParam;


    public PlayerState CurrentState { get; private set; }
    private Dictionary<PlayerState, bool> ComboState;

    private List<PlayerBattlePersonalSkill> playerSkills;
    //Max value
    private float currentSpeed;
    public float maxRunSpeed { get; private set; }
    public float maxCrawlSpeed { get; private set; }
    public float maxJumpForce { get; private set; }
    public int maxCrawlJumpForce { get; private set; }
    public int maxJumpStepCapacity { get; private set; }

    //Temp
    private int _tempJumpStepCapacity = 2;
    private float _tempScaleRunSpeed = 0;

    //Skill


    #endregion Variables

    #region Monobihaviour

    private void OnEnable()
    {
        SetUpComponent();
        InitializationVariables();
        UserInputCenter.Instance.Init();
        UserInputCenter.OnMoveButtonClick += SetRunState;
        UserInputCenter.OnJumpButtonClick += Jump;
        UserInputCenter.OnDownButtonClick += SetCrawlState;
        UserInputCenter.OnSKill1Click += () => PlaySkill(0);
    }
    private void OnDisable()
    {
        UserInputCenter.OnMoveButtonClick -= SetRunState;
        UserInputCenter.OnJumpButtonClick -= Jump;
        UserInputCenter.OnDownButtonClick -= SetCrawlState;
    }
    private void Update()
    {
        FindFinalState();
        MovePlayer();
        PlayAnimation();
    }
    #endregion Monobihaviour

    #region Setup
    private void SetUpComponent()
    {
        MyBody = GetComponent<Rigidbody2D>();
        myColliders = new List<Collider2D>(GetComponentsInChildren<Collider2D>());
        MyAnimatorController = GetComponent<PlayerAnimationBattle>();
        SkillParam = new SkillParameterBattle()
        {
            AnimatorController = MyAnimatorController,
            PlayerBody = MyBody,
            PlayerMainCollider = MyMainCollider,
            PlayerObj = gameObject,
            MyRenderer = MySpriteRenderer
        };
    }
    private void InitializationVariables()
    {
        maxRunSpeed = 3;
        maxCrawlSpeed = 1.5f;
        maxJumpForce = 5;
        maxCrawlJumpForce = 8;
        maxJumpStepCapacity = 2;

        CurrentState = PlayerState.Stand;
        ComboState = new Dictionary<PlayerState, bool>()
        {
            { PlayerState.Stand, true },
            { PlayerState.Jump, false },
            { PlayerState.Cling, false },
            { PlayerState.Crawl, false },
        };

        playerSkills = new List<PlayerBattlePersonalSkill>()
        {
            new SkillDashBattle()
        };

    }
    #endregion Setup

    #region Movement
    private void SetRunState(float _speedScale)
    {
        ComboState[PlayerState.Stand] = true;
        _tempScaleRunSpeed = _speedScale;
    }
    private void Jump(float _forceScale)
    {
        if (_tempJumpStepCapacity <= 0 || CheckHeadStuck()) return;

        if (ComboState[PlayerState.Crawl]) MyBody.velocity = new Vector2(MyBody.velocity.x, _forceScale * maxCrawlJumpForce);
        else MyBody.velocity = new Vector2(MyBody.velocity.x, _forceScale * maxJumpForce);
        _tempJumpStepCapacity--;
    }
    private void SetCrawlState(bool _crawlCondition)
    {
        if (_crawlCondition)
        {
            ComboState[PlayerState.Crawl] = true;
        }
        else
        {
            ComboState[PlayerState.Crawl] = false;
        }
    }
    private void SetClingState(bool _clingCondition)
    {
        if (_clingCondition)
        {
            MyBody.velocity = Vector2.zero;
            MyBody.gravityScale = 0;
        }
        else
        {
            MyBody.gravityScale = 1;
        }
    }
    private void MovePlayer()
    {
        SetClingState(CurrentState == PlayerState.Cling);
        if (CurrentState != PlayerState.Crawl)
        {
            currentSpeed = maxRunSpeed * _tempScaleRunSpeed;
        }
        else
        {
            currentSpeed = maxCrawlSpeed * _tempScaleRunSpeed;
        }
        MyBody.velocity = new Vector2(currentSpeed, MyBody.velocity.y);
        FlipPlayer(MyBody.velocity.x);


    }
    private void FlipPlayer(float velocity)
    {
        if (velocity > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (velocity < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    private void PlaySkill(int skillIndex)
    {
        playerSkills[skillIndex].PlaySkill(SkillParam);
    }

    #endregion Movement

    #region Raycast To Check State
    private bool CheckRayGround()
    {
        return CheckRay(transform.position, PointToCheckGround1.position) || CheckRay(transform.position, PointToCheckGround2.position);

    }
    private bool CheckRayHeadVertex()
    {
        return CheckRay(transform.position, PointToCheckHeadVertex.position);
    }
    private bool CheckRayForehead()
    {
        Vector2 startPoint = new Vector2(transform.position.x, PointToCheckForehead.position.y);
        return CheckRay(startPoint, PointToCheckForehead.position);
    }
    private bool CheckRayChin()
    {
        Vector2 startPoint = new Vector2(transform.position.x, PointToCheckChin.position.y);
        return CheckRay(startPoint, PointToCheckChin.position);
    }
    private bool CheckRayKnee()
    {
        Vector2 startPoint = new Vector2(transform.position.x, PointToCheckKnee.position.y);
        return CheckRay(startPoint, PointToCheckKnee.position);
    }
    private bool CheckRayFront()
    {
        return CheckRay(PointToCheckFront1.position, PointToCheckFront2.position);
    }
    private bool CheckRay(Vector2 _startPoint, Vector2 _endPoint)
    {
        Vector2 _direction = (_endPoint - _startPoint).normalized;
        float _distance = (_endPoint - _startPoint).magnitude;

        RaycastHit2D[] hits = Physics2D.RaycastAll(_startPoint, _direction, _distance);
        foreach (RaycastHit2D hit in hits)
        {
            if (myColliders.Contains(hit.collider)) continue;
            if (!hit.collider.isTrigger)
            {
                Debug.DrawLine(_startPoint, _endPoint, Color.red);
                return true;
            }
        }
        Debug.DrawLine(_startPoint, _endPoint, Color.white);
        return false;
    }
    #endregion

    #region Condition Movement

    private bool CheckHeadStuck()
    {
        return CheckRayHeadVertex();
    }
    private bool CheckOnGround()
    {
        return CheckRayGround();
    }
    private bool CheckAutoCrawl()
    {
        if (CheckRayForehead() && CheckRayFront()) { Debug.Log("haha"); return true; }
        return false;
    }
    private bool CheckClingState()
    {
        if (!CheckRayChin() && CheckRayFront() && CheckRayKnee()) { Debug.Log("haha1"); return true; }
        return false;
    }
    #endregion

    #region State And Animation
    private void FindFinalState()
    {
        //update
        if (CheckOnGround()) ComboState[PlayerState.Jump] = false;
        else ComboState[PlayerState.Jump] = true;
        if (CheckClingState() && MyBody.velocity.y <= 0) ComboState[PlayerState.Cling] = true;
        else ComboState[PlayerState.Cling] = false;

        //capacity
        if (CheckOnGround() || ComboState[PlayerState.Cling]) _tempJumpStepCapacity = maxJumpStepCapacity;

        //final
        if (ComboState[PlayerState.Cling]) CurrentState = PlayerState.Cling;
        else if (ComboState[PlayerState.Jump]) CurrentState = PlayerState.Jump;
        else if (ComboState[PlayerState.Crawl]) CurrentState = PlayerState.Crawl;
        else if (ComboState[PlayerState.Stand]) CurrentState = PlayerState.Stand;
    }

    private void PlayAnimation()
    {
        switch (CurrentState)
        {
            case PlayerState.Stand:
                if (MyBody.velocity.magnitude < 0.001) MyAnimatorController.PlayAnimation(PlayerBattleAnimation.Idle);
                else MyAnimatorController.PlayAnimation(PlayerBattleAnimation.Run);
                break;
            case PlayerState.Crawl:
                if (MyBody.velocity.magnitude < 0.001) MyAnimatorController.PlayAnimation(PlayerBattleAnimation.IdleCrawl);
                else MyAnimatorController.PlayAnimation(PlayerBattleAnimation.Crawl);
                break;
            case PlayerState.Jump:
                MyAnimatorController.PlayAnimation(PlayerBattleAnimation.Jump);
                break;
            case PlayerState.Cling:
                MyAnimatorController.PlayAnimation(PlayerBattleAnimation.Cling);
                break;

        }
    }

    #endregion


}
