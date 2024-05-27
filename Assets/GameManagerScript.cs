using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject particlePrefab;
    public GameObject wallPrefab;

    int[,] map;
    GameObject[,] field;
    int currentStage = 0;

    Stack<Vector2Int[]> moveHistory = new Stack<Vector2Int[]>();

    int[][,] stages = new int[1][,]
   {
       new int[,]
        {
            {4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            {4, 0, 0, 3, 0, 0, 0, 0, 0, 4},
            {4, 0, 2, 0, 4, 4, 4, 0, 2, 4},
            {4, 0, 1, 2, 0, 4, 3, 0, 0, 4},
            {4, 0, 0, 4, 0, 4, 0, 0, 0, 4},
            {4, 0, 3, 0, 2, 0, 2, 0, 0, 4},
            {4, 0, 0, 0, 0, 3, 0, 4, 3, 4},
            {4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
        }

   };

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

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].CompareTag("Wall"))
        {
            return false;
        }

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

        Vector3 moveFromPosition = new Vector3(moveFrom.x, map.GetLength(0) - 1 - moveFrom.y, 0);
        int numberOfParticles = 8;

        for (int i = 0; i < numberOfParticles; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Vector3 particlePosition = moveFromPosition + randomOffset;
            GameObject particle = Instantiate(particlePrefab, particlePosition, Quaternion.identity);
        }

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        moveHistory.Push(new Vector2Int[] { moveFrom, moveTo });

        return true;
    }

    bool IsCleared()
    {
        List<Vector2Int> goals = new();

        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        foreach (Vector2Int goal in goals)
        {
            if (field[goal.y, goal.x] == null || !field[goal.y, goal.x].CompareTag("Box"))
            {
                return false;
            }
        }
        return true;
    }

    void ClearStage()
    {
        // フィールド上のすべてのオブジェクトを削除
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] != null)
                {
                    Destroy(field[y, x]);
                    field[y, x] = null;  // この行を追加してフィールド配列をクリア
                }
            }
        }
        moveHistory.Clear();

        // クリア画面を表示
        clearText.SetActive(true);
    }

    void LoadStage(int stageIndex)
    {
        map = stages[stageIndex];
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
                    field[y, x] = Instantiate(goalPrefab, new Vector3(x, field.GetLength(0) - 1 - y, 0), Quaternion.identity);
                }
                if (map[y, x] == 4)
                {
                    field[y, x] = Instantiate(wallPrefab, new Vector3(x, map.GetLength(0) - 1 - y, 0), Quaternion.identity);
                }
            }
        }

        switch (stageIndex)
        {
            case 0:
                map = stages[0];
                break;
            case 1:
                map = stages[1];
                break;
            case 2:
                map = stages[2];
                break;
        }
    }

    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        LoadStage(currentStage);
    }

    void Update()
    {
        // プレイヤーの移動処理
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

        // 移動履歴を元に戻す処理
        if (Input.GetKeyDown(KeyCode.Z) && moveHistory.Count > 0)
        {
            Vector2Int[] lastMove = moveHistory.Pop();
            Vector2Int moveTo = lastMove[0];
            Vector2Int moveFrom = lastMove[1];

            Vector3 moveToPosition = new Vector3(moveTo.x, map.GetLength(0) - 1 - moveTo.y, 0);
            Move moveComponent = field[moveFrom.y, moveFrom.x].GetComponent<Move>();

            if (moveComponent != null)
            {
                moveComponent.MoveTo(moveToPosition);
            }

            field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
            field[moveFrom.y, moveFrom.x] = null;
        }

        // ステージクリア判定
        if (IsCleared())
        {
            Debug.Log("Clear!!!");
            currentStage++;

            if (currentStage < stages.Length)
            {
                ClearStage();
                LoadStage(currentStage);
            }
            else
            {
                clearText.SetActive(true);
            }
        }
    }

}
