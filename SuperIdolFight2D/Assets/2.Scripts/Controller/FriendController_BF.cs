using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

public class FriendController_BF : MonoBehaviour
{
    public enum FriendState
    {
        Online, Offline
    }
    #region Singleton
    protected FriendController_BF() { }

    private static FriendController_BF f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static FriendController_BF Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<FriendController_BF>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    public static FriendController_BF AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<FriendController_BF>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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

    #region Vaiables
    public Dictionary<string, FriendState> _tempFriendList = new Dictionary<string, FriendState>()
    {
        {"Trung#123123",FriendState.Offline }, 
        {"Mizoho#1das23123",FriendState.Offline }, 
        {"PhamThoai#123rr123",FriendState.Offline },
        {"TheThai#123rr123",FriendState.Offline },
        {"Hung#123rr123",FriendState.Offline },
        
    };

    public static Action OnFriendListUpdate;
    #endregion

}
