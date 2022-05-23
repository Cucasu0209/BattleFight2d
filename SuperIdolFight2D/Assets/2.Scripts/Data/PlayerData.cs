using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    #region Keystring
    private static readonly string KEY_UNIQUE_ID = "UniqueID";
    private static readonly string KEY_DISPLAY_NAME = "DisplayName";
    #endregion

    #region Temp Variables

    #endregion

    #region Default Value
    private static readonly string DefaultUniqueKey = "1234";
    private static readonly string DefaultDisplaName = "Trunks";
    #endregion

    #region Get Keys
    public static string GetKeyUniqueID()
    {
        return KEY_UNIQUE_ID;
    }
    public static string GetKeyDisplayName()
    {
        return KEY_DISPLAY_NAME;
    }
    #endregion

    #region Save
    public static void SaveUniqueID(string _uniqueID)
    {
        PlayerPrefs.SetString(GetKeyUniqueID(), _uniqueID);
    }
    public static void SaveDisplayName(string _displayName)
    {
        PlayerPrefs.SetString(GetKeyDisplayName(), _displayName);
    }
    #endregion

    #region Load
    public static string LoadUniqueID()
    {
        string result;
        if (PlayerPrefs.HasKey(GetKeyUniqueID()) == false) result = DefaultUniqueKey;
        else result = PlayerPrefs.GetString(GetKeyUniqueID());
        if (result == "") result = DefaultUniqueKey;
        return result;
    }
    public static string LoadDisplayName()
    {
        string result;
        if (PlayerPrefs.HasKey(GetKeyDisplayName()) == false) result = DefaultDisplaName;
        else result = PlayerPrefs.GetString(GetKeyDisplayName());
        if (result == "") result = DefaultDisplaName;
        return result;
    }
    #endregion
}
