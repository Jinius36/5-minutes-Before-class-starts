using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Student : MonoBehaviour
{
    public int sex; // 성별 남: 0, 여: 1
    public int nowFloor; // 현재 위치한 층, -1은 엘리베이터 탑승
    public int goalFloor; // 목표 층
    public int orderPlace; // 서있는 위치
    Canvas bubble;
    TextMeshProUGUI goalText;

    #region 드래그 앤 드롭
    Vector3 mousepoz;
    SpriteRenderer[] draglayer;
    bool isDragging = false;
    bool isAtOutside = true;
    public static bool isOnSetting = false;
    public static bool isElvMoving = false;
    private void ResetPlace()
    {
        transform.position = GameManager.Instance.place[orderPlace];
    }
    private void OnMouseDown()
    {
        if (!isOnSetting && !isElvMoving)
        {
            isDragging = true;
            bubble.gameObject.SetActive(true);
        }
    }
    private void OnMouseDrag()
    {
        for(int i=0; i<5; i++)
        {
            switch(i)
            {
                case 0:
                    draglayer[i].sortingLayerName = "DragHairBack";
                    break;
                case 1:
                    draglayer[i].sortingLayerName = "DragFace";
                    break;
                case 2:
                    draglayer[i].sortingLayerName = "DragHair";
                    break;
                case 3:
                    draglayer[i].sortingLayerName = "DragPants";
                    break;
                case 4:
                    draglayer[i].sortingLayerName = "DragTop";
                    break;
                default: break;
            }
        }
        bubble.sortingOrder = -1;
        if (isDragging)
        {
            mousepoz = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousepoz);
        }
    }
    private void OnMouseUp()
    {
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    draglayer[i].sortingLayerName = "HairBack";
                    break;
                case 1:
                    draglayer[i].sortingLayerName = "Face";
                    break;
                case 2:
                    draglayer[i].sortingLayerName = "Hair";
                    break;
                case 3:
                    draglayer[i].sortingLayerName = "Pants";
                    break;
                case 4:
                    draglayer[i].sortingLayerName = "Top";
                    break;
                default: break;
            }
        }
        bubble.sortingOrder = -2;
        isDragging = false;
        int p = 0;
        if (transform.position.x <= -1.0f || transform.position.x >= 1.8f 
            || transform.position.y >= 3.0f || transform.position.y <= -5.0f) // 너무 벗어났다면 원위치
        {
            if(orderPlace>2)
                bubble.gameObject.SetActive(false);
            ResetPlace();
            return;
        }
        else
        {
            switch (transform.position.x)
            {
                case var n when n > -1.0f && n <= -0.175f:
                    p = 0;
                    break;

                case var n when n > -0.175f && n < 0.975f:
                    p = 1;
                    break;

                case var n when n >= 0.975f && n < 1.8f:
                    p = 2;
                    break;
            }
        }
        if (isAtOutside) // 밖에다 놨다면
        {
            if (goalFloor == GameManager.Instance.floor) // 원하는 층에 도착한 상황
            {
                //GameManager.Instance.PlaySound(GameManager.Instance.sounds[(int)GameManager.soundList.StudentMove]);
                GameManager.Instance.check_Place[orderPlace] = false;
                UIManager.Instance.addAttend();
                if(GameManager.Instance.checkGoal())
                    GameManager.Instance.CallClear();
                Destroy(gameObject);
            }
            //if (!GameManager.Instance.check_Out[nowFloor,p]) // 내린 층이 원하는 층이 아니고 그 층 중에 빈 자리에 배치함
            //{
            //    nowFloor = GameManager.Instance.floor;
            //    transform.position = GameManager.Instance.place[p];
            //    if(orderPlace > 2) // 엘리베이터에서 밖으로 배치한 경우
            //        GameManager.Instance.check_Place[orderPlace] = false;
            //    else // 밖에서 밖으로 배치한 경우
            //        GameManager.Instance.check_Out[nowFloor, orderPlace] = false;
            //    orderPlace = p;
            //    GameManager.Instance.check_Out[nowFloor, p] = true;
            //}
            else // 내린 층이 원하는 층이 아님
            {
                ResetPlace();
                if (!isOnSetting && orderPlace > 2)
                    bubble.gameObject.SetActive(false);
                return;
            }
        }
        else // 엘리베이터에 놨다면
        {
            if(transform.position.y > -2.65f)
                p += 3;
            else
                p += 6;
            if (!GameManager.Instance.check_Place[p])
            {
                //GameManager.Instance.PlaySound(GameManager.Instance.sounds[(int)GameManager.soundList.StudentMove]);
                transform.position = GameManager.Instance.place[p];
                if (orderPlace > 2) // 엘리베이터에서 엘리베이터로 배치한 경우
                    GameManager.Instance.check_Place[orderPlace] = false;
                else // 밖에서 엘리베이터에 배치한 경우
                {
                    GameManager.Instance.check_Out[nowFloor, orderPlace] = false;
                    GameManager.Instance.checkExistence(nowFloor);
                }
                orderPlace = p;
                GameManager.Instance.check_Place[p] = true;
                nowFloor = -1;
                if (!isOnSetting)
                    bubble.gameObject.SetActive(false);
            }
            else
            {
                ResetPlace();
                if (!isOnSetting)
                    bubble.gameObject.SetActive(false);
                return;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAtOutside = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAtOutside = true;
    }
    #endregion

    void Start()
    {
        int i = 0;
        draglayer = new SpriteRenderer[6];
        foreach(Transform child in this.transform)
        {
            if(child.GetComponent<SpriteRenderer>() != null)
                draglayer[i] = child.GetComponent<SpriteRenderer>();
            i++;
        }
        bubble = GetComponentInChildren<Canvas>();
        goalText = bubble.GetComponentInChildren<TextMeshProUGUI>();
        if (goalFloor == 0)
            goalText.text = "B2";
        if (goalFloor == 1)
            goalText.text = "L";
        if (goalFloor >= 2)
            goalText.text = $"{goalFloor - 1}";
    }
}