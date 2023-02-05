using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class CupBoardsMovement : MonoBehaviour
{
    private enum NodeMoving
    {
        Nothing = 0,
        Start = 1,
        Moving = 2
    }

    [HideInInspector] 
    public UnityEvent OnCupBoardMoved;
    [SerializeField]
    private GameObject _moveableCupBoard;
    [SerializeField]
    private float _travelTime;
    [UsedImplicitly]
    private UnityEvent<Node> _onNodeSelected;
    private Node[] _positions;
    private Node _startPosition;
    private Node _endPosition;
    private NodeMoving _nodeMoving;
    private List<Node> _path;
    private GameObject _movableActor;
    private float _currentTime;
    private int _currentPosition;
    public void Initialize(UnityEvent<Node> onNodeSelected,Node[] positions)
    {
        _onNodeSelected = onNodeSelected;
        _positions = positions;
        _path = new List<Node>();
    }
    void Start()
    {
        _onNodeSelected.AddListener(ManageNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (_nodeMoving == NodeMoving.Moving)
        {
            _currentTime += Time.deltaTime;
            var result = _currentTime/ _travelTime ;
            _movableActor.transform.position = Vector3.Lerp(_path[_currentPosition].transform.position, _path[GetNextIndex()].transform.position, result);

            if (_currentTime >= _travelTime) 
            {
                _currentTime = 0;
                _currentPosition = GetNextIndex();
            }
            if (_currentPosition == _path.Count - 1)
            {
                _path.Clear();
                _nodeMoving = NodeMoving.Nothing;
                _endPosition.ResetNode(true,_movableActor.GetComponent<Renderer>().material.color);
                Destroy(_movableActor);
                OnCupBoardMoved?.Invoke();
            }
        }
    }
    int GetNextIndex()
    {
        return _currentPosition+1;
    }
    void OnDestroy()
    {
        _onNodeSelected.RemoveListener(ManageNode);
    }

    private void ManageNode(Node node)
    {
        if (_nodeMoving == NodeMoving.Nothing && node.IsOccupied)
        {
            FindPossiblePositions(node);
            _startPosition = node;
            _nodeMoving = NodeMoving.Start;
            return;
        }

        if (_nodeMoving == NodeMoving.Start && !node.IsOccupied)
        {
            CreatePath(node);
            _endPosition = node;
            _nodeMoving = NodeMoving.Moving;
            _movableActor = Instantiate(_moveableCupBoard);
            _movableActor.transform.position = _startPosition.transform.position;
            _movableActor.GetComponent<Renderer>().material.color = _startPosition.ColorRenderer.material.color;
            _startPosition.ResetNode(false,Color.white);
            _path.Reverse();
            _currentPosition = 0;
        }
        
    }

    private void CreatePath(Node node)
    {
        ResetNodes();
        var visitedNodes = new Dictionary<string, int>();
        foreach (var position in _positions)
        {
            visitedNodes.Add(position.name,-1);
        }
        visitedNodes[_startPosition.name] = 0;
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(_startPosition);
        var count = 0;
        while (queue.Count > 0)
        {
            count++;
            var value = queue.Dequeue();
            Debug.Log($"Find {value}");
            foreach (var element in value._neighbors)
            {
                if (element == node)
                {
                    visitedNodes[element.name] = count;
                    break;
                }
                if (element.IsOccupied || visitedNodes[element.name] > -1)
                {
                    continue;
                }
                visitedNodes[element.name] = count;
                queue.Enqueue(element);
            }
        }
        count = visitedNodes[node.name];
        _path.Add(node);
        var lastNode = node;
        while (count > 0)
        {
            var countBefore = _path.Count;
            Debug.Log($"Vozvr {lastNode.name}");
            foreach (var element in lastNode._neighbors)
            {
                if (visitedNodes[element.name] != count -1)
                {
                    continue;
                }
                lastNode = element;
                _path.Add(element);
            }
            if (_path.Count == countBefore)
            {
                _nodeMoving = NodeMoving.Nothing;
                return;
            }
            count--;
        }
        foreach (var node1 in _path)
        {
            Debug.Log(node1.name);
        }
    }

    private void FindPossiblePositions(Node node)
    {
        ResetNodes();
        var count = 0;
        var visitedNodes = new Dictionary<string, bool>();
        foreach (var position in _positions)
        {
            visitedNodes.Add(position.name,position.IsOccupied);
        }
        //visitedNodes[node.name] = true;
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            count++;
            var value = queue.Dequeue();
            Debug.Log($"Перешли к узлу {value}");
            foreach (var element in value._neighbors)
            {
                if (element.IsOccupied || visitedNodes[element.name])
                {
                    continue;
                }
                visitedNodes[element.name] = true;
                element.ColorRenderer.material.color = Color.cyan;
                queue.Enqueue(element);
            }
        }

        if (count == 1)
        {
            _nodeMoving = NodeMoving.Nothing;
        }
        Debug.Log($"Алгоритм закончился");
    }

    private void ResetNodes()
    {
        foreach (var position in _positions)
        {
            if (!position.IsOccupied)
            {
                position.ColorRenderer.material.color = Color.white;
            } 
        }
    }
}
