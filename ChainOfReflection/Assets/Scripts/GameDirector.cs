using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    // アタッチするオブジェクト
    [SerializeField]
    private GameObject appPrefab = null;

    // オープニングパラメータ
    [SerializeField]
    private AnimationParameter animationParameter = null;
    private List<ParameterTable> opParameter = null;

    // オープニングオブジェクト
    [SerializeField]
    private RectTransform opLight = null;
    [SerializeField]
    private RectTransform opCover = null;
    [SerializeField]
    private Text opStartMessage = null;
    [SerializeField]
    private CanvasGroup opGroup = null;

    // ゴールデバッグ用
    [SerializeField]
    private bool _isGoalDebugMode = false;
    public static bool isGoalDebugMode;
    [SerializeField]
    private int _debugLightNum = 0;
    public static int debugLightNum;

    // データ
    public static string[][] moveDataArray { get; private set; }

    private Motion motion;
    private LightLine lightLine;

    private void Awake(){
        if(!App.isInitialized){
            isGoalDebugMode = _isGoalDebugMode;
            debugLightNum = _debugLightNum;

            GameObject app = Instantiate(appPrefab) as GameObject;
            app.AddComponent(typeof(App));
            DontDestroyOnLoad(app);

            App.isInitialized = true;
        }

        moveDataArray = LoadData();
        motion = FindObjectOfType<Motion>();
        lightLine = FindObjectOfType<LightLine>();
        opParameter = animationParameter.ParameterList;
        AppUtil.InitTween();

    }

    private void Start(){
        StartCoroutine(StartGame());
    }

    private string[][] LoadData(){
        var data =
            from x in Resources.Load("TextData").ToString().Split('\n').Skip(1)
            select x.Split(',');
        return data.ToArray();
    }

    private void Update(){
        if(Input.GetKey(KeyCode.Space)){
            RestartGame();
        }
    }

    private IEnumerator StartGame() {
        if(GameDirector.isGoalDebugMode){
            motion.SetClearRoot();
            yield return new WaitForSeconds(2f);    // LightLineにてhitに値が入るのを待つ
            StartLighting();
            yield break;
        }

        if(!App.isGameMode){
            yield return PlayOpening();
            App.isGameMode = true;
        }
        yield return motion.DesideAngleAndPos();
        yield return new WaitForSeconds(2f);    // LightLineにてhitに値が入るのを待つ
        StartLighting();
    }

    private IEnumerator PlayOpening(){
        opGroup.gameObject.SetActive(true);
        float delay = opParameter.Find(x=> x.UseTarget=="ライト降下").Delay;
        float value = opParameter.Find(x=> x.UseTarget=="ライト降下").Value;
        float duration = opParameter.Find(x=> x.UseTarget=="ライト降下").Duration;
        string easeType = opParameter.Find(x=> x.UseTarget=="ライト降下").EaseType;
        yield return AppUtil.WaitDO(AppUtil.Move(opLight, new Vector2(opLight.anchoredPosition.x, value), opLight.anchoredPosition, duration, easeType, delay));

        delay = opParameter.Find(x=> x.UseTarget=="ライト向き変更").Delay;
        value = opParameter.Find(x=> x.UseTarget=="ライト向き変更").Value;
        duration = opParameter.Find(x=> x.UseTarget=="ライト向き変更").Duration;
        easeType = opParameter.Find(x=> x.UseTarget=="ライト向き変更").EaseType;
        yield return AppUtil.WaitDO(AppUtil.Scale(opLight, new Vector2(value, 1), duration, easeType, delay));

        delay = opParameter.Find(x=> x.UseTarget=="タイトル表示").Delay;
        value = opParameter.Find(x=> x.UseTarget=="タイトル表示").Value;
        duration = opParameter.Find(x=> x.UseTarget=="タイトル表示").Duration;
        easeType = opParameter.Find(x=> x.UseTarget=="タイトル表示").EaseType;
        yield return AppUtil.WaitDO(AppUtil.Move(opCover, opCover.anchoredPosition, new Vector2(value, opCover.anchoredPosition.y), duration, easeType, delay));

        delay = opParameter.Find(x=> x.UseTarget=="Tap to start表示暗").Delay;
        value = opParameter.Find(x=> x.UseTarget=="Tap to start表示暗").Value;
        duration = opParameter.Find(x=> x.UseTarget=="Tap to start表示暗").Duration;
        easeType = opParameter.Find(x=> x.UseTarget=="Tap to start表示暗").EaseType;
        float delay_2 = opParameter.Find(x=> x.UseTarget=="Tap to start表示明").Delay;
        float value_2 = opParameter.Find(x=> x.UseTarget=="Tap to start表示明").Value;
        float duration_2 = opParameter.Find(x=> x.UseTarget=="Tap to start表示明").Duration;
        string easeType_2 = opParameter.Find(x=> x.UseTarget=="Tap to start表示明").EaseType;
        yield return new WaitForSeconds(delay);
        AppUtil.Blink(opStartMessage, 50, value, value_2, duration, duration_2, easeType, easeType_2, delay_2);

        yield return new WaitUntil(()=> Input.GetMouseButtonDown(0));

        delay = opParameter.Find(x=> x.UseTarget=="スタート画面非表示").Delay;
        value = opParameter.Find(x=> x.UseTarget=="スタート画面非表示").Value;
        duration = opParameter.Find(x=> x.UseTarget=="スタート画面非表示").Duration;
        easeType = opParameter.Find(x=> x.UseTarget=="スタート画面非表示").EaseType;
        yield return AppUtil.WaitDO(AppUtil.FadeOut(opGroup, value, duration, easeType, delay));
        opGroup.gameObject.SetActive(false);
    }


    private void StartLighting(){
        StartCoroutine(lightLine.LightUp());
    }

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
