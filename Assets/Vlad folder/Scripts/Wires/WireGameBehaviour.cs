using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireGameBehaviour : MonoBehaviour
{
    public int height;
    public int lenght;
    public Vector3 wallSize;
    public Transform gameParent;
    public Transform wallPrefab;
    public Transform wireStartPrefab;
    public Transform wireEndPrefab;
    public Transform wireParent;
    public Transform wirePrefab;
    public Transform debugItem;

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
        if (!Application.isMobilePlatform)
        {
            height *= 2;
            lenght *= 2;
            gameParent.transform.localScale /= 2;
        }
        Regenrate();
    }

    private bool IsMazeSolvable(int x, int y, int endX, int endY)
    {
        if (x == endX && y == endY)
            return true;
        maze[x, y] = -1;
        var isSolvable = false;
        foreach (var dir in directions)
        {
            var xx = x + dir.x;
            var yy = y + dir.y;
            if (xx >= 0 && xx < maze.GetLength(0))
                if (yy >= 0 && yy < maze.GetLength(1))
                    if (maze[xx, yy] == 0)
                    {
                        isSolvable |= IsMazeSolvable(xx, yy, endX, endY);
                    }
        }
        return isSolvable;
    }

    private void GenerateNewMaze()
    {
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
                int x = i + directions[dir1].x;
                int y = j + directions[dir1].y;
                if (x != 0 && y != 0)
                    if (x != sx - 1 && y != sy - 1)
                        maze[x, y] = 0;
                if (Random.value > 0.5)
                {
                    var dir2 = dir1;
                    while (dir1 == dir2)
                    {
                        dir2 = Random.Range(0, 4);
                    }
                    x = i + directions[dir2].x;
                    y = j + directions[dir2].y;
                    if (x != 0 && y != 0)
                        if (x != sx - 1 && y != sy - 1)
                            maze[x, y] = 0;
                }
            }
        maze[0, 1] = 0;
        maze[sx - 1, sy - 2] = 0;
        if (!IsMazeSolvable(0, 1, sx - 1, sy - 2))
            GenerateNewMaze();
        maze[0, 1] = 2;
        maze[sx - 1, sy - 2] = 3;
        //Debug.Log("Is maze solvable " + IsMazeSolvable(0, 1, sx - 1, sy - 2));
    }

    public void Regenrate()
    {
        foreach (Transform t in gameParent)
            Destroy(t.gameObject);

        GenerateNewMaze();

        int sx = maze.GetLength(0);
        int sy = maze.GetLength(1);
        for (int i = 0; i < sx; i++)
            for (int j = 0; j < sy; j++)
            {
                float x = (i - 1f * sx / 2f + 1 / 2f) * wallSize.x;
                float y = (j - 1f * sy / 2f + 1 / 2f) * wallSize.y;
                if (maze[i, j] == 1)
                {
                    var wall = Instantiate(wallPrefab, gameParent);
                    wall.localPosition = new Vector3(x, y, -0.01f);
                    wall.name = wallPrefab.name;
                }
                //else if (maze[i, j] == -1)
                //{
                //    float x = (i - 1f * sx / 2f + 1 / 2f) * wallSize.x;
                //    float y = (j - 1f * sy / 2f + 1 / 2f) * wallSize.y;
                //    var wall = Instantiate(wallPrefab, gameParent);
                //    wall.localPosition = new Vector3(x, y, -0.01f);
                //    wall.localScale /= 2;
                //}
                else if (maze[i, j] == 2)
                {
                    var wall = Instantiate(wireStartPrefab, gameParent);
                    wall.localPosition = new Vector3(x, y, -0.01f);
                    wall.name = wireStartPrefab.name;
                }
                else if (maze[i, j] == 3)
                {
                    var wall = Instantiate(wireEndPrefab, gameParent);
                    wall.localPosition = new Vector3(x, y, -0.01f);
                    wall.name = wireEndPrefab.name;
                }
            }
    }

    private bool pressedFire = false;
    private Vector3 lastWirePosition;
    private Transform lastWireItem;

    private void FailedGame()
    {
        foreach(Transform child in wireParent)
            Destroy(child.gameObject);
        pressedFire = false;
        SoundManager.Instance.StopClip("Electricity");
    }

    private void PlaceNextWire(RaycastHit hit)
    {
        if(lastWireItem == null)
            lastWireItem = Instantiate(wirePrefab, wireParent);
        var locaPos = wireParent.InverseTransformPoint(hit.point);
        lastWireItem.localPosition = (lastWirePosition + locaPos) / 2;
        lastWireItem.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(lastWirePosition.y - locaPos.y, lastWirePosition.x - locaPos.x));
        var len = (lastWirePosition - locaPos).magnitude * 10;
        lastWireItem.localScale = new Vector3(len, 0.2f, 1);

        if(len > 0.3f)
        {
            lastWirePosition = locaPos;
            lastWireItem = null;
        }
    }

    public void WinGame()
    {
        GameBehaviour.SetGlobalValue(GameVariableKeys.WireRepapred.ToString(), 1);
        GameBehaviour.SetGlobalValue(GameVariableKeys.LaserCanBeCharged.ToString(), 1);
        ScreenBahaviour.Instance.SetCameraLocation("Desk");
        SoundManager.Instance.StopClip("Electricity");
    }

    private void Update()
    {
        if (GameBehaviour.GetGlobalValue(GameVariableKeys.WireRepapred.ToString()) > 0.5f)
            return;
        if(Input.GetAxis("Fire1") > 0 || Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                debugItem.position = hit.point;
                if(hit.collider.name == "WireStart")
                {
                    if(!pressedFire)
                    {
                        var locaPos = wireParent.InverseTransformPoint(hit.point);
                        lastWirePosition = locaPos;
                        pressedFire = true;
                        lastWireItem = null;
                        SoundManager.Instance.PlayClip("Electricity");
                    }
                }
                else if(hit.collider.name == "Board")
                {
                    if(pressedFire)
                    {
                        PlaceNextWire(hit);
                    }
                }
                else if(hit.collider.name == "Screw")
                {
                    FailedGame();
                }
                else if (hit.collider.name == "WireEnd")
                {
                    if (pressedFire)
                    {
                        WinGame();
                    }
                }
            }
            else Debug.Log("Noting hit");
        }
        else FailedGame();
    }
}
