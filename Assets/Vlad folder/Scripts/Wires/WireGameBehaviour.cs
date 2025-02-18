using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireGameBehaviour : MonoBehaviour
{
    public int height;
    public int lenght;
    public Transform gameParent;
    public Transform wallPrefab;
    public Vector3 wallSize;

    private int[,] maze;
    private Vector2Int[] directions = new[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };
    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        foreach (Transform t in gameParent)
            Destroy(t.gameObject);
        maze = new int[lenght * 2 + 1, height * 2 + 1];

        int sx = maze.GetLength(0);
        int sy = maze.GetLength(1);

        for (int i = 0; i < sx; i++)
            for (int j = 0; j < sy; j++)
                maze[i, j] = 1;

        for (int i = 1; i < sx - 1; i += 2)
            for (int j = 1; j < sy - 1; j += 2)
            {
                maze[i, j] = 0;
                var dir1 = Random.Range(0, 4);
                var dir2 = dir1;
                while (dir1 == dir2)
                {
                    dir2 = Random.Range(0, 4);
                }
                int x = i + directions[dir1].x;
                int y = j + directions[dir1].y;
                if(x != 0 && y != 0)
                    if(x != sx - 1 && y != sy - 1)
                        maze[x, y] = 0;
                x = i + directions[dir2].x;
                y = j + directions[dir2].y;
                if (x != 0 && y != 0)
                    if (x != sx - 1 && y != sy - 1)
                        maze[x, y] = 0;
            }

        for (int i = 0; i < sx; i++)
            for (int j = 0; j < sy; j++)
                if (maze[i, j] == 1)
                {
                    float x = (i - 1f * sx / 2f + 1 / 2f) * wallSize.x;
                    float y = (j - 1f * sy / 2f + 1 / 2f) * wallSize.y;
                    var wall = Instantiate(wallPrefab, gameParent);
                    wall.localPosition = new Vector3(x, y, -0.01f);
                }
    }
}
