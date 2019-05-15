using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehaviours : MonoBehaviour
{
    private static GameObject CurrentWindow = null;
    private GameObject OpenedWindow = null;

    [Header("References")]
    [SerializeField] private GameObject[] Windows;

    private void Awake()
    {
        SetActiveWindow(Windows[0]);
    }

    public void SetActiveWindow(GameObject newWindow)
    {
        CurrentWindow = newWindow;
    }

    public void ChangeActiveWindow(int window)
    {
        CurrentWindow?.SetActive(false);
        Windows[window]?.SetActive(true);
        CurrentWindow = Windows[window];
    }

    public void OpenWindow(GameObject newWindow)
    {
        newWindow?.SetActive(true);
        OpenedWindow = newWindow;
    }
}
