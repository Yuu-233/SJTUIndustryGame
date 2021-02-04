﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReportWindow : MonoBehaviour, BasicReportWindow
{
    [Header("生成小报告位置")]
    public GameObject EventReportList;
    [Header("小报告Prefab")]
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    private List<GameObject> EventReports = new List<GameObject>();

    public void ClearList()
    {
        Helper.ClearList(EventReports);
    }

    public void GenerateList()
    {
        ClearList();
        foreach (MainEvent mainEvent in Stage.GetEvents())
        {
            GameObject clone = GameObject.Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
            clone.GetComponent<SingleEventReport>().ShowSingleEvent(mainEvent);
            EventReports.Add(clone);
        }
    }

    void Start()
    {
        GenerateList();
    }
}
