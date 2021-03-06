﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Motion : MonoBehaviour
{

    [SerializeField]
    private float[] clearAngleArray = null;
    [SerializeField]
    private float[] clearPosArray = null;

    // データ
    public static float[] angleValueArray { get; private set; }
    private string[][] moveDataArray;

    private DG.Tweening.Sequence angleSequence = null;

    private bool isTapped = false;

    private void Start(){
        moveDataArray = GameDirector.moveDataArray;
        angleValueArray = new float[moveDataArray.Length];
        for(int i=0; i<angleValueArray.Length; ++i){    // 角度の値を初期化
            angleValueArray[i] = 360f;
        }
    }


    public IEnumerator DesideAngleAndPos(){
        Transform focusParticle =  GameObject.Find(moveDataArray[0][0]).transform.Find("SelectedParticle");
        foreach(string[] dataArray in moveDataArray){
            string name = dataArray[0];
            string[] moveTarget = dataArray[2].Split('、');
            GameObject reflection = GameObject.Find(name);
            if(moveTarget.Contains("なし")) continue;     // 動かす対象がないものはスキップ
            // 注目切り替え
            yield return AppUtil.WaitDO(AppUtil.Move(focusParticle, focusParticle.position, reflection.transform.position, 1f, "OutQuart"));
            focusParticle.gameObject.SetActive(false);
            focusParticle = reflection.transform.Find("SelectedParticle");
            focusParticle.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            // 動かす
            if(moveTarget.Contains("足場")){    // 足場を動かしたあとに鏡を回転
                SlidePos(reflection);
                yield return WaitUntilTap();
                AppUtil.KillDO(angleSequence, false);
                yield return new WaitForSeconds(0.1f);
                RotateAngle(reflection);
            } else
            {
                RotateAngle(reflection);
            }
            yield return WaitUntilTap();
            AppUtil.KillDO(angleSequence, false);
            yield return new WaitForSeconds(0.5f);  // Kill終了まで待機
        }

        focusParticle.gameObject.SetActive(false);
        SetAngles();    // 全てのオブジェクトの角度決定後に角度を取得する
    }

    private void RotateAngle(GameObject moveObject){
        angleSequence = AppUtil.DOSequence(
            new DG.Tweening.Tween[] {
                AppUtil.Rotate(moveObject.transform, new Vector3(0f, 0f, 70f), 2f, "OutQuart"),
                AppUtil.Rotate(moveObject.transform, new Vector3(0f, 0f, -70f), 2f, "OutQuart")
            },
            0f,
            0f,
            -1
        );
    }

    private void SlidePos(GameObject moveObject){
        moveObject = moveObject.transform.parent.gameObject;
        Vector3 originPos = moveObject.transform.position;
        Vector3 targetPos = new Vector3(originPos.x-10f, originPos.y, originPos.z);
        angleSequence = AppUtil.DOSequence(
            new DG.Tweening.Tween[] {
                AppUtil.Move(moveObject.transform, moveObject.transform.position, targetPos, 3f),
                AppUtil.Move(moveObject.transform, moveObject.transform.position, originPos, 3f),
            },
            0f,
            0f,
            -1
        );
    }

    private void SetAngles(){
        foreach(string[] dataArray in moveDataArray){
            int index = Array.IndexOf(moveDataArray, dataArray);
            string name = dataArray[0];
            string direction = dataArray[1];
            string[] moveTarget = dataArray[2].Split('、');
            float targetAngle = GameObject.Find(name).transform.eulerAngles.z;
            switch(direction){
                case "右":
                angleValueArray[index] = targetAngle;
                break;
                case "左":
                angleValueArray[index] = targetAngle+180f;
                break;
                case "下":
                // 入射角から反射角を計算
                Vector3 prevPos = GameObject.Find(moveDataArray[index-1][0]).transform.position;
                Vector3 currentPos = GameObject.Find(name).transform.position;
                float x = prevPos.x+(currentPos.x-prevPos.x)*2;
                float y = currentPos.y - prevPos.y;
                float hitAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
                angleValueArray[index] = hitAngle - 180f + hitAngle;
                break;
            }
        }
    }

    public void SetClearRoot(){
        int angleIndexCount = 0;
        int posIndexCount = 0;
        foreach(string[] dataArray in moveDataArray){
            if(dataArray[2].Split('、').Contains("本体")){  // 角度回転あり
                GameObject.Find(dataArray[0]).transform.eulerAngles = new Vector3(0f, 0f, clearAngleArray[angleIndexCount]);
                angleValueArray[angleIndexCount] = clearAngleArray[angleIndexCount];
                angleIndexCount += 1;
            }
            if(dataArray[2].Split('、').Contains("足場")){  // 足場移動あり
                GameObject moveObject = GameObject.Find(dataArray[0]).transform.parent.gameObject;
                moveObject.transform.position = new Vector3(clearPosArray[posIndexCount], moveObject.transform.position.y, moveObject.transform.position.z);
                posIndexCount += 1;
            }
        }

        SetAngles();
    }


    private IEnumerator WaitUntilTap(){
        yield return new WaitUntil(()=>isTapped);
        isTapped = false;
    }

    public void OnClick(){
        isTapped = true;
    }

}

