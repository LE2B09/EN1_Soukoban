using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    int[,] map;
    GameObject[,] field;

    //void PrintArray()
    //{
    //    string debugText = "";
    //    for (int i = 0; i < map.Length; i++)
    //    {
    //        debugText += map[i].ToString() + ",";
    //    }
    //    Debug.Log(debugText);
    //}

    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < field.Length; x++)
            {
                if (x == -1 && y == -1)
                {
                    return GetPlayerIndex();
                }
            }
        }
        return -GetPlayerIndex();
    }

    bool MoveNumber(int number, int moveFrom, int moveTo)
    {
        GameObject obj = null;
        obj.transform.position = new Vector3();

        if (moveTo < 0 || moveTo >= map.Length) { return false; }
        if (map[moveTo] == 2)
        {
            int velocity = moveTo - moveFrom;
            bool success = MoveNumber(2, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // GameObject instance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        map = new int[,]
        {
            {0,0,0,0,0},
            {0,0,0,0,0},
            {0,0,0,0,1}
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
                if (map[x, y] == 1)
                {
                    field[y, x] = Instantiate(playerPrefab, new Vector3(), Quaternion.identity);
                }
            }
        }
        //PrintArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            //int playerIndex = GetPlayerIndex();
            //MoveNumber(1, playerIndex, playerIndex + 1);
            //PrintArray();
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //int playerIndex = GetPlayerIndex();
            //MoveNumber(1, playerIndex, playerIndex - 1);
            //PrintArray();
        }

    }
}
