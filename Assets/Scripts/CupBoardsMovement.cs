using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private NodeMoving _nodeStatus;
    private List<Node> _path;
    private GameObject _movableActor;
    private float _currentTime;
    private int _currentPosition;

    public void Initialize(UnityEvent<Node> onNodeSelected, Node[] positions)
    {
        _onNodeSelected = onNodeSelected;
        _positions = positions;
        _path = new List<Node>();
        _nodeStatus = NodeMoving.Nothing;
        _currentTime = 0;
    }

    void Start()
    {
        _onNodeSelected.AddListener(ManageNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (_nodeStatus == NodeMoving.Moving)
        {
            if (_currentPosition == _path.Count - 1)
            {
                _endPosition.ResetNode(true);
                _endPosition.ChangeNodeColor(_movableActor.GetComponent<Renderer>().material.color);
                Destroy(_movableActor);
                OnCupBoardMoved?.Invoke();
                _nodeStatus = NodeMoving.Nothing;
                Debug.Log("Перемещение завершилось");
                return;
            }

            _currentTime += Time.deltaTime;
            var result = _currentTime / _travelTime;
            _movableActor.transform.position = Vector3.Lerp(_path[_currentPosition].transform.position,
                _path[GetNextIndex()].transform.position, result);

            if (_currentTime >= _travelTime)
            {
                _currentTime = 0;
                _currentPosition = GetNextIndex();
                Debug.Log($"Перемещение в точку {_path[_currentPosition].name} завершилось");
            }
        }
    }

    int GetNextIndex()
    {
        return _currentPosition + 1;
    }
    void OnDestroy()
    {
        _onNodeSelected.RemoveListener(ManageNode);
    }

    private void ManageNode(Node node)
    {
        Debug.Log(_nodeStatus);
        if (_nodeStatus == NodeMoving.Nothing && node.IsOccupied)
        {
            _path.Clear();
            _endPosition = null;
            _startPosition = node;
            _nodeStatus = NodeMoving.Start;
            FindPossiblePositions();
            return;
        }

        if (_nodeStatus == NodeMoving.Start && !node.IsOccupied)
        {
            _endPosition = node;
            _currentPosition = 0;
            _nodeStatus = NodeMoving.Moving;
            
            CreatePath();
            _movableActor = Instantiate(_moveableCupBoard);
            
            var position = _startPosition.transform.position;
            _movableActor.transform.position = new Vector3(position.x, position.y, position.z - 2);
            
            _movableActor.GetComponent<Renderer>().material.color = _startPosition.MaterialColor.color;
            _startPosition.ResetNode(false);
            _startPosition.ChangeNodeColor(Color.white);
        }
    }

    private void CreatePath()
    {
        ResetNodesColor();
        var visitedNodes = new Dictionary<string, int>();
        //Debug.Log($"Построение пути, вывод всех вершин");
        foreach (var position in _positions)
        {
            visitedNodes.Add(position.name, -1);
            //Debug.Log($"{position.name}, {position.IsOccupied}");
        }
        visitedNodes[_startPosition.name] = 0;
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(_startPosition);
        var count = 0;
        while (queue.Count > 0)
        {
            count++;
            var value = queue.Dequeue();
            //Debug.Log($"Попал в вершину {value.name} - {visitedNodes[value.name]}");
            foreach (var element in value.Neighbors)
            {
                if (element == _endPosition)
                {
                    visitedNodes[element.name] = count;
                    //Debug.Log($"Конец {element.name} - {visitedNodes[element.name]}");
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

        count = visitedNodes[_endPosition.name];
        _path.Add(_endPosition);
        var lastNode = _endPosition;
        //Debug.Log($"Построение пути count = {count}");
        while (count > 0)
        {
            var countBefore = _path.Count;
            //Debug.Log($"Попал в вершину {lastNode.name} - {visitedNodes[lastNode.name]}{count}{countBefore}");
            foreach (var element in lastNode.Neighbors)
            {
                //Debug.Log($"Соседи {element.name} - {visitedNodes[element.name]}");
                if (visitedNodes[element.name] != count - 1)
                {
                    continue;
                }

                lastNode = element;
                _path.Add(element);
            }

            if (_path.Count == countBefore)
            {
                _nodeStatus = NodeMoving.Nothing;
                Debug.Log($"Нельзя построить путь обратно");
                return;
            }
            count--;
        }
        _path.Reverse();
        var str = "";
        foreach (var node1 in _path)
        {
            str += node1.name;
        }
        Debug.Log(str);
    }

    private void FindPossiblePositions()
    {
        ResetNodesColor();
        var count = 0;
        var visitedNodes = new Dictionary<string, bool>();
        //Debug.Log($"Покраска пути, вывод всех вершин");
        foreach (var position in _positions)
        {
            visitedNodes.Add(position.name, position.IsOccupied);
            Debug.Log($"{position.name}, {position.IsOccupied}");
        }
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(_startPosition);
        while (queue.Count > 0)
        {
            count++;
            var value = queue.Dequeue();
            //Debug.Log($"Перешли к узлу {value}");
            foreach (var element in value.Neighbors)
            {
                //Debug.Log($"{element.name}: {visitedNodes[element.name]}");
                if (visitedNodes[element.name])
                {
                    continue;
                }

                visitedNodes[element.name] = true;
                element.MaterialColor.color = Color.gray;
                queue.Enqueue(element);
            }
        }

        if (count == 1)
        {
            _nodeStatus = NodeMoving.Nothing;
            //Debug.Log($"Вам некуда ходить Status {_nodeStatus}");
            
        }

        //Debug.Log($"Алгоритм закончился");
    }

    private void ResetNodesColor()
    {
        foreach (var position in _positions)
        {
            if (!position.IsOccupied)
            {
                position.ChangeNodeColor(Color.white);
            }
        }
    }
}