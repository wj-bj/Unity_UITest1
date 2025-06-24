using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Match3BoardGenerator : MonoBehaviour
{
    [Header("Board Size")]
    public int width = 8;  // Number of columns
    public int height = 8; // Number of rows

    [Header("Tile Settings")]
    public GameObject[] tilePrefabs; // List of tile prefabs to randomly choose from
    public float tileSpacing = 1.0f; // Distance between each tile

    [Header("Parent for Tiles")]
    public Transform tileParent; // Optional: parent object to organize tiles in hierarchy

    private List<GameObject> spawnedTiles = new List<GameObject>();
    private int[,] tileTypes; // 2D array to store tile types for connections

    void Start()
    {
        GenerateBoard(); // Optional: remove if only editor button is desired
        UpdateTileMatch();
    }

    /// <summary>
    /// Generate the match-3 game board by filling it with random tiles.
    /// </summary>
    public void GenerateBoard()
    {
        ClearBoard();

        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogError("Please assign tile prefabs.");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x * tileSpacing, y * tileSpacing);
                int randomIndex = Random.Range(0, tilePrefabs.Length);
                GameObject tileMesh = Instantiate(tilePrefabs[randomIndex]);
                GameObject tile = new GameObject($"Tile{randomIndex}_{x}_{y}");
                tileMesh.transform.SetParent(tile.transform);
                if (tileParent != null)
                {
                    tile.transform.SetParent(tileParent);
                    tile.transform.localPosition = new Vector3(position.x, position.y, 0f);
                }


                tile.name = $"Tile{randomIndex}_{x}_{y}";

                spawnedTiles.Add(tile);
                tileTypes[x, y] = randomIndex; // Store the type of tile at this position
            }
        }
    }

    GameObject GetTileAt(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null; // Out of bounds
        }

        int index = x + y * width;

        if (index >= 0 && index < spawnedTiles.Count)
        {
            return spawnedTiles[index];
        }

        return null; // Tile not found
    }

    int[,] FindMatches()
    {
        int[,] board = tileTypes;
        int width = board.GetLength(0);
        int height = board.GetLength(1);
        int[,] result = new int[width, height];

        // Horizontal check
        for (int y = 0; y < height; y++)
        {
            int count = 1;
            for (int x = 1; x < width; x++)
            {
                if (board[x, y] == board[x - 1, y])
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        for (int i = 0; i < count; i++)
                            result[x - 1 - i, y] = 1;
                    }
                    count = 1;
                }
            }
            if (count >= 3)
            {
                for (int i = 0; i < count; i++)
                    result[width - 1 - i, y] = 1;
            }
        }

        // Vertical check
        for (int x = 0; x < width; x++)
        {
            int count = 1;
            for (int y = 1; y < height; y++)
            {
                if ( board[x, y] == board[x, y - 1])
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        for (int i = 0; i < count; i++)
                            result[x, y - 1 - i] = 1;
                    }
                    count = 1;
                }
            }
            if (count >= 3)
            {
                for (int i = 0; i < count; i++)
                    result[x, height - 1 - i] = 1;
            }
        }

        return result;
    }

    void UpdateTileMatch()
    {
        int[,] matches = FindMatches();
        // for (int y = height - 1; y >= 0; y--) // 从上往下打印（棋盘视觉上通常是上层在上）
        // {
        //     string line = "";
        //     for (int x = 0; x < width; x++)
        //     {
        //         line += matches[x, y] + " ";
        //     }
        //     Debug.Log(line);
        // }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (matches[x, y] == 1)
                {
                    GameObject tile = GetTileAt(x, y);
                    if (tile != null)
                    {
                        tile.AddComponent<TileClickRotate>(); // Ensure TileClickRotate is added to the tile
                    }
                }
            }
        }
    }

    /// <summary>
    /// Destroy all tiles under the tileParent.
    /// </summary>
    public void ClearBoard()
    {
        if (tileParent == null) return;

        for (int i = tileParent.childCount - 1; i >= 0; i--)
        {

            DestroyImmediate(tileParent.GetChild(i).gameObject);

        }
        spawnedTiles.Clear();
        tileTypes = new int[width, height]; // Reset the tile types array
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Match3BoardGenerator))]
public class Match3BoardGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Match3BoardGenerator generator = (Match3BoardGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Board"))
        {
            generator.GenerateBoard();
        }

        if (GUILayout.Button("Clear Board"))
        {
            generator.ClearBoard();
        }
    }
}
#endif