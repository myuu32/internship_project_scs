using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 offset; // ターゲットからのオフセット
    public Vector2 boundsCenter; // 境界制限の中心
    public Vector2 boundsSize; // 境界制限のサイズ
    public float smoothSpeed = 0.125f; // カメラ移動のスムージング係数
    public float minBoundsCenter = 20f; // 最小境界中心値
    public float maxBoundsCenter = 50f; // 最大境界中心値
    public float followSpeed = 5f; // 追跡速度
    public AnimationCurve followCurve; // 追跡曲線

    private GameObject[] players; // プレーヤーオブジェクトを格納する配列
    private Vector3 desiredPosition; // カメラの望ましい位置

    void FixedUpdate()
    {
        // 少なくとも2つのプレーヤーがいるかどうかを確認
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length < 2)
        {
            Debug.LogWarning("'Player' タグを持つプレーヤーが少なくとも2人必要です。");
            return;
        }

        // GameObject 配列を Transform 配列に変換
        Transform[] playerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerTransforms[i] = players[i].transform;
        }

        // プレーヤー間の中心位置を計算
        Vector3 centerPosition = (playerTransforms[0].position + playerTransforms[1].position) / 2f;
        boundsCenter.y = Mathf.Lerp(minBoundsCenter, maxBoundsCenter, Vector3.Distance(playerTransforms[0].position, playerTransforms[1].position) / 50f);

        // カメラの望ましい位置を計算
        desiredPosition = centerPosition + offset;

        // 境界制限を適用
        Vector2 minBounds = boundsCenter - boundsSize / 2f;
        Vector2 maxBounds = boundsCenter + boundsSize / 2f;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // カメラを望ましい位置にスムーズに移動
        float t = followCurve.Evaluate(Time.deltaTime * followSpeed); // 追跡曲線の値を計算
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * t); // speed乘以t
        transform.position = smoothedPosition;
    }
}
