using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneBase : MonoBehaviour
{

    // ゴールデバッグ用
    [SerializeField]
    private bool _isGoalDebugMode = false;
    public static bool isGoalDebugMode;
    [SerializeField]
    private int _debugLightNum = 0;
    public static int debugLightNum;
    [SerializeField]
    private float[] clearAngleArray = null;
    [SerializeField]
    private float[] clearPosArray = null;

    private LightLine lightLine;

    public static string[][] moveDataArray { get; private set; }
    public static float[] angleValueArray { get; private set; }

    private DG.Tweening.Sequence angleSequence = null;

    private void Awake(){
        // デバッグチェック
        isGoalDebugMode = _isGoalDebugMode;
        debugLightNum = _debugLightNum;

        moveDataArray = LoadData();
        angleValueArray = new float[moveDataArray.Length];
        lightLine = FindObjectOfType<LightLine>();

        AppUtil.InitTween();
    }

    private void Start(){
        if(isGoalDebugMode){
            SetClearRoot();
        } else
        {
            for(int i=0; i<angleValueArray.Length; ++i){
                angleValueArray[i] = 360f;
            }
            StartCoroutine(DesideAngleAndPos());
        }
    }

    private void Update(){
        if(Input.GetKey(KeyCode.Space)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private string[][] LoadData(){
        var data =
            from x in Resources.Load("TextData").ToString().Split('\n').Skip(1)
            select x.Split(',');
        return data.ToArray();
    }

    private IEnumerator DesideAngleAndPos(){
        foreach(string[] dataArray in moveDataArray){
            string name = dataArray[0];
            string[] moveTarget = dataArray[2].Split('、');
            if(moveTarget.Contains("なし")) continue;     // 動かす対象がないものはスキップ
            if(moveTarget.Contains("足場")){    // 足場を動かしたあとに鏡を回転
                SlidePos(GameObject.Find(name));
                yield return new WaitUntil(()=> Input.GetMouseButtonDown(0));
                AppUtil.KillDO(angleSequence, false);
                yield return new WaitForSeconds(0.1f);
                RotateAngle(GameObject.Find(name));
            } else
            {
                RotateAngle(GameObject.Find(name));
            }
            yield return new WaitUntil(()=> Input.GetMouseButtonDown(0));
            AppUtil.KillDO(angleSequence, false);
            yield return new WaitForSeconds(0.1f);  // Kill終了まで待機
        }

        SetAngles();    // 全てのオブジェクトの角度決定後に角度を取得する

        Invoke("StartLighting", 3f);   // LightLineのhitに値が入るのを待機
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
                AppUtil.Move(moveObject.transform, targetPos, 3f),
                AppUtil.Move(moveObject.transform, originPos, 3f),
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

    private void SetClearRoot(){
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
        Invoke("StartLighting", 3f);   // LightLineのhitに値が入るのを待機
    }
    private void StartLighting(){
        StartCoroutine(lightLine.LightUp());
    }


}

