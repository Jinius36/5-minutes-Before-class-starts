using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2 : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.goal = 10;
        Debug.Log(GameManager.Instance.goal);
    }
}
