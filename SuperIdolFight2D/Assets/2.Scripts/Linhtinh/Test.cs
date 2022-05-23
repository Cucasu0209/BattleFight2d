using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    ///Singleton

    // Start is called before the first frame update
//    #region Singleton
//    protected PlayFabController() { }

//    private static PlayFabController f_instance;

//    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
//    public static PlayFabController Instance
//    {
//        get
//        {
//            if (f_instance != null) return f_instance;
//            //if (ApplicationIsQuitting) return null;
//            f_instance = FindObjectOfType<PlayFabController>();
//            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
//            return f_instance;
//        }
//    }
//    private static PlayFabController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<PlayFabController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
//    public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation = false) where T : MonoBehaviour
//    {
//        var component = FindObjectOfType<T>();
//        if (component != null && isSingleton)
//        {
//            Debug.Log("Cannot add another " + typeof(T).Name + " to this Scene because you don't need more than one.");
//#if UNITY_EDITOR
//            UnityEditor.Selection.activeObject = component;
//#endif
//            return component;
//        }

//        component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();

//#if UNITY_EDITOR
//        UnityEditor.Undo.RegisterCreatedObjectUndo(component.gameObject, "Created " + gameObjectName);
//        if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = component.gameObject;
//#endif
//        return component;
//    }
//    private void Awake()
//    {
//        if (f_instance != null && f_instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        f_instance = this;
//        DontDestroyOnLoad(gameObject);


//    }
//    #endregion
}
