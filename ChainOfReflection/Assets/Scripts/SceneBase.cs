#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneBase : MonoBehaviour
{
    [SerializeField]
    private float clearAngle1 = 0f;
    [SerializeField]
    private float clearPos = 0f;
    [SerializeField]
    private float clearAngle2 = 0f;
    [SerializeField]
    private float clearAngle3 = 0f;

    private LightLine lightLine;

    private void Awake(){
#if (DEBUG)
        foreach(Angle angle in FindObjectsOfType<Angle>()){
            angle.enabled = false;
        }
        lightLine = FindObjectOfType<LightLine>();
        SetClearRoot();
#endif
        AppUtil.InitTween();
    }

    private void SetClearRoot(){
        GameObject firstObject = GameObject.Find("Light");
        GameObject secondObject = GameObject.Find("Person2");
        GameObject thirdObject = GameObject.Find("Mirror1");
        GameObject forthObject = GameObject.Find("Mirror2");

        firstObject.transform.eulerAngles = new Vector3(0f, 0f, clearAngle1);
        Angle.firstAngle = clearAngle1;
        secondObject.transform.position = new Vector3(clearPos, secondObject.transform.position.y, secondObject.transform.position.z);
        thirdObject.transform.eulerAngles = new Vector3(0f, 0f, -clearAngle2);
        Angle.secondAngle = clearAngle2;
        forthObject.transform.eulerAngles = new Vector3(0f, 0f, clearAngle3);
        Angle.thirdAngle = clearAngle3;

        Invoke("StartDebug", 3f);
    }
    private void StartDebug(){
        StartCoroutine(lightLine.LightUp());
    }
}
