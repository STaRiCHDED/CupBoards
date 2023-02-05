using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [SerializeField] 
    private Canvas _canvas;

    private UnityEvent _onGameEnded;
    public void Initialize(UnityEvent onGameEnded)
    {
        _canvas.enabled = false;
        _onGameEnded = onGameEnded;
    }

    private void Start()
    {
        _onGameEnded.AddListener(ShowMenu);
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        _onGameEnded.RemoveListener(ShowMenu);
    }

    private void ShowMenu()
    {
        _canvas.enabled = true;
    }
}
