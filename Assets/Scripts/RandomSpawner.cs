using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    private Node[] _positions;
    private Color[] _colors;
    private GameObject[] _winPositions;
    
    public void Initialize(Node[] positions,Color[] colors,GameObject[] winPositions)
    {
        _positions = positions;
        _colors = colors;
        _winPositions = winPositions;
    }
    void Start()
    {
        var positions = RandomizePositions();
        var colors = RandomizeColors();
        for (int i = 0; i < 6; i++)
        {
            _positions[positions[i]].ResetNode(true,_colors[colors[i]]);
            _winPositions[i].GetComponent<Renderer>().material.color = _colors[colors[i]];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<int> RandomizePositions()
    {
        List<int> positionsArray = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            int number;
            do
            {
                number = Random.Range(0, 8);
            } while (positionsArray.Contains(number));
            positionsArray.Add(number);
        }

        return positionsArray;
    }
    private int[] RandomizeColors()
    {
        int[] colorsArray = new int[6];
        for (var i = 0; i < colorsArray.Length; i++)
        {
            colorsArray[i] = i;
        }
        for (int i = 0; i < 6; i++)
        {
            int j = Random.Range(0, 5);
            (colorsArray[j], colorsArray[i]) = (colorsArray[i], colorsArray[j]);
        }

        return colorsArray;
    }
}
