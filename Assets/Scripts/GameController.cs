using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField] 
    private Node[] _positions;
    [SerializeField] 
    private Color[] _colors;
    [SerializeField] 
    private GameObject[] _winPositions;
    [SerializeField] 
    private InputController _inputController;
    [SerializeField] 
    private RandomSpawner _randomSpawner;
    [SerializeField] 
    private CupBoardsMovement _cupBoardsMovement;
    [SerializeField] 
    private UIController _uiController;

    [HideInInspector]
    public UnityEvent _onGameEnded;

    // Start is called before the first frame update
    void Awake()
    {
        //_onGameEnded = new UnityEvent();
        _randomSpawner.Initialize(_positions, _colors, _winPositions);
        _cupBoardsMovement.Initialize(_inputController.OnNodeSelected, _positions);
        _uiController.Initialize(_onGameEnded);
    }

    private void Start()
    {
        _cupBoardsMovement.OnCupBoardMoved.AddListener(CheckCupBoards);
    }

    void Update()
    {
        
    }

    private void OnDestroy()
    {
        _cupBoardsMovement.OnCupBoardMoved.RemoveListener(CheckCupBoards);
    }

    private void CheckCupBoards()
    {
        var isGameEnded = true;
        var count = 0;
        var i = 0;
        while (count < 3)
        {
            if (_winPositions[i].GetComponent<Renderer>().material.color !=
                _positions[i].GetComponent<Renderer>().material.color)
            {
                isGameEnded = false;
                break;
            }

            if (_winPositions[i + 3].GetComponent<Renderer>().material.color ==
                _positions[i + 6].GetComponent<Renderer>().material.color)
            {
                isGameEnded = false;
                break;
            }
        }
        if (isGameEnded)
        {
            _onGameEnded?.Invoke();
        }
    }

    public void Restart()
    {
        _randomSpawner.Initialize(_positions, _colors, _winPositions);
        _cupBoardsMovement.Initialize(_inputController.OnNodeSelected, _positions);
        _uiController.Initialize(_onGameEnded);
    }
}