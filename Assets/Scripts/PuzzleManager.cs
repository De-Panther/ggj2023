using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    public Transform cubesParent;
    public PuzzleCube cubePrefab; // Prefab for the cube game object
    public Vector2Int gridSize = new (6,10); // Number of cubes in each row and column of the grid
    public float spacing = 1f; // Space between cubes in the grid
    public float disconnectDistance = 0.1f;
    public System.Action FinishedPuzzle;
    
    private readonly Dictionary<Vector2Int,List<PuzzleCube>> _grid = new(); // 2D array to store the grid of cubes
    private readonly Dictionary<PuzzleCube, Vector2Int> _movables = new();
    private readonly Dictionary<PuzzleCube, Vector2Int> _required = new();
    private bool _shuffled;

    private void Awake()
    {
        if (cubesParent == null)
            cubesParent = transform;
        foreach (var cube in cubesParent.GetComponentsInChildren<PuzzleCube>())
        {
            cube.Init(this);
        }
    }

    private void Start()
    {
        foreach (var cube in cubesParent.GetComponentsInChildren<PuzzleCube>())
        {
            var pos = GetCell(cube.transform.position);

            // Places a new cube at the calculated position
            cube.transform.position = GetWorld(pos);
                
            _grid.Add(pos, new List<PuzzleCube> {cube});
            // Store the newly created cube in the grid array
            if (cube.movable)
            {
                _movables.Add(cube, pos);
                if (cube.required)
                    _required.Add(cube, pos);
            }
        }
        Shuffle();
    }

    private void Shuffle()
    {
        // separate a cube to enforce that at least one cube will be randomized
        var lastCube = _movables.Keys.Last();
        foreach (var cube in _movables.Keys.ToList())
        {
            var cubePos = _movables[cube];
            var other = _movables.Keys.Where(k=>k!=cube&&k!=lastCube).OrderBy(_ => Random.value).FirstOrDefault();
            if(other == null)
                continue;
            var otherPos = _movables[other];
            _movables[cube] = otherPos;
            cube.transform.position = GetWorld(otherPos);
            _grid[cubePos].Remove(cube);
            _grid[cubePos].Add(other);
            _movables[other] = cubePos;
            other.transform.position = GetWorld(cubePos);
            _grid[otherPos].Remove(other);
            _grid[otherPos].Add(cube);
        }
        _shuffled = true;
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
                _movables.Add(cube, posKey);
                if (cube.required)
                    _required.Add(cube, posKey);
                cube.name += $"_{x},{y}";
            }
        }
    }

    public void PlaceOnGrid(Vector2 position, PuzzleCube cube, bool extraZ)
    {
        var pos = _movables[cube];
        var newPos = GetCell(position);
        if (!_grid.ContainsKey(newPos))
        {
            cube.GridConnected(false);
            cube.target.localPosition = Vector3.zero;
            return;   
        }
        cube.GridConnected(true);
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
        else
        {
            if (height == 0 && _required.ContainsKey(cube) && _required[cube] == newPos)
                cube.LockInPlace();
        }
        _movables[cube] = newPos;

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
        }
        else 
            cube.target.localPosition = Vector3.zero;
        
        if (!extraZ && cube.required && PuzzleSolved())
        {
            FinishedPuzzle?.Invoke();
            Debug.Log("Finished puzzle: " + name);
        }
    }

    private Vector2Int GetCell(Vector3 worldPosition)
    {
        var pos = worldPosition - transform.position;
        // Calculate the grid position from the world position
        int x = Mathf.RoundToInt(pos.x / spacing);
        int y = Mathf.RoundToInt(pos.y / spacing);

        // Return the grid position as a Vector2Int
        return new Vector2Int(x,y);
    }
    
    private Vector3 GetWorld(Vector2Int cellPosition)
    {
        return  transform.position + spacing * new Vector3(cellPosition.x, cellPosition.y);;
    }

    private bool PuzzleSolved()
    {
        return _shuffled && _required.All(r => _movables[r.Key] == r.Value && r.Key.transform.localPosition.z > -1);
    }
}