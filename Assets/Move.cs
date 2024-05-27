using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update

    // 完了までにかかる時間
    private float timeTaken = 0.2f;
    // 経過時間
    private float timeElapsed;
    // 目的地
    private Vector3 destination;
    // 出発地
    private Vector3 original;

    public void MoveTo(Vector3 newDestination)
    {
        // 目的地が変わった場合のみ移動を開始する
        if (newDestination != destination)
        {
            // 経過時間を初期化
            timeElapsed = 0;
            // 出発地を現在の位置に設定
            original = transform.position;
            // 新しい目的地を設定
            destination = newDestination;
        }
    }

    void Start()
    {
        //目的地・出発地を現在地で初期化
        destination = transform.position;
        original = destination;
    }

    // Update is called once per frame
    void Update()
    {
        // 目的地に到着していたら処理しない
        if (original == destination) { return; }

        // 時間経過を加算
        timeElapsed += Time.deltaTime;
        // 経過時間が完了時間の何割かを算出
        float timeRate = timeElapsed / timeTaken;
        // 完了時間を超えるようであれば実行完了時間相当に丸める
        if (timeRate > 1) { timeRate = 1; }
        // イージング用計算（リニア）
        float easing = timeRate;
        // 座標を算出
        Vector3 currentPosition = Vector3.Lerp(original, destination, easing);
        // 算出した座標をpositionに代入
        transform.position = currentPosition;

        // 移動が完了したらoriginalをdestinationに設定
        if (timeRate == 1)
        {
            original = destination;
        }
    }
}
