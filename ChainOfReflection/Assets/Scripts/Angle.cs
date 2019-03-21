using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angle : MonoBehaviour
{


    private string currentTurn;

    private DG.Tweening.Sequence angleSequence;

    private bool isLightingMode = false;
    public float firstAngle { get; private set; }
    public float secondAngle { get; private set; }
    public float thirdAngle { get; private set; }
    // private LineRenderer lr;


    private void Start()
    {
        currentTurn = "Light";
        RotateAngle();


        // lr = GetComponent<LineRenderer>();
        // lr.generateLightingData = true;

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
                break;
            }
        }

        // Rayを飛ばす
        // if(isLightingMode){
        //     firstAngle = lightObject.localEulerAngles.z;
        //     RaycastHit2D hit1 = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(firstAngle*Mathf.Deg2Rad), Mathf.Sin(firstAngle*Mathf.Deg2Rad)));
        //     if(hit1){
        //         Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(firstAngle*Mathf.Deg2Rad), Mathf.Sin(firstAngle*Mathf.Deg2Rad)) * 100, Color.blue, 1);
        //         lr.positionCount = 2;
        //         lr.SetPosition(0, transform.position);
        //         lr.SetPosition(1, hit1.point);
        //     }
        // }
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
