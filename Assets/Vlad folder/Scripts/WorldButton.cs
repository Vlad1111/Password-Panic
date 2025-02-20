using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorldButton : MonoBehaviour
{
    public MeshRenderer mr;
    public UnityEvent onClick;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            OnMouseExit();
            return;
        }
        mr.enabled = true;
    }

    private void OnMouseExit()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        mr.enabled = false;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        onClick.Invoke();
        OnMouseExit();
    }
}
