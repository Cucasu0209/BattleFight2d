using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [Header("Bottom")]
    [SerializeField] private TextMeshProUGUI ProgressText;
    [SerializeField] private Slider ProgressSlider;

    [Header("New Name")]
    [SerializeField] private GameObject NamePanel;
    [SerializeField] private TMP_InputField NameInput;
    [SerializeField] private TextMeshProUGUI DisplayNameInvalidText;
    [SerializeField] private Button NameButton;

    private int currentRound = 0;
    private float currentPercent = 0;

    private void OnEnable()
    {
        NamePanel.SetActive(false);
        StartLoading();
        PlayfabController_BF.OnUploadDisplayNameSuccess += LoadDataController_BF.Instance.OnLoadComplete;
        PlayfabController_BF.OnUploadDisplayNameFail += OnDisplayNameFail;
    }

    private void OnDisable()
    {
        PlayfabController_BF.OnUploadDisplayNameSuccess -= LoadDataController_BF.Instance.OnLoadComplete;
        PlayfabController_BF.OnUploadDisplayNameFail -= OnDisplayNameFail;
    }

    #region Functions
    private void OnDisplayNameFail()
    {
        DisplayNameInvalidText.SetText("Invalid Name");
    }
    private void StartLoading()
    {
        StartCoroutine(ILoading(LoadDataController_BF.Instance.OnLoadComplete));
    }
    private void ChangeValueUI(float _targetPercent, float _time = 0, Action OnComplete = null)
    {
        if (_time == 0)
        {
            currentPercent = _targetPercent;
            DisplayProgess();
            if (OnComplete != null)
                OnComplete();
        }
        else
        {
            StartCoroutine(IChangeValueUI(_targetPercent, _time, OnComplete));
        }

    }
    private void DisplayProgess()
    {
        ProgressSlider.value = (ProgressSlider.minValue + currentPercent / 100 * (ProgressSlider.maxValue - ProgressSlider.minValue));
        ProgressText.text = string.Format("Loading {0}/{1} ({2}%)",
            currentRound,
            LoadDataController_BF.Instance._tempLoadingMinTimeComponents.Length,
            Mathf.Ceil(currentPercent));
    }
    private void OnLoadingEnd(Action OnComplete)
    {
        if (string.IsNullOrEmpty(PlayfabController_BF.Instance.myDisplayName))
        {
            NamePanel.SetActive(true);
            NameButton.onClick.AddListener(() =>
            {
                string _name = NameInput.text;
                if (_name.Length <= 4 && _name.Contains("#")) return;
                string newID = IdentityGenerator.GenerateID();
                PlayfabController_BF.Instance.UploadDisplayName(_name, newID);
            });
        }
        else
        {

            if (OnComplete != null) OnComplete();
        }

    }
    #endregion

    #region Coroutines
    private IEnumerator ILoading(Action OnComplete)
    {
        IEnumerator RunRound;
        for (int i = 0; i < LoadDataController_BF.Instance._tempLoadingMinTimeComponents.Length; i++)
        {
            currentRound = i;
            float duration = LoadDataController_BF.Instance._tempLoadingMinTimeComponents[i];
            RunRound = IChangeValueUI(99, duration, null);
            StartCoroutine(RunRound);
            yield return new WaitForSeconds(duration * (1.2f));
            StopCoroutine(RunRound);
            while (LoadDataController_BF.Instance.GetDataRedyCount() <= i)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (i == LoadDataController_BF.Instance._tempLoadingMinTimeComponents.Length - 1)
            {
                currentRound = i + 1;
                ChangeValueUI(100);
            }
            else
            {
                ChangeValueUI(0);
            }
        }
        yield return new WaitForSeconds(0.5f);
        OnLoadingEnd(OnComplete);

    }
    private IEnumerator IChangeValueUI(float _targetPercent, float _time, Action OnComplete)
    {
        float deltaPercent = _targetPercent - currentPercent;
        float duration;
        while (currentPercent < Mathf.Min(_targetPercent, 100))
        {
            duration = Time.deltaTime;
            yield return new WaitForSeconds(duration);
            currentPercent += duration / _time * deltaPercent;
            if (currentPercent > Mathf.Min(_targetPercent, 100)) currentPercent = Mathf.Min(_targetPercent, 100);
            DisplayProgess();
        }
        if (OnComplete != null)
            OnComplete();
    }
    #endregion
}
