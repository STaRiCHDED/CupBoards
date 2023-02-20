using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Node : MonoBehaviour
{
    [field: SerializeField] 
    public Node[] Neighbors { get; private set; }

    public bool IsOccupied { get; private set; }

    public Material MaterialColor => GetComponent<Renderer>().material;

    public void ResetNode(bool isOccupied)
    {
        IsOccupied = isOccupied;
    }
    public void ChangeNodeColor(Color color)
    {
        MaterialColor.color = color;
    }

    private void Awake()
    {
        IsOccupied = false;
    }
}
