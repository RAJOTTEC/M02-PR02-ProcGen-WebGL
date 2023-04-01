using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    public Text widthText;
    public Slider widthSlider;
    public Text heightText;
    public Slider heightSlider;

    public int width = 10;
    public int height = 10;
    public float wallLength = 1f;
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    private bool[,] visited;

    void Start()
    {
        visited = new bool[width, height];

        GenerateMaze();
    }

    void Update()
    {
        // Assigning values to sliders and text
        width = (int)widthSlider.value;
        widthText.text = width.ToString();
        height = (int)heightSlider.value;
        heightText.text = height.ToString();

        // Check for input to regenerate the maze
        if (Input.GetKeyDown("space"))
        {
            // Destroy any existing walls
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Reset the visited array and generate a new maze
            visited = new bool[width, height];
            GenerateMaze();
        }
    }


    void GenerateMaze()
    {
        // Create a border of walls around the maze
        for (int x = 0; x < width; x++)
        {
            InstantiateWall(new Vector3(x, 0, -0.5f), Quaternion.identity); // South wall
            InstantiateWall(new Vector3(x, 0, height - 0.5f), Quaternion.identity); // North wall
        }

        for (int y = 0; y < height; y++)
        {
            InstantiateWall(new Vector3(-0.5f, 0, y), Quaternion.Euler(0f, 90f, 0f)); // West wall
            InstantiateWall(new Vector3(width - 0.5f, 0, y), Quaternion.Euler(0f, 90f, 0f)); // East wall
        }

        // Create floor of the maze
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                InstantiateFloor(new Vector3(x, 0, y), Quaternion.Euler(0f, 90f, 0f));
            }
        }

        // Start at a random position
        int startX = Random.Range(0, width);
        int startY = Random.Range(0, height);

        // Mark the starting position as visited
        visited[startX, startY] = true;

        // Create the initial cell stack
        Stack<Vector2Int> cellStack = new Stack<Vector2Int>();
        cellStack.Push(new Vector2Int(startX, startY));

        while (cellStack.Count > 0)
        {
            // Get the current cell
            Vector2Int currentCell = cellStack.Peek();

            // Get the neighbors of the current cell
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(currentCell);

            if (neighbors.Count > 0)
            {
                // Choose a random unvisited neighbor
                Vector2Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];

                // Remove the wall between the current cell and the chosen neighbor
                RemoveWall(currentCell, randomNeighbor);

                // Mark the chosen neighbor as visited and push it onto the stack
                visited[randomNeighbor.x, randomNeighbor.y] = true;
                cellStack.Push(randomNeighbor);
            }
            else
            {
                // If there are no unvisited neighbors, backtrack
                cellStack.Pop();
            }
        }
    }

    void InstantiateFloor(Vector3 position, Quaternion rotation)
    {
        GameObject floor = Instantiate(floorPrefab, position, rotation, this.transform);
    }

    void InstantiateWall(Vector3 position, Quaternion rotation)
    {
        GameObject wall = Instantiate(wallPrefab, position * wallLength, rotation, this.transform);
        wall.transform.localScale = new Vector3(wallLength * 1.2f, 1, 0.2f);
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>();

        // Check north neighbor
        if (cell.y < height - 1 && !visited[cell.x, cell.y + 1])
        {
            unvisitedNeighbors.Add(new Vector2Int(cell.x, cell.y + 1));
        }

        // Check east neighbor
        if (cell.x < width - 1 && !visited[cell.x + 1, cell.y])
        {
            unvisitedNeighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        }

        // Check south neighbor
        if (cell.y > 0 && !visited[cell.x, cell.y - 1])
        {
            unvisitedNeighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        }

        // Check west neighbor
        if (cell.x > 0 && !visited[cell.x - 1, cell.y])
        {
            unvisitedNeighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        }

        return unvisitedNeighbors;
    }

    void RemoveWall(Vector2Int cell1, Vector2Int cell2)
    {
        // Calculate the position of the wall between the two cells
        float x = (cell1.x + cell2.x) * 0.5f;
        float y = (cell1.y + cell2.y) * 0.5f;

        // Instantiate the wall prefab and rotate it based on the direction between the two cells
        GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0f, y), Quaternion.identity, this.transform);
        if (cell1.x == cell2.x)
        {
            wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            wall.transform.rotation = Quaternion.identity;
        }
    }
}