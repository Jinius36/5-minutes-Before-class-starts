using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Student : MonoBehaviour
{
    public int sex; // ���� ��: 0, ��: 1
    public int weight; // ������
    public int nowFloor; // ���� ��ġ�� ��, -1�� ���������� ž��
    public int goalFloor; // ��ǥ ��
    public int orderPlace; // ���ִ� ��ġ

    Vector3 mousepoz;

    
    Canvas canvas;
    TextMeshProUGUI text;

    bool isDragging = false;
    [SerializeField] bool isOnElevator = false;
    [SerializeField] bool isAtOutside = true;
    public static bool isOnSetting = false;

    #region �巡�� �� ���
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
            || transform.position.y >= 3.0f || transform.position.y <= -5.0f) // �ʹ� ����ٸ� ����ġ
        {
            if(orderPlace>2)
                canvas.gameObject.SetActive(false);
            ResetPlace();
            return;
        }
        else if(transform.position.x > -1.0f && transform.position.x <= -0.175f) // x ��ġ ����
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

        if (isAtOutside) // �ۿ��� ���ٸ�
        {
            if (goalFloor == GameManager.Instance.floor) // ���ϴ� ���� ������ ��Ȳ
            {
                GameManager.Instance.check_Place[orderPlace] = false;
                UIManager.Instance.addAttend();
                Destroy(gameObject);
            }

            //if (!GameManager.Instance.check_Out[nowFloor,p]) // ���� ���� ���ϴ� ���� �ƴϰ� �� �� �߿� �� �ڸ��� ��ġ��
            //{
            //    nowFloor = GameManager.Instance.floor;
            //    transform.position = GameManager.Instance.place[p];
            //    if(orderPlace > 2) // ���������Ϳ��� ������ ��ġ�� ���
            //        GameManager.Instance.check_Place[orderPlace] = false;
            //    else // �ۿ��� ������ ��ġ�� ���
            //        GameManager.Instance.check_Out[nowFloor, orderPlace] = false;
            //    orderPlace = p;
            //    GameManager.Instance.check_Out[nowFloor, p] = true;
            //}

            else // ���� ���� ���ϴ� ���� �ƴ�
            {
                ResetPlace();
                if (!isOnSetting && orderPlace > 2)
                    canvas.gameObject.SetActive(false);
                return;
            }
        }
        else // ���������Ϳ� ���ٸ�
        {
            if(transform.position.y > -2.65f) 
            {

                p += 3;
                if (!GameManager.Instance.check_Place[p] 
                    && GameManager.Instance.totalWeight + weight < GameManager.Instance.maxWeight)
                {
                    transform.position = GameManager.Instance.place[p];
                    if (orderPlace > 2) // ���������Ϳ��� ���������ͷ� ��ġ�� ���
                        GameManager.Instance.check_Place[orderPlace] = false;
                    else // �ۿ��� ���������Ϳ� ��ġ�� ���
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
                    if (orderPlace > 2) // ���������Ϳ��� ���������ͷ� ��ġ�� ���
                        GameManager.Instance.check_Place[orderPlace] = false;
                    else // �ۿ��� ���������Ϳ� ��ġ�� ���
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
