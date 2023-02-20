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

    //[HideInInspector]
    //public UnityEvent _onGameEnded;

    // Start is called before the first frame update
    void Awake()
    {
        _randomSpawner.Initialize(_positions, _colors, _winPositions);
        _cupBoardsMovement.Initialize(_inputController.OnNodeSelected, _positions);
        //_uiController.Initialize(_onGameEnded);
        //_inputController.Initialize(_cupBoardsMovement.NodeStatus);
    }

    
    private void Start()
    {
        _cupBoardsMovement.OnCupBoardMoved.AddListener(CheckCupBoards);
    }

    private void OnDestroy()
    {
        _cupBoardsMovement.OnCupBoardMoved.RemoveListener(CheckCupBoards);
    }

    private void CheckCupBoards()
    {
        var isGameEnded = true;
        var count = 0;
        while (count < 3)
        {
            if (_winPositions[count].GetComponent<Renderer>().material.color !=
                _positions[count].GetComponent<Renderer>().material.color)
            {
                isGameEnded = false;
                break;
            }

            if (_winPositions[count + 3].GetComponent<Renderer>().material.color !=
                _positions[count + 6].GetComponent<Renderer>().material.color)
            {
                isGameEnded = false;
                break;
            }

            count++;
        }
        if (isGameEnded)
        {
            Debug.Log("GameEnded");
        }
    }

    /*
    public void Restart()
    {
        _randomSpawner.Initialize(_positions, _colors, _winPositions);
        _cupBoardsMovement.Initialize(_inputController.OnNodeSelected, _positions);
        _uiController.Initialize(_onGameEnded);
    }
    */
}