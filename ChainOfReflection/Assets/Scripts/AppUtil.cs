using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AppUtil : MonoBehaviour
{
    public static void InitTween(){
        DOTween.useSmoothDeltaTime = true;
    }

    public static Sequence DOSequence(Tween[] tweenArray, float prependInterval, float appendInterval, int loopTime=1, TweenCallback callback=null){
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(prependInterval);
        sequence.AppendInterval(appendInterval);
        foreach(Tween tween in tweenArray){
            sequence.Append(tween);
        }
        sequence.SetLoops(loopTime);
        sequence.AppendCallback(callback);
        return sequence;
    }

    public static IEnumerator WaitDO(Tween tween){
        yield return tween.WaitForCompletion();
    }

    public static IEnumerator WaitDO(Sequence sequence){
        yield return sequence.WaitForCompletion();
    }


    public static void KillDO(Tween tween, bool complete=true){
        tween.Kill(complete);
    }

    public static void KillDO(Sequence sequence, bool complete=true){
        sequence.Kill(complete);
    }

    public static void SetOnCompleteCallback(Tween tween, TweenCallback action){
        tween.OnComplete(action);
    }

    public static void SetOnCompleteCallback(Sequence sequence, TweenCallback action){
        sequence.OnComplete(action);
    }

    public static void SetOnKillCallback(Tween tween, TweenCallback action){
        tween.OnKill(action);
    }

    public static void SetOnKillCallback(Sequence sequence, TweenCallback action){
        sequence.OnKill(action);
    }

    public static Tween DOTO(DG.Tweening.Core.DOGetter<Vector3> getter, DG.Tweening.Core.DOSetter<Vector3> setter, Vector3 endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = DOTween.To(getter, setter, endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween ShowRect(RectTransform rect, string changeValue, float startValue, float duration, string ease, float delay=0){
        /* changeValue："x", "y", "full" */
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        rect.gameObject.SetActive(true);
        Vector2 targetScale = rect.localScale;
        switch(changeValue){
            case "x":
            rect.localScale = new Vector2(rect.localScale.x * startValue, rect.localScale.y);
            break;
            case "y":
            rect.localScale = new Vector2(rect.localScale.x, rect.localScale.y * startValue);
            break;
            case "full":
            rect.localScale = new Vector2(rect.localScale.x * startValue, rect.localScale.y * startValue);
            break;
        }
        Tween tween = rect.DOScale(targetScale, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween HideRect(RectTransform rect, string changeValue, float duration, string ease, float delay=0f){
        /* changeValue："x", "y", "full" */
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        rect.gameObject.SetActive(true);
        Vector2 targetScale = Vector2.zero;
        switch(changeValue){
            case "x":
            targetScale = new Vector2(0f, rect.localScale.y);
            break;
            case "y":
            targetScale = new Vector2(rect.localScale.y, 0f);
            break;
        }
        Tween tween = rect.DOScale(targetScale, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween Scale(RectTransform rect, Vector2 endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = rect.DOScale(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween Move(RectTransform target, Vector2 startAnchoredPos, Vector2 endAnchoredPos, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        target.anchoredPosition = startAnchoredPos;
        Vector2  targetAnchoredPos = endAnchoredPos;
        Tween tween = target.DOAnchorPos(targetAnchoredPos, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween Move(Transform target, Vector2 startPos, Vector2 endPos, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        target.position = startPos;
        Vector2  targetAnchoredPos = endPos;
        Tween tween = target.DOMove(targetAnchoredPos, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween MoveTo(RectTransform target, Vector2 endPos, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = target.DOAnchorPos(endPos, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween MoveFrom(RectTransform target, Vector2 startPos, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Vector2  targetPos = target.anchoredPosition;
        target.anchoredPosition = startPos;
        Tween tween = target.DOAnchorPos(targetPos, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween Rotate(Transform target, Vector3 endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = target.DORotate(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween FadeIn(Image target, float endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        target.gameObject.SetActive(true);
        Tween tween = target.DOFade(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween FadeOut(Image target, float endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = target.DOFade(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween FadeIn(Text target, float endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        target.gameObject.SetActive(true);
        Tween tween = target.DOFade(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween FadeOut(Text target, float endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = target.DOFade(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween FadeIn(CanvasGroup target, float endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        target.gameObject.SetActive(true);
        Tween tween = target.DOFade(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Tween FadeOut(CanvasGroup target, float endValue, float duration, string ease="OutQuad", float delay=0f){
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        Tween tween = target.DOFade(endValue, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }

    public static Sequence Blink(Image target, int blinkTime, float darkAlpha, float blightAlpha, float darkDuration, float blightDuration, string darkEaseType, string blightEaseType, float prependInteval=0f, float appendInteval=0f){
        target.gameObject.SetActive(true);
        Tween[] tweenArray = new Tween[blinkTime*2];
        for(int i=0; i<blinkTime*2;  i+=2){
            tweenArray[i] = AppUtil.FadeOut(target, darkAlpha, darkDuration, darkEaseType); // dark
            tweenArray[i+1] = AppUtil.FadeIn(target, blightAlpha, blightDuration, blightEaseType); // blight
        }
        Sequence sequence = AppUtil.DOSequence(
            tweenArray,
            prependInteval,
            appendInteval
        );
        return sequence;
    }

    public static Sequence Blink(Text target, int blinkTime, float darkAlpha, float blightAlpha, float darkDuration, float blightDuration, string darkEaseType, string blightEaseType, float prependInteval=0f, float appendInteval=0f){
        target.gameObject.SetActive(true);
        Tween[] tweenArray = new Tween[blinkTime*2];
        for(int i=0; i<blinkTime*2;  i+=2){
            tweenArray[i] = AppUtil.FadeOut(target, darkAlpha, darkDuration, darkEaseType); // dark
            tweenArray[i+1] = AppUtil.FadeIn(target, blightAlpha, blightDuration, blightEaseType); // blight
        }
        Sequence sequence = AppUtil.DOSequence(
            tweenArray,
            prependInteval,
            appendInteval
        );
        return sequence;
    }

}
