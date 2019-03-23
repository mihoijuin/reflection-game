using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angle : MonoBehaviour
{


    private string currentTurn;

    private DG.Tweening.Sequence angleSequence = null;

    public static float firstAngle { get; set; }
    public static float secondAngle { get; set; }
    public static float thirdAngle { get; set; }

    private LightLine lightLine;

    private void Awake(){
        lightLine = FindObjectOfType<LightLine>();
    }

    private void Start()
    {
        currentTurn = "Light";
        firstAngle = 360;
        secondAngle = 360;
        thirdAngle = 360;
        RotateAngle();

    }

    private void Update(){
        if(Input.GetMouseButtonDown(0)){
            AppUtil.KillDO(angleSequence, false);
            switch(currentTurn){
                case "Light":
                firstAngle = transform.localEulerAngles.z;
                currentTurn = "Person2";
                SlidePos();
                break;
                case "Person2":
                currentTurn = "Mirror1";
                RotateAngle();
                break;
                case "Mirror1":
                secondAngle = transform.localEulerAngles.z;
                currentTurn = "Mirror2";
                RotateAngle();
                break;
                case "Mirror2":
                thirdAngle = transform.localEulerAngles.z;
                currentTurn = null;
                StartCoroutine(lightLine.LightUp());
                break;
            }
        }
    }

    private void RotateAngle(){
        if(gameObject.name == currentTurn){
            angleSequence = AppUtil.DOSequence(
                new DG.Tweening.Tween[] {
                    AppUtil.Rotate(transform, new Vector3(0f, 0f, 90f), 2f),
                    AppUtil.Rotate(transform, new Vector3(0f, 0f, -90f), 2f)
                },
                0f,
                0f,
                -1
            );
        }
    }

    private void SlidePos(){
        if(gameObject.name == currentTurn){
            Vector3 originPos = transform.position;
            Vector3 targetPos = new Vector3(originPos.x-10f, originPos.y, originPos.z);
            angleSequence = AppUtil.DOSequence(
                new DG.Tweening.Tween[] {
                    AppUtil.Move(transform, targetPos, 3f),
                    AppUtil.Move(transform, originPos, 3f),
                },
                0f,
                0f,
                -1
            );
        }
    }

}
