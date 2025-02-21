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
            DialogueBehaviour.Instance.ShowDialogueFromFile("key snap");
            GameBehaviour.SetGlobalValue(GameVariableKeys.KeySnapped.ToString(), 1);
        }
        else if(!hasKeyFound)
        {
            DialogueBehaviour.Instance.ShowLine("I need to finde the spare key in the safe");
        }
        else
        {
            GameBehaviour.SetGlobalValue(GameVariableKeys.LaserArmed.ToString(), 1);
        }
    }
}
