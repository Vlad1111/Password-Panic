using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LounchButtonBehaviour : MonoBehaviour
{
    public void UseKey()
    {
        var hasKeySnapped = GameBehaviour.GetGlobalValue(GameVariableKeys.KeySnapped.ToString()) > 0.5f;
        var hasKeyFound = GameBehaviour.GetGlobalValue(GameVariableKeys.SpareKeyFound.ToString()) > 0.5f;
        if(!hasKeySnapped)
        {
            GameBehaviour.SetGlobalValue(GameVariableKeys.KeySnapped.ToString(), 1);
        }
        else if(!hasKeyFound)
        {

        }
        else
        {
            GameBehaviour.SetGlobalValue(GameVariableKeys.LaserArmed.ToString(), 1);
        }
    }
}
