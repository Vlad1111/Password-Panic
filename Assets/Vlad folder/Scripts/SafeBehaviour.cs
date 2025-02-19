using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBehaviour : MonoBehaviour
{
    public int codeLenght = 5;
    public Transform safeRoller;
    public Transform correctIndicator;
    public Transform incorectIndicator;
    public List<int> code = new List<int> ();

    private float timmer = 0.01f;
    private float targetRotation = 0;

    private void Start()
    {
        for (int i = 0; i < codeLenght; i++)
        {
            var rand = Random.Range(-1, 2);
            if (rand <= 0) rand--;

            code.Add(rand);
        }
    }
    public void StartCrackingSafe()
    {
        gameObject.SetActive(true);
    }

    private int currentCodePart = 0;
    public void MoveToDirection(int direction)
    {
        if(code[currentCodePart] == direction)
        {
            correctIndicator.gameObject.SetActive(true);
            incorectIndicator.gameObject.SetActive(false);
            targetRotation -= direction * (12 + Random.value * 6);
            currentCodePart++;
            if(currentCodePart >= code.Count)
            {
                GameBehaviour.SetGlobalValue(GameVariableKeys.SpareKeyFound.ToString(), 1);
                ScreenBahaviour.Instance.SetCameraLocation("Desk");
                gameObject.SetActive(false);
            }
        }
        else
        {
            correctIndicator.gameObject.SetActive(false);
            incorectIndicator.gameObject.SetActive(true);
            currentCodePart = 0;
            targetRotation = 0;
        }
        timmer = 1;
    }

    private void Update()
    {
        if(timmer > 0)
        {
            timmer -= Time.deltaTime;
            if(timmer <= 0)
            {
                correctIndicator.gameObject.SetActive(false);
                incorectIndicator.gameObject.SetActive(false);
            }
        }
        safeRoller.localRotation = Quaternion.Lerp(safeRoller.localRotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * 10);
    }
}
