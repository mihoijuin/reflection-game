using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLine : MonoBehaviour
{

    private DG.Tweening.Sequence lightSequence;
    Transform lightObject;

    private bool isLightingMode = false;
    private float firstAngle;
    private LineRenderer lr;


    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.generateLightingData = true;

        lightObject = transform.parent;
        lightSequence = AppUtil.DOSequence(
            new DG.Tweening.Tween[] {
                AppUtil.Rotate(lightObject, new Vector3(0f, 0f, 90f), 2f),
                AppUtil.Rotate(lightObject, new Vector3(0f, 0f, -90f), 2f)
            },
            0f,
            0f,
            -1
        );
        AppUtil.SetOnKillCallback(lightSequence, ()=> isLightingMode = true);

    }

    private void Update(){
        if(Input.GetMouseButtonDown(0)){
            AppUtil.KillDO(lightSequence, false);
        }

        // Rayを飛ばす
        if(isLightingMode){
            firstAngle = lightObject.localEulerAngles.z;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(firstAngle*Mathf.Deg2Rad), Mathf.Sin(firstAngle*Mathf.Deg2Rad)));
            if(hit){
                Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(firstAngle*Mathf.Deg2Rad), Mathf.Sin(firstAngle*Mathf.Deg2Rad)) * 100, Color.blue, 1);
                lr.positionCount = 2;
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, hit.point);
            }
        }
    }

}
