using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : MonoBehaviour
{
    #region 싱글톤
    static Student instance;
    Student() { }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static Student Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    int sex; // 성별 남: 0, 여: 1
    int weight; // 몸무게
    int nowFloor; // 현재 위치한 층, -1은 엘리베이터 탑승
    int goalFloor; // 목표 층
    int orderPlaceOut; // 엘리베이터 밖에 있을 때 좌측: 0, 중간: 1, 우측: 2 순으로 서있는 위치 결정 
    int orderPlaceElv; // 엘리베이터 안에서의 위치, 위 1 2 3 아래 4 5 6
    

   

    Vector3 mousepoz; // = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
    //public Vector3 place;


    [SerializeField] bool isDragging = false;
    [SerializeField] bool isOnElevator = false;
    [SerializeField] bool isAtOutside = true;

    public Collider2D you;

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            mousepoz = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousepoz);
        }
    }

    private void OnMouseUp()
    {
        int p = 0;
        isDragging = false;
        if (isOnElevator)
        {
            nowFloor = -1;
            transform.position = new Vector3(-0.8f, -2, 10);
            //p = 3;
            //while (GameManager.Instance.placement[p].Item1==false)
            //{
            //    p++;
            //}
            //GameManager.Instance.placement[p] = new Tuple<bool, Vector3>(true, new Vector3(-0.8f + p * 1.3f, -2, 10));
            //place = GameManager.Instance.placement[p].Item2;
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

void Start()
    {
        //place = gameObject.GetComponent<Vector3>();
        //you=gameObject.AddComponent<Collider2D>();
    }
}
