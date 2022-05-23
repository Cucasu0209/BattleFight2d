using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabController_BF : MonoBehaviour
{
    public enum PlayfabDataType
    {
        DisplayName,
    }
    #region Singleton
    protected PlayfabController_BF() { }

    private static PlayfabController_BF f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static PlayfabController_BF Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<PlayfabController_BF>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    public static PlayfabController_BF AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<PlayfabController_BF>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    public Dictionary<PlayfabDataType, bool> ReadyData = new Dictionary<PlayfabDataType, bool>()
    {
        {PlayfabDataType.DisplayName, false},
    };

    private string myUserName = "";
    public string myDisplayName = "";

    public static Action OnLoginSuccess;
    public static Action OnLoginFail;
    public static Action OnRegisterSuccess;
    public static Action OnRegisterFail;
    public static Action OnGetDisplayNameSuccess;
    public static Action OnGetDisplayNameFail;
    public static Action OnUploadDisplayNameSuccess;
    public static Action OnUploadDisplayNameFail;
    #endregion

    #region Monobehaviour
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "2049A";
        }
    }
    private void OnDisable()
    {

    }
    #endregion

    #region Call API
    public void Login(string _username, string _password)
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            Username = _username,
            Password = _password
        },
        (result) =>
        {
            myUserName = _username;
            if (OnLoginSuccess != null) OnLoginSuccess();
            Debug.Log("[PlayfabController_BF]: Login Success");
        },
        (error) =>
        {
            if (OnLoginFail != null) OnLoginFail();
            Debug.Log("[PlayfabController_BF]: Login Fail. Msg: " + error);
        });
    }
    public void Register(string _username, string _password)
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            Username = _username,
            Password = _password,
            RequireBothUsernameAndEmail = false
        },
        (result) =>
        {
            if (OnRegisterSuccess != null) OnRegisterSuccess();
            Debug.Log("[PlayfabController_BF]: Register Success");
        },
        (error) =>
        {
            if (OnRegisterFail != null) OnRegisterFail();
            Debug.Log("[PlayfabController_BF]: Register Fail. Msg: " + error);
        }); ;
    }
    public void LoadDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
        {
            Username = myUserName
        },
        (result) =>
        {
            ReadyData[PlayfabDataType.DisplayName] = true;
            myDisplayName = result.AccountInfo.TitleInfo.DisplayName;
            if (!string.IsNullOrEmpty(myDisplayName) && myDisplayName.Split("#").Length >= 2)
            {
                PlayerData.SaveDisplayName(myDisplayName.Split("#")[0]);
                PlayerData.SaveUniqueID(myDisplayName.Split("#")[1]);
            }
            if (OnGetDisplayNameSuccess != null) OnGetDisplayNameSuccess();
            Debug.Log("[PlayfabController_BF]: Load Display name Success");
        },
        (error) =>
        {
            if (OnGetDisplayNameFail != null) OnGetDisplayNameFail();
            Debug.Log("[PlayfabController_BF]: Load Display name Fail. Msg: " + error);
        });
    }
    public void UploadDisplayName(string _newUserName, string _uniqueID)
    {
        Debug.Log("HHAA "+_newUserName + "#" + _uniqueID);
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = _newUserName + "#" + _uniqueID
        },
        (result) =>
        {
            myDisplayName = _newUserName + "#" + _uniqueID;
            PlayerData.SaveDisplayName(_newUserName);
            PlayerData.SaveDisplayName(_uniqueID);
            if (OnUploadDisplayNameSuccess != null) OnUploadDisplayNameSuccess();
            Debug.Log("[PlayfabController_BF]: Upload Display name Success");
        },
        (error) =>
        {
            if (OnUploadDisplayNameFail != null) OnUploadDisplayNameFail();
            Debug.Log("[PlayfabController_BF]:  Upload Display name Fail. Msg: " + error);
        });
    }
    #endregion


}
