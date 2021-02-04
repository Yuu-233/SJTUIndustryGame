﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;

/// <summary>
/// 单个报告在选择界面的缩略形态
/// </summary>

public enum ReportSelectionWindowType
{
    [Description("事件报告")]
    EventType,
    [Description("动物报告")]
    AnimalType,
    [Description("事件报告")]
    EnvironmentType
}

public class SingleEventReport : MonoBehaviour
{
    // For Events
    [HideInInspector]
    public MainEvent mainEvent;                            // Event存储在MainEvent的数据类型

    //For Animal and Environment Reports
    [HideInInspector]
    public Animal animal;                         // Animal的数据类型

    //For Environment Reports
    [HideInInspector]
    public EventStage environmentEventStage;

    [HideInInspector]
    public ReportSelectionWindowType windowType;



    [Header("展示的图片的Image")]
    public Image EventImage;                            //展示的
    [Header("展示EventName的text")]
    public Text EventName;



    #region 展示自己
    public void ShowSingleEvent(MainEvent mainEvent)
    {
        this.mainEvent = mainEvent;
        EventName.text = mainEvent.name;
        windowType = ReportSelectionWindowType.EventType;
    }

    public void ShowSingleAnimal(Animal animal)
    {
        this.animal = animal;
        EventName.text = animal.animalName;
        windowType = ReportSelectionWindowType.AnimalType;
    }
    public void ShowSingleEnvironment(EventStage EventStage)
    {
        this.environmentEventStage = EventStage;
        EventName.text = EventStage.name;
        windowType = ReportSelectionWindowType.EnvironmentType;
    }

    #endregion


    #region 展示详情界面
    public void DisplayReport()
    {
        switch (windowType)
        {
            case ReportSelectionWindowType.EventType:
                ReportUI.GenerateEventDetailsWindow(mainEvent);
                break;
            case ReportSelectionWindowType.AnimalType:
                ReportUI.GenerateAnimalDetailsWindow(animal);
                break;
            case ReportSelectionWindowType.EnvironmentType:
                ReportUI.GenerateEnvironmentDetailsWindow(environmentEventStage);
                break;

        }
    }

    #endregion
}
