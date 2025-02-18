using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerBehaviour : MonoBehaviour
{
    public EmailBehaviour emailBehaviour;

    public float scale = 0;
    private void OnEnable()
    {
        scale = 1;
        transform.localScale = new Vector3(1, 0, 1);
    }

    private void Update()
    {
        if(scale > 0)
        {
            transform.localScale = new Vector3(1, 1 - scale, 1);
            scale -= Time.deltaTime * 5;
            if(scale <= 0)
            {
                scale = 0;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
