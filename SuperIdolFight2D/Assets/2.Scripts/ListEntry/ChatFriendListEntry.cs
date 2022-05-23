using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;


public class ChatFriendListEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private Button SelfButton;
    [SerializeField] private Image Background;
    public void SetInf(string _name, UnityEngine.Events.UnityAction<ChatFriendListEntry> OnSelfClick)
    {
        NameText.SetText(_name);
        SelfButton.onClick.AddListener(() => OnSelfClick(this));
    }
    public void Active(bool _is)
    {
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, _is ? 1 : 0);
    }

}
