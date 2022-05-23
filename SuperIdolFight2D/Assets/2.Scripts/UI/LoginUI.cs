using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public enum LocalScene
    {
        Login,
        Register,
        NULL
    }

    #region Variables
    [HideInInspector] public static LoginUI Instance;
    [Header("Top")]
    [SerializeField] private Button BackButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button SoundVolumeButton;
    [SerializeField] private GameObject[] SoundVolumeDisplay;
    [Header("Login")]
    [SerializeField] private GameObject LoginPanel;
    [SerializeField] private TMP_InputField UserNameLoginInput;
    [SerializeField] private TMP_InputField PasswordLoginInput;
    [SerializeField] private TextMeshProUGUI LoginLogText;
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button GoRegisterButton;

    [Header("Register")]
    [SerializeField] private GameObject RegisterPanel;
    [SerializeField] private TMP_InputField UserNameRegisterInput;
    [SerializeField] private TMP_InputField PasswordRegisterInput;
    [SerializeField] private TMP_InputField PasswordRegister2rdInput;
    [SerializeField] private TextMeshProUGUI RegisterLogText;
    [SerializeField] private Button RegisterButton;


    private LocalScene localScene = LocalScene.NULL;


    private string LOG_PASSWORD_INVALID = "Password is too short (at least 6 character).";
    private string LOG_PASSWORD_2RD_NOT_CORRECT = "Two password is not the same.";
    private string LOG_LOGIN_FAIL = "Username is not exist or Wrong password";
    private string LOG_REGISTE_FAIL = "Username is already exist.";
    private string LOG_REGISTER_SUCCESS = "Register success.";

    #endregion

    #region Monibehaviour
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ChangeLocalScene(LocalScene.Login);
        RegisterUIEvent();

        PlayfabController_BF.OnLoginSuccess += OnLoginSuccess;
        PlayfabController_BF.OnLoginFail += OnLoginFail;
        PlayfabController_BF.OnRegisterSuccess += OnRegisterSuccess;
        PlayfabController_BF.OnRegisterFail += OnLoginFail;

    }
    private void OnDisable()
    {
        PlayfabController_BF.OnLoginSuccess -= OnLoginSuccess;
        PlayfabController_BF.OnLoginFail -= OnLoginFail;
        PlayfabController_BF.OnRegisterSuccess -= OnRegisterSuccess;
        PlayfabController_BF.OnRegisterFail -= OnLoginFail;

    }
    #endregion

    #region Setup
    private void RegisterUIEvent()
    {
        LoginButton.onClick.AddListener(OnLoginClick);
        GoRegisterButton.onClick.AddListener(() => { ChangeLocalScene(LocalScene.Register); });
        RegisterButton.onClick.AddListener(OnRegisterClick);
        BackButton.onClick.AddListener(OnBackClick);
        QuitButton.onClick.AddListener(OnQuitClick);
        SoundVolumeButton.onClick.AddListener(OnSoundVolumeClick);
    }
    private void OnRegisterClick()
    {
        if (LocalCheckRegister())
        {
            string _username = UserNameRegisterInput.text, _password = PasswordRegisterInput.text;
            PlayfabController_BF.Instance.Register(_username, _password);
            Debug.Log("[LoginUI]: " + string.Format("Register Usename: {0}, Password: {1}", UserNameRegisterInput.text, PasswordRegisterInput.text));
        }

    }
    private void OnLoginClick()
    {
        if (LocalCheckLogin())
        {
            string _username = UserNameLoginInput.text, _password = PasswordLoginInput.text;
            PlayfabController_BF.Instance.Login(_username, _password);
            Debug.Log("[LoginUI]: " + string.Format("Login Usename: {0}, Password: {1}", UserNameLoginInput.text, PasswordLoginInput.text));
        }




    }

    private void OnBackClick()
    {
        ChangeLocalScene(LocalScene.Login);
        Debug.Log("[LoginUI]: " + "BackClick");
    }
    private void OnQuitClick()
    {
        Debug.Log("[LoginUI]: " + "QuitClick");
    }
    private void OnSoundVolumeClick()
    {
        for (int i = 0; i < SoundVolumeDisplay.Length; i++)
        {
            if (SoundVolumeDisplay[i].active)
            {
                SoundVolumeDisplay[i].SetActive(false);
                SoundVolumeDisplay[(i + 1) % SoundVolumeDisplay.Length].SetActive(true);
                break;
            }
        }
        Debug.Log("[LoginUI]: " + "VolumeChanege");
    }

    private void ChangeLocalScene(LocalScene _scene)
    {
        if (localScene == _scene) return;
        ClearAllInputField();
        LoginPanel.SetActive(_scene == LocalScene.Login);
        RegisterPanel.SetActive(_scene == LocalScene.Register);
        localScene = _scene;
    }
    private void ClearAllInputField()
    {
        UserNameLoginInput.text = "";
        PasswordLoginInput.text = "";
        LoginLogText.text = "";

        UserNameRegisterInput.text = "";
        PasswordRegisterInput.text = "";
        PasswordRegister2rdInput.text = "";
        RegisterLogText.text = "";
    }
    #endregion

    #region OtherFunctions
    private bool LocalCheckLogin()
    {
        if (PasswordLoginInput.text.Length < 6)
        {
            LoginLogText.text = LOG_PASSWORD_INVALID;
            return false;
        }
        return true;
    }

    private bool LocalCheckRegister()
    {
        if (PasswordRegister2rdInput.text != PasswordRegisterInput.text)
        {
            RegisterLogText.text = LOG_PASSWORD_2RD_NOT_CORRECT;
            return false;
        }
        if (PasswordRegister2rdInput.text.Length < 6)
        {
            RegisterLogText.text = LOG_PASSWORD_INVALID;
            return false;
        }
        return true;
    }

    #endregion

    #region API Callbacks
    private void OnLoginSuccess()
    {
        LoadDataController_BF.Instance.ShowLoading(LoadDataController_BF.LoadingCause.StartToMainMenu);
    }
    private void OnLoginFail()
    {
        LoginLogText.text = LOG_LOGIN_FAIL;
    }
    private void OnRegisterSuccess()
    {
        RegisterLogText.text = LOG_REGISTER_SUCCESS;
    }
    private void OnRegisterFail()
    {
        RegisterLogText.text = LOG_REGISTE_FAIL;
    }
    #endregion

}
