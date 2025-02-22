﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtalPen : MonoBehaviour
{
    private int isCableCar;
    private GameObject clone;
    private LineRenderer line;
	public LineRenderer cableline = null;
    private int i;
    public GameObject obs;
    public float lineWidth = 0.05f;
    public float precision = 0.05f;
    public List<Vector3> posList = new List<Vector3>();
    public List<Vector3> portalList = new List<Vector3>();
    private bool isPortalOpen = false;
    public float minDoorHeight = 0.1f;
    public float minDoorWidth = 0.1f;
    private float characterHeight;
    private float characterWidth;
    private float closeCanvasDis = 1f;
	public int cableProtalID = -1;
    private GameObject player;
    public KeyCode protalKey = KeyCode.Q;
    private bool open = false;

    int sign(float x)
    {
        return (x < 0) ? (-1) : (1);
    }

    private enum vec3 { top, bottom, left, right, center }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        characterWidth = player.GetComponent<BoxCollider2D>().size.x;
        characterHeight = player.GetComponent<BoxCollider2D>().size.y;
        portalList.Clear();
    }
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (portalList.Count >= 2)
            {
                foreach (GameObject x in GameObject.FindGameObjectsWithTag("clonedLine"))
                    Destroy(x);
                foreach (GameObject x in GameObject.FindGameObjectsWithTag("CableCarClone"))
                    if (x.name != "BlackHoldLine(Clone)")
                        Destroy(x);
                portalList.Clear();
            }
            clone = (GameObject)Instantiate(obs, obs.transform.position, transform.rotation);//克隆一个带有LineRender的物体   
            line = clone.GetComponent<LineRenderer>();//获得该物体上的LineRender组件
            line.tag = "clonedLine"; 
            line.SetColors(new Color32(255, 160, 120, 50), new Color32(255, 160, 120, 50));//设置颜色  
            line.SetWidth(lineWidth, lineWidth);//设置宽度
            i = 0;
            posList.Clear();
            isPortalOpen = false;
            open = false;
            isCableCar = 0;
        }
        else if (Input.GetMouseButton(0)&& MousePositionDetection() == 0)
        {
            if (clone != null)
                Destroy(clone);
        }
        else if (Input.GetMouseButton(0) && MousePositionDetection() != 0 && !open)
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
        else if (Input.GetMouseButtonUp(0) && (isCableCar = MousePositionDetection()) != 0)
        {
            if (clone != null)
            {
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
                IsOpen(posList);
                if (isPortalOpen)
                {
                    if ((topPoint.y - bottomPoint.y) < minDoorHeight || (rightPoint.x - leftPoint.x) < minDoorWidth)
                    {
                        //Debug.Log("太小了");
                        Destroy(clone);

                    }
                    else
                    {
                        if (isCableCar == 2)
                        {
                            clone.tag = "CableCarClone";
                            cableline = line;
							cableProtalID = portalList.Count;
                        }
                        Debug.Log("传送门" + centerPoint);
                        portalList.Add(centerPoint);
                        open = true;
                    }
                }
                else
                {
                    Destroy(clone);
                }
            }
        }
        else if(!open) Destroy(clone);
        if (portalList.Count == 2)
        {
            Portal(portalList[0], portalList[1]);
        }

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
    private void IsOpen(List<Vector3> list)
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
            isPortalOpen = true;
    }
    private void Portal(Vector3 from, Vector3 to)
    {
        if (((player.transform.position - from).sqrMagnitude < 0.5f) && Input.GetKeyDown(protalKey))
        {
            player.transform.position = to;
        }
        else if (((player.transform.position - to).sqrMagnitude < 0.5f)&&Input.GetKeyDown(protalKey))
        {
            player.transform.position = from;
        }
    }
    private int MousePositionDetection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            if (hit.collider.tag == "CableCar")
                return 2;
            else if (hit.collider.tag == "Canvas")
                return 1;
            else
                return 0;
        }
        else
            return 0;
    }
}
