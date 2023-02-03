using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public GameObject cubePrefab; // Prefab for the cube game object
    public Vector2Int gridSize = new (6,10); // Number of cubes in each row and column of the grid
    public float spacing = 1f; // Space between cubes in the grid
    
    private List<GameObject>[,] grid; // 2D array to store the grid of cubes
    private Dictionary<GameObject, Vector2Int> positions = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Initialize the grid array
        grid = new List<GameObject>[gridSize.x, gridSize.y];
        
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
                grid[x, y] = new List<GameObject> {cube};
                positions.Add(cube, new Vector2Int(x,y));
                cube.name += $"_{x},{y}";
            }
        }
    }

    public Vector3 PlaceOnGrid(Vector2 position, GameObject cube, bool extraZ)
    {
        var pos = positions[cube];
        var newPos = GetCellFromWorldPosition(position);
        if (pos != newPos)
            grid[pos.x, pos.y].Remove(cube);
        newPos.x = Math.Clamp(newPos.x, 0, gridSize.x-1);
        newPos.y = Math.Clamp(newPos.y, 0, gridSize.y-1);
        var newCell = grid[newPos.x, newPos.y];
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
        positions[cube] = newPos;
        return transform.position + cellPosition;
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
}