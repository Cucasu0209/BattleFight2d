using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneController_BF : MonoBehaviour
{
    #region Singleton
    protected SceneController_BF() { }

    private static SceneController_BF f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static SceneController_BF Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<SceneController_BF>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    public static SceneController_BF AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<SceneController_BF>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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


    public enum Scene_BF
    {
        Login,
        Loading,
        MainMenu,
        Lobby,
        Room,
        BattleFightOnline
    }

    private string[] SceneName = new string[]
    {
        "Login",
        "Loading",
        "MainMenu",
        "Lobby",
        "Room",
        "BattleFightOnline",
    };

    public UnityEngine.Events.UnityAction<Scene, LoadSceneMode> OnSceneLoaded;


    public void LoadLocalScene(Scene_BF _scene, bool _effect = true)
    {
        if (SceneManager.GetActiveScene().name == SceneName[((int)_scene)])
        {
            SceneManager.LoadScene(SceneName[((int)_scene)]);
            return;
        }

        if (_effect)
        {
            EffectSceneOut(() =>
            {
                SceneManager.LoadScene(SceneName[((int)_scene)]);
                OnSceneLoaded = (a, b) => EffectSceneIn(() => { SceneManager.sceneLoaded -= OnSceneLoaded; });
                SceneManager.sceneLoaded += OnSceneLoaded;

            });
        }
        else
        {
            SceneManager.LoadScene(SceneName[((int)_scene)]);
        }

    }

    private void EffectSceneOut(Action OnEffectDone)
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        GameObject effectPrefab = (GameObject)Resources.Load("EffectLoadScene/BackToWhite");
        GameObject _newEffect = Instantiate(effectPrefab, mainCanvas.transform);
        _newEffect.transform.localScale = Vector3.zero;

        _newEffect.transform.DOScale(0, 0.1f).OnComplete(() =>
        {
            _newEffect.transform.DOScale(20, 0.5f).OnComplete(() =>
            {
                if (OnEffectDone != null) OnEffectDone();
            });
        });
    }

    private void EffectSceneIn(Action OnEffectDone)
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        GameObject effectPrefab = (GameObject)Resources.Load("EffectLoadScene/BackToWhite");
        GameObject _newEffect = Instantiate(effectPrefab, mainCanvas.transform);
        _newEffect.transform.localScale = Vector3.one * 20;
        _newEffect.transform.DOScale(Vector3.one * 20, 0.1f).OnComplete(() =>
        {
            _newEffect.transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                if (OnEffectDone != null) OnEffectDone();
            });
        });

    }
}
