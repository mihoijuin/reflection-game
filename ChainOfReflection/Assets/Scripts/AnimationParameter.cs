using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Animation Parameter/Create ParameterTable", fileName = "ParameterTable" )]
public class AnimationParameter : ScriptableObject
{
    public List<ParameterTable> ParameterList = new List<ParameterTable>();
}


 [System.Serializable]
public class ParameterTable {

   //設定したいデータの変数
   public string UseTarget = "対象オブジェクト";
   public float  Delay = 0f, Value = 0f, Duration = 0f;
   public string EaseType = "OutQuad";

}