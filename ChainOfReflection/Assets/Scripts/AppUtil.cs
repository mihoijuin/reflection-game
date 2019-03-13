using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AppUtil : MonoBehaviour
{
    public static void InitTween(){
        DOTween.useSmoothDeltaTime = true;
    }

    public static void DOSequence(Tween[] tweenArray, float prependInterval, float appendInterval){
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(prependInterval);
        sequence.AppendInterval(appendInterval);
        foreach(Tween tween in tweenArray){
            sequence.Append(tween);
        }
    }

    public static IEnumerator WaitForTween(Tween tween){
        yield return tween.WaitForCompletion();
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

    public static Tween MoveRect(RectTransform rect, string from, Vector2 startAnchoredPos, Vector2 endAnchoredPos, float duration, string ease, float delay=0f){
        /* from："上" */
        Ease easeType = (Ease)Enum.Parse(typeof(Ease), ease);
        rect.anchoredPosition = startAnchoredPos;
        Vector2  targetAnchoredPos = endAnchoredPos;
        Tween tween = rect.DOAnchorPos(targetAnchoredPos, duration).SetEase(easeType).SetDelay(delay);
        return tween;
    }
}
