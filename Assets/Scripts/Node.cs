using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [field: SerializeField] 
    public Node[] _neighbors { get; private set;}

    public bool IsOccupied { get; private set; }

    public Renderer ColorRenderer => GetComponent<Renderer>();

    public void ResetNode(bool isOccupied,Color color)
    {
        ColorRenderer.material.color = color;
        IsOccupied = isOccupied;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
