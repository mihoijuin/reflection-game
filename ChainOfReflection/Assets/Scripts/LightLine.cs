using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLine : MonoBehaviour
{
    private LineRenderer lr;

    private RaycastHit2D hit1;
    private RaycastHit2D hit2;
    private RaycastHit2D hit3;
    private RaycastHit2D hit4;

    public GameObject person;

    private void Awake(){
        lr = GetComponent<LineRenderer>();
        lr.generateLightingData = true;
    }

    private void FixedUpdate(){
        // Rayを飛ばす
        hit1 = DrawRaycast(transform.position, Angle.firstAngle);   // デフォルト右向き
        if(hit1){
            float x = transform.position.x+(hit1.point.x-transform.position.x)*2;
            float y = hit1.point.y - transform.position.y;
            float hit1Angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            hit1Angle = hit1Angle - 180f + hit1Angle;
            hit2 = DrawRaycast(hit1.transform.position, hit1Angle); // デフォルト下向き
        }
        if(hit2){
            hit3 = DrawRaycast(hit2.transform.position, -Angle.secondAngle + 180f); // デフォルト左向き
        }
        if(hit3){
            hit4 = DrawRaycast(hit3.transform.position, Angle.thirdAngle);  // デフォルト右向き
        }
    }

    private RaycastHit2D DrawRaycast(Vector2 from, float angle){
        if(angle != 360) Debug.DrawRay(from, new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)) * 100, Color.blue, 1);
        return Physics2D.Raycast(from, new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad)));
    }

    public IEnumerator LightUp(){
        Debug.Log(hit1.collider.name);
        Debug.Log(hit2.collider.name);
        Debug.Log(hit3.collider.name);
        Debug.Log(hit4.collider.name);
        // Instantiate(person,  new Vector2(transform.position.x+(hit1.point.x-transform.position.x)*2, Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y), hit1.transform.rotation);
        int hitCount = 0;
        if(hit1.collider.name != "Mirror") hitCount += 1;
        if(hit2.collider.name != "Mirror1") hitCount += 1;
        if(hit3.collider.name != "Mirror2") hitCount += 1;
        if(hit4.collider.name != "HitPlace") hitCount += 1;

        int lineNum = hitCount + 2;

        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, hit1.point);
        if(hit1.collider.name != "Mirror") yield break;
        yield return new WaitForSeconds(1f);

        lr.positionCount = 3;
        // lr.SetPosition(2, new Vector2(transform.position.x+(hit1.point.x-transform.position.x)*2, Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y));
        lr.endWidth += 0.2f;
        lr.SetPosition(2, hit2.point);
        if(hit2.collider.name != "Mirror1") yield break;

        lr.positionCount = 4;
        lr.endWidth += 0.2f;
        lr.SetPosition(3, hit3.point);
        if(hit3.collider.name != "Mirror2") yield break;

        lr.positionCount = 5;
        lr.endWidth += 0.2f;
        lr.SetPosition(4, hit4.point);
        if(hit4.collider.name != "HitPlace") yield break;

        Debug.Log("goal");
        yield break;
    }
}
