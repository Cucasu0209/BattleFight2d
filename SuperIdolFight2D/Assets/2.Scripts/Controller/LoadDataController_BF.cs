using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

public class LoadDataController_BF : MonoBehaviour
{
    public enum LoadingCause
    {
        StartToMainMenu,
        StartToBattleOnlineGame,

    }

    #region Singleton
    protected LoadDataController_BF() { }

    private static LoadDataController_BF f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static LoadDataController_BF Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<LoadDataController_BF>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    public static LoadDataController_BF AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<LoadDataController_BF>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    public void Awake()
    {
        if (f_instance != null && f_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        f_instance = this;
        DontDestroyOnLoad(gameObject);


    }
    public void Init()
    {
        Debug.Log("Init Singleton " + this.name);
    }
    #endregion

    #region Variables
    public float[] _tempLoadingMinTimeComponents = new float[] { 2, 2, 2, 2, 2, 2, 2 };
    public Action OnLoadComplete;
    #endregion

    #region Loading
    public void ShowLoading(LoadingCause _cause)
    {
        switch (_cause)
        {
            case LoadingCause.StartToMainMenu:
                LoadDataStartMainMenu();
                OnLoadComplete = LoadDataStartMainMenuComplete;
                break;
            default:
                break;

        }
        SceneController_BF.Instance.LoadLocalScene(SceneController_BF.Scene_BF.Loading);
    }

    public void LoadDataStartMainMenu()
    {
        _tempLoadingMinTimeComponents = new float[] { 0.5f, };
        PlayfabController_BF.Instance.LoadDisplayName();
    }

    public int GetDataRedyCount()
    {
        int numberOfDataReady = 0;
        if (PlayfabController_BF.Instance.ReadyData[PlayfabController_BF.PlayfabDataType.DisplayName]) numberOfDataReady++;
        return numberOfDataReady;
    }

    public void LoadDataStartMainMenuComplete()
    {
        SceneController_BF.Instance.LoadLocalScene(SceneController_BF.Scene_BF.MainMenu);
    }

    #endregion
}
