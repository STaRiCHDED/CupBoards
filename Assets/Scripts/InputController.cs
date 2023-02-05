using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Node> OnNodeSelected;
    [SerializeField] 
    private LayerMask _layer;
    
    private bool _isSelected;
    private Vector3 _scale;
    private GameObject _previousSelectedObject;
    private Node _currentNode;
    public void Initialize()
    {
    }

    private void Awake()
    {
        _isSelected = false;
        _scale = Vector3.zero;
        _previousSelectedObject = null;
        _currentNode = null;
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray,out RaycastHit hitInfo,50,_layer))
        {
            _previousSelectedObject = hitInfo.collider.gameObject;
            var scale = _previousSelectedObject.transform.localScale;
            if (!_isSelected)
            {
                _scale = scale;
                _isSelected = true;
                _previousSelectedObject.transform.localScale = new Vector3(scale.x + 0.2f, scale.y + 0.2f, 0);
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
            _previousSelectedObject.transform.localScale = _scale;
            _isSelected = false;
        }
    }
}