using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour, IPauseHandler
{
    public static PauseManager Instance { get; private set; }
    public static event Action OnPauseManagerReady;

    void Start()
    {
        gameObject.SetActive(false); // Скрыть окно паузы
        Instance = this;
        Instance.IsPaused = false;
        OnPauseManagerReady?.Invoke(); // Уведомляем всех, что менеджер готов
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
        if (isPaused) // Разблокирует курсор и покажет окно паузы
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowWindow();
        }
        else // Заблокирует курсор и скроет окно паузы
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
