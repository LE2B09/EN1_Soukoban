using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    int[,] map;
    GameObject[,] field;

    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].CompareTag("Player")) { return new Vector2Int(x, y); }
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        // nullチェックしてからタグチェックを行う
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        Vector3 moveToPosition = new Vector3(moveTo.x, map.GetLength(0) - 1 - moveTo.y, 0);
        Move moveComponent = field[moveFrom.y, moveFrom.x].GetComponent<Move>();

        if (moveComponent != null)
        {
            moveComponent.MoveTo(moveToPosition);
        }

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        return true;
    }


    bool IsCleared()
    {
        //Vector2Int型の可変長配列の作成
        List<Vector2Int> goals = new();

        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                //格納場所が否かどうか
                if (map[y, x] == 3)
                {
                    //格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        // 要素数はgoals.Countで取得
        foreach (Vector2Int goal in goals)
        {
            if (field[goal.y, goal.x] == null || !field[goal.y, goal.x].CompareTag("Box"))
            {
                // 一つでも箱がなかったら条件未達成
                return false;
            }
        }
        //条件未達成でなければ条件達成
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        //解像度とウィンドウモード
        Screen.SetResolution(1280, 720, false);
        map = new int[,]    //3を格納場所とする
        {
            {0, 0, 0, 0, 0},
            {0, 0, 1, 2, 3},
            {0, 0, 2, 0, 0},
            {0, 0, 3, 0, 0},
            {0, 0, 0, 0, 0}
        };

        field = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(playerPrefab, new Vector3(x, map.GetLength(0) - 1 - y, 0), Quaternion.identity);
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(boxPrefab, new Vector3(x, map.GetLength(0) - 1 - y, 0), Quaternion.identity);
                }
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(goalPrefab, new Vector3(x, map.GetLength(0) - 1 - y, 0), Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
        }

        // もしクリアしていたら
        if (IsCleared())
        {
            Debug.Log("Clear!!!");
            clearText.SetActive(true);
        }
    }

}
