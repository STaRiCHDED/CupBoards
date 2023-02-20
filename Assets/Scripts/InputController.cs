using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    [HideInInspector] 
    public UnityEvent<Node> OnNodeSelected;
    [SerializeField] 
    private LayerMask _layer;

    private bool _isSelected;
    private GameObject _previousSelectedObject;

    private Node _currentNode;
    //private NodeMoving _nodeMoving;

    private void Awake()
    {
        _isSelected = false;
        _previousSelectedObject = null;
        _currentNode = null;
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, _layer))
        {
            _previousSelectedObject = hitInfo.collider.gameObject;
            if (!_isSelected)
            {
                _isSelected = true;
                _previousSelectedObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.01f);
                //_previousSelectedObject.transform.localScale = new Vector3(scale.x + 0.2f, scale.y + 0.2f, 0);
                _currentNode = _previousSelectedObject.GetComponent<Node>();
            }

            if (Input.GetMouseButtonDown(0) && _isSelected)
            {
                OnNodeSelected?.Invoke(_currentNode);
            }
        }
        else
            ResetScale();
    }

    private void ResetScale()
    {
        if (_isSelected)
        {
            _previousSelectedObject.transform.DOScale(new Vector3(1, 1, 1), 0.01f);
            _isSelected = false;
        }
    }
}