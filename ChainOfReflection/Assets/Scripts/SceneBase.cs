using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    private void Awake(){
        AppUtil.InitTween();
    }
}
