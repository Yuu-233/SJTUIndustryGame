﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Building")]
public class BuildingInfo : ScriptableObject
{
    public string buildingName;
    [TextArea]
    public string description;
    public GameObject model;
    public Sprite icon;
    public int moneyCost, timeCost;
    [Header("禁止玩家建造")]
    public bool preventPlayerConstruct;
    [Header("是否是基地")]
    public bool isBasement;
    [Space]
    [Header("Area限制")]
    [Min(1)]
    public int areaLimit = 1;
    [Space]
    [Header("Region限制")]
    public bool hasRegionLimit;
    [Min(1)]
    public int regionLimit = 1;
    [Space]
    public List<BuildingInfo> preFinishBuildings;
    public List<AreaAction> preFinishAreaActions;
    [Header("建筑物效果")]
    public List<AreaBuff> buffs;
    
    public bool CanConstructIn(Area area)
    {
        return !preventPlayerConstruct && area.CountBuilding(this) < areaLimit && (!hasRegionLimit || area.region.CountBuilding(this) < regionLimit)
            && preFinishBuildings.Find(building => !area.ContainsConstructedBuildingInfo(building)) == null
            && preFinishAreaActions.Find(action => !area.ContainsFinishedAction(action)) == null;
    }
}
public class Building
{
    public readonly BuildingInfo info;
    public readonly Area area;
    private int constructionProgress;

    public List<AreaBuff> buffs { get {return info.buffs;}}

    public Building(BuildingInfo info, Area area)
    {
        this.info = info;
        this.area = area;
    }
    public void DayIdle()
    {
        if(IsConstructed())
        {
            buffs.ForEach(buff => buff.Idle(area));
        } else
        {
            constructionProgress += 1;
            if (IsConstructed())
            {
                FinishConstruction();
                PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow("建设完成", info.buildingName));
                if (info.isBasement)
                {
                    area.region.SetBaseArea(area);
                }
            }
        }
    }
    public void FinishConstruction()
    {
        constructionProgress = info.timeCost;
        buffs.ForEach(buff => buff.Applied(area));
    }
    public bool IsConstructed()
    {
        return constructionProgress >= info.timeCost;
    }
    public float GetConstructionRate()
    {
        return info.timeCost > 0 ? ((float) constructionProgress / info.timeCost) : 1.0f;
    }
    public void Removed()
    {
        buffs.ForEach(buff => buff.Removed(area));
    }
}