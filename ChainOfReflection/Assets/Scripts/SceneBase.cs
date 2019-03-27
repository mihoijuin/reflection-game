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
    private bool isGoalDebugMode = false;
    [SerializeField]
    private int debugLightNum = 0;
    [SerializeField]
    float[] clearAngleArray = null;
    [SerializeField]
    float[] clearPosArray = null;

    private LightLine lightLine;
    private Angle[] angleArray;

    public static float[] angleValueArray { get; private set; }

    private string[] moveObjectArray = {"Light", "Person2", "Mirror1", "Mirror2"};
    // private string[] directionArray = {"右", "下", "左", "右"};
    // private string[][] moveDataArray;

    private DG.Tweening.Sequence angleSequence = null;

    public static int decideTime { get; private set; }

    private void Awake(){
        // データ
        // LoadData();

        // 角度を取るオブジェクトの個数分の長さの配列を作成
        int angleObjectCount = 0;
        foreach(string moveObject in moveObjectArray){
            if(moveObject.StartsWith("Person")) continue;
            angleObjectCount += 1;
        }
        angleValueArray = new float[angleObjectCount];

        angleArray = FindObjectsOfType<Angle>();
        lightLine = FindObjectOfType<LightLine>();
        if(isGoalDebugMode){
            foreach(Angle angle in angleArray){
                angle.enabled = false;
            }
            SetClearRoot();
        }

        AppUtil.InitTween();
    }

    private void Start(){
        if(!isGoalDebugMode){
            for(int i=0; i<angleValueArray.Length; ++i){
                angleValueArray[i] = 360f;
            }
            decideTime = angleArray.Length;
            StartCoroutine(DesideAngleAndPos());
        }
    }

    private void Update(){
        if(Input.GetKey(KeyCode.Space)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // private void LoadData(){
    //     moveDataArray = new string[moveObjectArray.Length][];
    //     for(int i=0; i<moveDataArray.Length; ++i){
    //         moveDataArray[i] = new string[] {moveObjectArray[i], directionArray[i]};
    //     }
    // }

    private IEnumerator DesideAngleAndPos(){
        foreach(string moveObject in moveObjectArray){
            Angle moveTarget = null;
            foreach(Angle angle in angleArray){
                if(angle.name == moveObject) {
                    moveTarget = angle;
                }
            }
            if(moveTarget.name.StartsWith("Person")){
                SlidePos(moveTarget.gameObject);
            } else
            {
                RotateAngle(moveTarget.gameObject);
            }
            yield return new WaitUntil(()=> Input.GetMouseButtonDown(0));
            AppUtil.KillDO(angleSequence, false);
            yield return new WaitForSeconds(0.1f);
        }

        // T全てのオブジェクトの角度決定後に角度を取得する
        // moveObjectArrayをforeachして回転のものだけangleを参照して入れていく
        int angleObjectCount = 0;
        foreach(string moveObject in moveObjectArray){
            if(!moveObject.StartsWith("Person")){
                Angle target = null;
                foreach(Angle angle in angleArray){
                    if(angle.name == moveObject) {
                        target = angle;
                        break;
                    }
                }
                angleValueArray[angleObjectCount] = target.transform.eulerAngles.z;
                angleObjectCount += 1;
            }
        }
        Invoke("StartLighting", 3f);   // LightLineのhitに値が入るのを待機
    }

    public void RotateAngle(GameObject moveObject){
        angleSequence = AppUtil.DOSequence(
            new DG.Tweening.Tween[] {
                AppUtil.Rotate(moveObject.transform, new Vector3(0f, 0f, 90f), 2f),
                AppUtil.Rotate(moveObject.transform, new Vector3(0f, 0f, -90f), 2f)
            },
            0f,
            0f,
            -1
        );
    }

    public void SlidePos(GameObject moveObject){
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

    private void SetClearRoot(){
        int posIndexCount = 0;
        int angleIndexCount = 0;
        for(int i=0; i<debugLightNum; i++){
            if(moveObjectArray[i].StartsWith("Person")){
                GameObject moveObject = GameObject.Find(moveObjectArray[i]);
                moveObject.transform.position = new Vector3(clearPosArray[posIndexCount], moveObject.transform.position.y, moveObject.transform.position.z);
                posIndexCount += 1;
            } else
            {
                GameObject.Find(moveObjectArray[i]).transform.eulerAngles = new Vector3(0f, 0f, clearAngleArray[angleIndexCount]);
                angleValueArray[angleIndexCount] = clearAngleArray[angleIndexCount];
                angleIndexCount += 1;
            }
        }


        Invoke("StartLighting", 3f);   // LightLineのhitに値が入るのを待機
    }
    private void StartLighting(){
        StartCoroutine(lightLine.LightUp());
    }


}

