using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityGenerator
{
    public static string GenerateID()
    {
        long i = 1;
        int numOfTrail = 4;
        int numOfHead = 3;

        foreach (byte b in Guid.NewGuid().ToByteArray())
        {
            i *= ((int)b + 1);
        }
        string uniqueKeys = "";
        for (int j = 0; j < numOfHead; j++)
        {
            uniqueKeys += GetRandomKey();
        }

        uniqueKeys += String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

        for (int j = 0; j < numOfTrail; j++)
        {
            uniqueKeys += GetRandomKey();
        }
        return uniqueKeys;
    }

    private static string GetRandomKey()
    {
        string _keys = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        int rand = UnityEngine.Random.Range(0, _keys.Length);

        return _keys[rand].ToString();
    }
}
