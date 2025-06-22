using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchNew = UnityEngine.InputSystem.EnhancedTouch.Touch;
using FingerNew = UnityEngine.InputSystem.EnhancedTouch.Finger;


public class TouchInputManager : MonoBehaviour
{
    public static TouchInputManager Instance;

    public delegate void TouchEvent(FingerNew finger);
    public event TouchEvent OnTapStart;
    public event TouchEvent OnTapEnd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameSessionManager.Instance.LogToFile($"Criou o touch input");
        }
        else
        {
            Destroy(gameObject);
        }
        EnhancedTouchSupport.Enable();
    }

    private void OnEnable()
    {
        GameSessionManager.Instance.LogToFile($"on enable touch input");
        TouchNew.onFingerDown += HandleFingerDown;
        TouchNew.onFingerUp += HandleFingerUp;
    }

    private void OnDisable()
    {
        GameSessionManager.Instance.LogToFile($"on disable touch input");
        TouchNew.onFingerDown -= HandleFingerDown;
        TouchNew.onFingerUp -= HandleFingerUp;
    }

    private void HandleFingerDown(FingerNew finger)
    {
        GameSessionManager.Instance.LogToFile($"finger down");
        OnTapStart?.Invoke(finger);
    }

    private void HandleFingerUp(FingerNew finger)
    {
        GameSessionManager.Instance.LogToFile($"finger up");
        OnTapEnd?.Invoke(finger);
    }
}
