using System;
using UnityEngine;
using WebXR.Interactions;

public class PuzzleCube : MonoBehaviour
{
    public bool movable = true;
    public bool required = true;
    public Transform target;
    public Action ConnectedToGridEvent;
    public Action DisconnectedFromGridEvent;

    private Vector3 _lastPos;
    private bool _extraZ;
    private bool _connectedToGrid = true;
    private PuzzleManager _manager;

    public void Init(PuzzleManager manager)
    {
        _manager = manager;
    }
    
    private void Start()
    {
        if (!movable)
            LockInPlace();
    }

    public void LockInPlace()
    {
        movable = false;
        GetComponent<MouseDragObject>().enabled = false;
    }

    private void Update()
    {
        if(!movable)
            return;
        if (_lastPos == transform.position)
            return;
        _lastPos = transform.position;

        PlaceCell();
    }

    private void PlaceCell()
    {
        if(!movable)
            return;
        _manager.PlaceOnGrid(transform.position, this, _extraZ);
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

    public void GridConnected(bool connected)
    {
        if (connected == _connectedToGrid)
            return;
        _connectedToGrid = connected;
        if(connected)
            ConnectedToGridEvent?.Invoke();
        else 
            DisconnectedFromGridEvent?.Invoke();
    }
}