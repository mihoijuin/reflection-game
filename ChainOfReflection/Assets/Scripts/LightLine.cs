using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLine : MonoBehaviour
{
    private LineRenderer lr;
    private RaycastHit2D[] hitArray;

    private void Awake(){
        lr = GetComponent<LineRenderer>();
        lr.generateLightingData = true;
    }

    private void Start(){
        hitArray = new RaycastHit2D[SceneBase.moveDataArray.Length];
        if(SceneBase.isGoalDebugMode) hitArray = new RaycastHit2D[SceneBase.debugLightNum];
    }

    private void FixedUpdate(){
        // Rayを飛ばす
        for(int i=0; i<hitArray.Length; ++i){
            if(i == 0) {
                hitArray[i] = DrawRaycast(transform.position, SceneBase.angleValueArray[i]);
            } else
            {
                if(hitArray[i-1]) hitArray[i] = DrawRaycast(hitArray[i-1].transform.position, SceneBase.angleValueArray[i]);
            }
        }
    }

    private RaycastHit2D DrawRaycast(Vector2 from, float angle){
        if(angle != 360) Debug.DrawRay(from, new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)) * 100, Color.blue, 1);
        return Physics2D.Raycast(from, new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)));
    }

    public IEnumerator LightUp(){
        // 対象オブジェクトにヒットした回数を取得
        int hitCount = 0;
        for(int i=0; i<hitArray.Length-1; ++i){
            string hitName = hitArray[i].collider.name;
            string targetName = SceneBase.moveDataArray[i+1][0];
            Debug.Log(hitName);
            if(hitName == targetName && hitCount >= i) hitCount += 1;
        }

        // 光を伸ばす場所をRayCastから取得
        Vector3[] linePointArray = new Vector3[hitArray.Length+1];
        var data =
            from x in SceneBase.moveDataArray
            select x[0];
        string[] nameArray = data.ToArray();
        for(int i=0; i<linePointArray.Length; ++i){
            if(i==0){
                linePointArray[i] = transform.position;
            } else
            {
                if(nameArray.Contains(hitArray[i-1].collider.name) || hitArray[i-1].collider.name == "HitPlace"){
                    linePointArray[i] = hitArray[i-1].transform.position;
                } else
                {
                    linePointArray[i] = hitArray[i-1].point;
                }
            }
        }

        // 徐々に光を伸ばしていく
        DG.Tweening.Tween[] tweenArray = new DG.Tweening.Tween[hitCount+1];
        lr.SetPosition(0, linePointArray[0]);   // 初期設定
        lr.positionCount += 1;
        int lineNum = 1;
        lr.SetPosition(lineNum, linePointArray[0]);
        for (int i = 0; i < tweenArray.Length; ++i){    // 線を伸ばすためのTweenの配列を作成
            int index = i;  // なぜかtweenの中ではインクリメントが使用できなかったので新しくint型変数を作成
            tweenArray[i] = AppUtil.DOTO(
                () => linePointArray[index],
                v =>
                {
                    linePointArray[index] = v;
                    lr.SetPosition(lineNum, linePointArray[index]);
                },
                linePointArray[index+1],
                0.5f,
                "OutExpo",
                0.5f
            );
            if(i < tweenArray.Length-1){    // 次のラインのセッティング
                AppUtil.SetOnCompleteCallback(tweenArray[i], ()=>{
                    lineNum++;
                    lr.positionCount+=1;
                    lr.SetPosition(lineNum, linePointArray[index+1]);
                    lr.endWidth += 0.3f;
                });
            }
        }

        AppUtil.SetOnCompleteCallback(AppUtil.DOSequence(tweenArray, 0f, 0f, 1), ()=>{  // 線を伸ばす&ゴール判定
            if(hitCount == nameArray.Length-1 && hitArray[hitArray.Length-1].collider.name == "HitPlace") Debug.Log("goal");
        });

        yield break;
    }
}
