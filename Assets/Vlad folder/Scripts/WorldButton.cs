using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        mr.enabled = true;
    }

    private void OnMouseExit()
    {
        mr.enabled = false;
    }

    private void OnMouseDown()
    {
        onClick.Invoke();
        OnMouseExit();
    }
}
