using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLine : MonoBehaviour
{
    private LineRenderer lr;

    private RaycastHit2D hit1;
    private RaycastHit2D hit2;
    private RaycastHit2D hit3;
    private RaycastHit2D hit4;

    public GameObject person;

    private void Awake(){
        lr = GetComponent<LineRenderer>();
        lr.generateLightingData = true;
    }

    private void FixedUpdate(){
        // Rayを飛ばす
        hit1 = DrawRaycast(transform.position, SceneBase.angleValueArray[0]);   // デフォルト右向き
        if(hit1){
            float x = transform.position.x+(hit1.point.x-transform.position.x)*2;
            float y = hit1.point.y - transform.position.y;
            float hit1Angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            hit1Angle = hit1Angle - 180f + hit1Angle;
            hit2 = DrawRaycast(hit1.transform.position, hit1Angle); // デフォルト下向き
        }
        if(hit2){
            hit3 = DrawRaycast(hit2.transform.position, -SceneBase.angleValueArray[1] + 180f); // デフォルト左向き
        }
        if(hit3){
            hit4 = DrawRaycast(hit3.transform.position, SceneBase.angleValueArray[2]);  // デフォルト右向き
        }
    }

    private RaycastHit2D DrawRaycast(Vector2 from, float angle){
        if(angle != 360) Debug.DrawRay(from, new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)) * 100, Color.blue, 1);
        return Physics2D.Raycast(from, new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)));
    }

    public IEnumerator LightUp(){
        Debug.Log(hit1.collider.name);
        Debug.Log(hit2.collider.name);
        Debug.Log(hit3.collider.name);
        Debug.Log(hit4.collider.name);
        int hitCount = 0;
        if(hit1.collider.name == "Mirror") hitCount += 1;
        if(hit2.collider.name == "Mirror1") hitCount += 1;
        if(hit3.collider.name == "Mirror2") hitCount += 1;
        // if(hit4.collider.name == "HitPlace") hitCount += 1;

        // 徐々に光が伸びていく
        Vector3[] linePointArray = new Vector3[]
        {
            transform.position,
            hit1.point,
            hit2.point,
            hit3.point,
            hit4.point
        };
        DG.Tweening.Tween[] tweenArray = new DG.Tweening.Tween[hitCount+1];
        // 初期設定
        lr.SetPosition(0, linePointArray[0]);
        lr.positionCount += 1;
        int lineNum = 1;
        lr.SetPosition(lineNum, linePointArray[0]);
        // 線を伸ばす
        for (int i = 0; i < tweenArray.Length; ++i){
            int index = i;  // なぜかtweenの中ではインクリメントが使用できなかったので新しくint型変数を作成
            tweenArray[i] = AppUtil.DOTO(
                () => linePointArray[index],
                v =>
                {
                    linePointArray[index] = v;
                    lr.SetPosition(lineNum, linePointArray[index]);
                },
                linePointArray[i+1],
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
        AppUtil.DOSequence(tweenArray, 0f, 0f, 1);

        if(lineNum == 4) Debug.Log("goal");
        yield break;
    }
}
