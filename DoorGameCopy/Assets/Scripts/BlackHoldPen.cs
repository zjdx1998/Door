﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoldPen : MonoBehaviour
{
    private GameObject clone;
    private LineRenderer line;
    private int i;
    public GameObject obs;
    private List<Vector3> posList = new List<Vector3>();
    private bool isBlackHoleOpen = false;
    public float lineWidth = 0.05f;
    private float characterHeight;
    private float characterWidth;
    private GameObject player;
    private List<Vector3> bHList = new List<Vector3>();

    private enum vec3 { top, bottom, left, right, center }
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        characterWidth = player.GetComponent<BoxCollider2D>().size.x;
        characterHeight = player.GetComponent<BoxCollider2D>().size.y;
        bHList.Clear();
    }
    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clone = (GameObject)Instantiate(obs, obs.transform.position, transform.rotation);//克隆一个带有LineRender的物体   
            line = clone.GetComponent<LineRenderer>();//获得该物体上的LineRender组件  
            line.SetColors(Color.blue, Color.red);//设置颜色  
            line.SetWidth(lineWidth, lineWidth);//设置宽度  
            i = 0;
            posList.Clear();
            isBlackHoleOpen = false;
        }
        if (Input.GetMouseButton(0) && !MousePositionDetection())
        {
            if (clone != null)
                Destroy(clone);
        }
        if (Input.GetMouseButton(0) && MousePositionDetection())
        {
            if (clone != null)
            {
                i++;
                Vector3 pos = new Vector3();
                line.positionCount = i;//设置顶点数  
                pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15));
                line.SetPosition(i - 1, pos);//设置顶点位置
                if (posList.Count == 0)
                    posList.Add(pos);
                else if (pos != posList[posList.Count - 1])
                {
                    posList.Add(pos);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && MousePositionDetection())
        {
            if (clone != null)
            {

                IsBHOpen(posList);
                List<Vector3> changeList = new List<Vector3>();
                changeList = posList;
                Vector3 topPoint = new Vector3();
                Vector3 bottomPoint = new Vector3();
                Vector3 leftPoint = new Vector3();
                Vector3 rightPoint = new Vector3();
                Vector3 centerPoint = new Vector3();
                topPoint = posList[posList.Count - 1];
                bottomPoint = GetPoint(changeList, vec3.bottom);
                rightPoint = GetPoint(changeList, vec3.right);
                leftPoint = GetPoint(changeList, vec3.left);
                centerPoint = GetPoint(changeList, vec3.center);
                if (isBlackHoleOpen)
                {
                    Debug.Log("黑洞门");
                    bHList.Add(centerPoint);
                }
                else
                {
                    Destroy(clone);
                }
            }
        }

    }
    private void IsBHOpen(List<Vector3> list)
    {
        int xChangeCount = 0;
        int yChangeCount = 0;
        for (int n = 1; n < list.Count - 1; n++)
        {
            int q = n;
            while ((q < list.Count - 1) && ((list[q].y - list[q + 1].y) == 0))
            {
                q++;
            }
            if (q == list.Count - 1)
                ;
            else if ((list[n - 1].y - list[n].y) * (list[q].y - list[q + 1].y) < 0)
            {
                yChangeCount++;
            }
            q = n;
            while ((q < list.Count - 1) && ((list[q].x - list[q + 1].x) == 0))
            {
                q++;
            }
            if (q == list.Count - 1)
                ;
            else if ((list[n - 1].x - list[n].x) * (list[q].x - list[q + 1].x) < 0)
                xChangeCount++;
        }
        if (xChangeCount >= 2 && yChangeCount >= 2)
            isBlackHoleOpen = true;
    }
    private Vector3 GetPoint(List<Vector3> list, vec3 vec)
    {

        if (vec.ToString() == "top")
        {
            for (int m = 0; m < list.Count - 1; m++)
            {
                if (list[m].y > list[m + 1].y)
                {
                    Vector3 q = new Vector3();
                    q = list[m];
                    list[m] = list[m + 1];
                    list[m + 1] = q;
                }
            }
        }
        if (vec.ToString() == "bottom")
        {
            for (int m = 0; m < list.Count - 1; m++)
            {
                if (list[m].y < list[m + 1].y)
                {
                    Vector3 q = new Vector3();
                    q = list[m];
                    list[m] = list[m + 1];
                    list[m + 1] = q;
                }
            }
        }
        if (vec.ToString() == "left")
        {
            for (int m = 0; m < list.Count - 1; m++)
            {
                if (list[m].x < list[m + 1].x)
                {
                    Vector3 q = new Vector3();
                    q = list[m];
                    list[m] = list[m + 1];
                    list[m + 1] = q;
                }
            }
        }
        if (vec.ToString() == "right")
        {
            for (int m = 0; m < list.Count - 1; m++)
            {
                if (list[m].x > list[m + 1].x)
                {
                    Vector3 q = new Vector3();
                    q = list[m];
                    list[m] = list[m + 1];
                    list[m + 1] = q;
                }
            }
        }
        if (vec.ToString() == "center")
        {
            Vector3 t = new Vector3();
            Vector3 b = new Vector3();
            Vector3 l = new Vector3();
            Vector3 r = new Vector3();
            t = GetPoint(list, vec3.top);
            b = GetPoint(list, vec3.bottom);
            l = GetPoint(list, vec3.left);
            r = GetPoint(list, vec3.right);
            float x = (l.x + r.x) / 2;
            float y = (t.y + b.y) / 2;

            return new Vector3(x, y, 0f);
        }
        return list[list.Count - 1];

    }
    private bool MousePositionDetection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            if (hit.collider.tag == "Canvas")
                return true;
            else
                return false;
        }
        else
            return false;
    }

}