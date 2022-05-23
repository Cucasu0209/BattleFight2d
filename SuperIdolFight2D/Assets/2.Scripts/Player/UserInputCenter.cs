using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class UserInputCenter : MonoBehaviour
{
    #region Singleton
    protected UserInputCenter() { }

    private static UserInputCenter instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static UserInputCenter Instance
    {
        get
        {
            if (instance != null) return instance;
            //if (ApplicationIsQuitting) return null;
            instance = FindObjectOfType<UserInputCenter>();
            if (instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return instance;
        }
    }
    private static UserInputCenter AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<UserInputCenter>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
    public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation = false) where T : MonoBehaviour
    {
        var component = FindObjectOfType<T>();
        if (component != null && isSingleton)
        {
            Debug.Log("Cannot add another " + typeof(T).Name + " to this Scene because you don't need more than one.");
#if UNITY_EDITOR
            UnityEditor.Selection.activeObject = component;
#endif
            return component;
        }

        component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(component.gameObject, "Created " + gameObjectName);
        if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = component.gameObject;
#endif
        return component;
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


    }
    #endregion

    #region Variables
    public static Action<float> OnJumpButtonClick;
    public static Action<float> OnMoveButtonClick;
    public static Action<bool> OnDownButtonClick;

    public static Action OnSKill1Click;


    #endregion

    #region Monobihaviour
    void Update()
    {
        //Move
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (OnMoveButtonClick != null) OnMoveButtonClick(-1);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (OnMoveButtonClick != null) OnMoveButtonClick(1);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            if (OnMoveButtonClick != null) OnMoveButtonClick(0);
        }

        //Jump
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            if (OnJumpButtonClick != null) OnJumpButtonClick(1);
        }

        //Sit down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (OnDownButtonClick != null) OnDownButtonClick(true);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            if (OnDownButtonClick != null) OnDownButtonClick(false);
        }

        //Skill1
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (OnSKill1Click != null) OnSKill1Click();
        }

    }
    #endregion

    #region OtherFunctions
    public void Init()
    {

    }
    #endregion

}
