using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PuzzleCube : MonoBehaviour
{
    [Header("Parameters")] 
    public bool movable = true;
    public bool required = true;
    public bool randomized = true;
    
    [Header("Settings")]
    public Transform target;
    public float disconnectDistance = 0.1f;

    private Vector3 _lastPos;
    private bool _extraZ;

    private void Update()
    {
        if (_lastPos == transform.position)
            return;
        _lastPos = transform.position;

        PlaceCell();
    }

    private void PlaceCell()
    {
        PuzzleManager.Instance.PlaceOnGrid(transform.position, this, _extraZ);

    }
    
    void OnMouseDown()
    {
        _extraZ = true;
        PlaceCell();
    }

    void OnMouseUp()
    {
        _extraZ = false;
        PlaceCell();
    }
}