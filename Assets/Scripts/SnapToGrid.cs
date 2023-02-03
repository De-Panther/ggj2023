using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public Transform target;
 

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
        var position = PuzzleManager.Instance.PlaceOnGrid(transform.position, gameObject, _extraZ);
        
        // Set the position of the game object to the snapped position
        target.position = position;
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