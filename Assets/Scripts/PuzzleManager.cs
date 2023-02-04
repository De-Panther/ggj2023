using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public Transform cubesParent;
    public PuzzleCube cubePrefab; // Prefab for the cube game object
    public Vector2Int gridSize = new (6,10); // Number of cubes in each row and column of the grid
    public float spacing = 1f; // Space between cubes in the grid
    public float disconnectDistance = 0.1f;
    
    private Dictionary<Vector2Int,List<PuzzleCube>> _grid = new(); // 2D array to store the grid of cubes
    private readonly Dictionary<PuzzleCube, Vector2Int> _positions = new();
    private Dictionary<PuzzleCube, Vector2Int> _required = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (cubesParent == null)
            cubesParent = transform;
        foreach (var cube in cubesParent.GetComponentsInChildren<PuzzleCube>())
        {
            var pos = GetCellFromWorldPosition(cube.transform.position);

            // Instantiate a new cube at the calculated position
            cube.transform.position = transform.position + spacing * new Vector3(pos.x, pos.y);
                
            // Store the newly created cube in the grid array
            _grid.Add(pos, new List<PuzzleCube> {cube});
            _positions.Add(cube, pos);
            if (cube.required)
                _required.Add(cube, pos);
        }
    }

    private void CreateFromPrefab()
    {
        // Create a loop to generate the grid of cubes
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                // Calculate the position of the current cube
                var posX = x * spacing;
                var posY = y * spacing;

                // Instantiate a new cube at the calculated position
                var cube = Instantiate(cubePrefab,  transform.position+new Vector3(posX, posY), Quaternion.identity, transform);
                
                // Store the newly created cube in the grid array
                var posKey = new Vector2Int(x, y);
                _grid.Add(posKey, new List<PuzzleCube> {cube});
                _positions.Add(cube, posKey);
                if (cube.required)
                    _required.Add(cube, posKey);
                cube.name += $"_{x},{y}";
            }
        }
    }

    public void PlaceOnGrid(Vector2 position, PuzzleCube cube, bool extraZ)
    {
        var pos = _positions[cube];
        var newPos = GetCellFromWorldPosition(position);
        if (!_grid.ContainsKey(newPos))
        {
            cube.target.localPosition = Vector3.zero;
            return;   
        }
        if (pos != newPos)
            _grid[pos].Remove(cube);
            
        var newCell = _grid[newPos];
        if (pos != newPos)
        {
            newCell.Add(cube);
        }

        var height = newCell.Count(o => o != cube);
        var cellPosition = new Vector3(newPos.x * spacing, newPos.y * spacing, height * -spacing);
        if (extraZ)
        {
            if (height == 0)
            {
                cellPosition.z = -spacing * 0.8f;
            }
            else
            {
                cellPosition.z -= spacing / 4;
            }
        }
        _positions[cube] = newPos;
        if (!extraZ && PuzzleSolved())
            Debug.Log("LEVEL SOLVED!");

        var final = transform.position + cellPosition;
        cube.target.position = final;

        if (Mathf.Abs(cube.transform.position.x - final.x) < disconnectDistance
            && Mathf.Abs(cube.transform.position.y - final.y) < disconnectDistance
            && cube.transform.localPosition.z < disconnectDistance
            && cube.transform.position.z > final.z - disconnectDistance)
        {
            // Set the position of the game object to the snapped position
            cube.target.position = final;
            var s = newCell.Aggregate($"{cube.name}[{newPos}]", (current, c) => current + (c.name + " "));
            if(!extraZ)
                Debug.Log(s);
        }
        else 
            cube.target.localPosition = Vector3.zero;
    }

    private Vector2Int GetCellFromWorldPosition(Vector3 worldPosition)
    {
        var pos = worldPosition - transform.position;
        // Calculate the grid position from the world position
        int x = Mathf.RoundToInt(pos.x / spacing);
        int y = Mathf.RoundToInt(pos.y / spacing);

        // Return the grid position as a Vector2Int
        return new Vector2Int(x,y);
    }

    private bool PuzzleSolved()
    {
        return _required.All(r => _positions[r.Key] == r.Value);
    }
}