﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/EnvironmentStat")]
public class EnvironmentStatType : ScriptableObject
{
    [Header("环境指标名称")]
    public string statName;
    [Header("环境指标说明")]
    public string description;
    [Header("是否是造成坏影响的指标")]
    public bool isNegative;
    [Header("造成的影响")]
    [Reorderable(generatablesNestClass: typeof(AreaBuff))]
    public AreaBuff.ReorderableList buffs;
    [Header("影响每日最大增长比")]
    [Min(0.0f)]
    public float maxGrowRate;
    [Header("初始数值区间(min, max)")]
    public Vector2 initialValueRange;
    /// <summary>
    /// 累计动物数改变
    /// </summary>
    [HideInInspector]
    public float animalChanged;

    private void Awake()
    {
        animalChanged = 0;
    }

    public static EnvironmentStatType[] GetAllTypes()
    {
        return ResourcesLoader.GetAllEnvironmentStatTypes();
    }
}

public class EnvironmentStat
{
    private readonly EnvironmentStatType type;
    public readonly Area area;

    /// <summary>
    /// 环境指标名称
    /// </summary>
    public string name { get { return type.statName; } }
    /// <summary>
    /// 环境指标说明
    /// </summary>
    public string description { get { return type.description; } }
    /// <summary>
    /// 是否是造成坏影响的指标
    /// </summary>
    public bool isNegative { get { return type.isNegative; } }
    /// <summary>
    /// 每日最大增长值
    /// </summary>
    public float maxGrowRate { get { return type.maxGrowRate; } }
    /// <summary>
    /// 造成的影响
    /// </summary>
    public List<AreaBuff> buffs { get { return type.buffs.List; } }
    /// <summary>
    /// 指标值
    /// </summary>
    public float value;
    public EnvironmentStat(EnvironmentStatType environmentStatType, Area area)
    {
        type = environmentStatType;
        this.area = area;
        buffs.ForEach(buff => buff.Applied(area, value));
    }
    /// <summary>
    /// 判断指标种类
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <returns></returns>
    public bool IsType(EnvironmentStatType environmentStatType)
    {
        return type.Equals(environmentStatType);
    }
    /// <summary>
    /// 每日流程
    /// </summary>
    public void DayIdle()
    {
        //affect current area
        buffs.ForEach(buff => buff.Idle(area, value));
        AreaBuff buffAnimalAmount = buffs.Find(buff => buff is AreaBuff.BuffAnimalAmount);
        if(buffAnimalAmount != null)
        {
            type.animalChanged += ((AreaBuff.BuffAnimalAmount)buffAnimalAmount).change;
        }
        //grow and spread
        if(value > 0.0f && maxGrowRate > 0)
        {
            //TODO: reference degree of people environment attention
            value *= 1.0f + UnityEngine.Random.Range(0.0f, maxGrowRate);
            if(value > 1.0f)
            {
                float overflow = value - 1.0f;
                value = 1.0f;
                //spreads to nearby area
                List<Area> neighborAreas = new List<Area>(area.GetNeighborAreas());
                neighborAreas[UnityEngine.Random.Range(0, neighborAreas.Count)].AddEnvironmentStat(type, overflow);
            }
        }
    }
    /// <summary>
    /// 清除环境指标
    /// </summary>
    public void Removed()
    {
        buffs.ForEach(buff => buff.Removed(area, value));
    }
}