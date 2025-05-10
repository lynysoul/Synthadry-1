using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour, IPauseHandler
{
    public static PauseManager Instance { get; private set; }
    public static event Action OnPauseManagerReady;

    void Start()
    {
        gameObject.SetActive(false); // ������ ���� �����
        Instance = this;
        Instance.IsPaused = false;
        OnPauseManagerReady?.Invoke(); // ���������� ����, ��� �������� �����
    }

    private readonly List<IPauseHandler> _handlers = new();

    public bool IsPaused { get; private set; }

    public void Register(IPauseHandler handler)
    {
        Debug.Log(1);
        Debug.Log(handler);
        _handlers.Add(handler);
    }

    public void UnRegister(IPauseHandler handler)
    {
        _handlers.Remove(handler);
    }

    public void SetPaused(bool isPaused)
    {
        if (isPaused) // ������������ ������ � ������� ���� �����
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowWindow();
        }
        else // ����������� ������ � ������ ���� �����
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HideWindow();
        }
        IsPaused = isPaused;
        foreach (var handler in _handlers)
        {
            Debug.Log(2);
            Debug.Log(handler);
            handler.SetPaused(isPaused); 
        }
    }

    private void ShowWindow()
    {
        gameObject.SetActive(true);
    }

    private void HideWindow()
    {
        gameObject.SetActive(false);
    }
}
