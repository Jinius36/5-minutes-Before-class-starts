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
    public int weight; // 몸무게
    public int nowFloor; // 현재 위치한 층, -1은 엘리베이터 탑승
    public int goalFloor; // 목표 층
    public int orderPlace; // 서있는 위치

    Vector3 mousepoz;

    
    Canvas canvas;
    TextMeshProUGUI text;

    bool isDragging = false;
    [SerializeField] bool isOnElevator = false;
    [SerializeField] bool isAtOutside = true;
    public static bool isOnSetting = false;

    #region 드래그 앤 드롭
    private void OnMouseDown()
    {
        if (!isOnSetting)
        {
            isDragging = true;
            canvas.gameObject.SetActive(true);
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            mousepoz = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousepoz);
        }
    }

    private void ResetPlace()
    {
        transform.position = GameManager.Instance.place[orderPlace];
    }

    private void OnMouseUp()
    {
        isDragging = false;
        int p = 0;
        if(transform.position.x <= -1.0f || transform.position.x >= 1.8f 
            || transform.position.y >= 3.0f || transform.position.y <= -5.0f) // 너무 벗어났다면 원위치
        {
            if(orderPlace>2)
                canvas.gameObject.SetActive(false);
            ResetPlace();
            return;
        }
        else if(transform.position.x > -1.0f && transform.position.x <= -0.175f) // x 위치 판정
        {
            p = 0;
        }
        else if (transform.position.x > -0.175f && transform.position.x < 0.975f)
        {
            p = 1;
        }
        else if (transform.position.x >= 0.975f && transform.position.x < 1.8f)
        {
            p = 2;
        }

        if (isAtOutside) // 밖에다 놨다면
        {
            if (goalFloor == GameManager.Instance.floor) // 원하는 층에 도착한 상황
            {
                GameManager.Instance.check_Place[orderPlace] = false;
                UIManager.Instance.addAttend();
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
                    canvas.gameObject.SetActive(false);
                return;
            }
        }
        else // 엘리베이터에 놨다면
        {
            if(transform.position.y > -2.65f) 
            {

                p += 3;
                if (!GameManager.Instance.check_Place[p] 
                    && GameManager.Instance.totalWeight + weight < GameManager.Instance.maxWeight)
                {
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
                        canvas.gameObject.SetActive(false);
                }
                else
                {
                    ResetPlace();
                    if (!isOnSetting)
                        canvas.gameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                p += 6;
                if (!GameManager.Instance.check_Place[p] 
                    && GameManager.Instance.totalWeight + weight < GameManager.Instance.maxWeight)
                {
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
                        canvas.gameObject.SetActive(false);
                }
                else
                {
                    ResetPlace();
                    if (!isOnSetting)
                        canvas.gameObject.SetActive(false);
                    return;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isOnElevator = true;
        isAtOutside = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isOnElevator = false;
        isAtOutside = true;
    }
    #endregion

    void Start()
    {
        
        canvas = GetComponentInChildren<Canvas>();
        text = canvas.GetComponentInChildren<TextMeshProUGUI>();
        if (goalFloor == 0)
            text.text = "B2";
        if (goalFloor == 1)
            text.text = "L";
        if (goalFloor >= 2)
            text.text = $"{goalFloor - 1}";
    }
}
