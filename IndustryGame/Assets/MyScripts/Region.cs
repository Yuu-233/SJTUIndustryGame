﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 洲
/// </summary>
public class Region
{
    /// <summary>
    /// 洲名称
    /// </summary>
    public readonly string name;
    /// <summary>
    /// 洲序号
    /// </summary>
    public readonly int regionId;
    /// <summary>
    /// 是否为海洋
    /// </summary>
    public bool IsOcean { get { return regionId == -1; } }

    //data
    private readonly List<Area> areas = new List<Area>();
    private MainEvent mainEvent;
    /// <summary>
    /// 这个州上被设定的事件（一个）
    /// </summary>
    public MainEvent MainEvent { get { return mainEvent; } }
    private readonly List<Animal> concernedAnimals = new List<Animal>();
    private readonly HexSpiral hexSpiral = new HexSpiral();
    private int basementLevel;
    public int BasementLevel { get { return basementLevel; } }
    private int reservatedAreaCount;
    private float reservationTime = 1;
    private float baseReservationPower = 2f, reservationProgress;
    private Area baseArea;
    private Area left, right, bottom, top;
    private Region northR, southR, westR, eastR;
    private Vector3 center;
    private Vector3 highestPosition;
    public float observeOrthoSize;
    private Dictionary<Stack<HexCell>, float> lastHighLightedCellAndTime = new Dictionary<Stack<HexCell>, float>();
    private GameObject nameDisplay;

    private static readonly NameTemplates regionNameTemplates = Resources.Load<NameTemplates>("NameTemplates/RegionName");

    private BasementLabelHUD basementLabelHUD;

    public Region(int regionId)
    {
        this.regionId = regionId;
        name = regionId == -1 ? "海洋" : regionNameTemplates.PickRandomOne();
        basementLabelHUD = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/Area/BasementLabel").GetComponent<BasementLabelHUD>());
        basementLabelHUD.gameObject.SetActive(false);
    }
    /// <summary>
    /// 更新洲名显示
    /// </summary>
    public void UpdateNameDisplay()
    {
        if(nameDisplay != null)
            nameDisplay.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f - OrthographicCamera.ZoomRate);
    }
    /// <summary>
    /// 每帧流程
    /// </summary>
    public void FrameIdle()
    {
        //reservation
        foreach(var pair in lastHighLightedCellAndTime.ToList())
        {
            Stack<HexCell> cells = pair.Key;
            if ((lastHighLightedCellAndTime[cells] -= Timer.getTimeSpeed() * Time.deltaTime) < 0) //remove outdated highlight effects
            {
                while (cells.Count > 0)
                {
                    cells.Pop().HighLighted = false;
                }
                lastHighLightedCellAndTime.Remove(cells);
            }
        }
        if (baseArea != null)
        {
            reservationProgress += GetReservationPower() * Timer.getTimeSpeed() * Time.deltaTime;
            Stack<HexCell> lastHighLightedCells = new Stack<HexCell>();
            float reservationCostOfOneArea = reservationTime + concernedAnimals.Count * 0.2f;
            while (reservationProgress >= reservationCostOfOneArea)
            {
                reservationProgress -= reservationCostOfOneArea;
                HexCell cell = null;
                int loops = 0;
                while (++loops < 512)
                {
                    cell = Stage.GetHexGrid().GetCell(hexSpiral.next());
                    if (cell != null && cell.RegionId == regionId)
                        break;
                }
                if (cell == null)
                {
                    InGameLog.AddLog("an area is missing", Color.red);
                    reservationInit();
                }
                else
                {
                    cell.HighLighted = true;
                    lastHighLightedCells.Push(cell);
                    cell.GetComponentInChildren<Area>().AddReservation();
                    if (++reservatedAreaCount >= areas.Count)
                    {
                        reservationInit();
                    }
                }
            }
            if(lastHighLightedCells.Count > 0)
            {
                lastHighLightedCellAndTime.Add(lastHighLightedCells, 3.0f);
            }
            //show progress circle on top of basement area
            basementLabelHUD.reservationProgressImage.fillAmount = (float)reservatedAreaCount / areas.Count;
            //basement tooltip
            if (basementLabelHUD.tooltip.activeSelf)
            {
                basementLabelHUD.tooltipText.text = "基地等级: " + RomanNumerals.convert(basementLevel) + "\n调查进度: " + reservatedAreaCount + " / " + areas.Count;
            }
        }
    }
    /// <summary>
    /// 每日流程
    /// </summary>
    public void DayIdle()
    {
        if (regionId == -1)
            return;
        foreach (Area area in areas)
        {
            area.dayIdle();
        }
        if(mainEvent != null)
        {
            mainEvent.DayIdle();
        }
    }
    /// <summary>
    /// 初始化调查
    /// </summary>
    private void reservationInit()
    {
        reservatedAreaCount = 0; // base area is always reservated
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
    }
    /// <summary>
    /// 更新洲濒危动物列表(每当一个事件流开始/结束时被调用)
    /// </summary>
    public void UpdateConcernedSpecies()
    {
        concernedAnimals.Clear();
        if (mainEvent != null && mainEvent.IsAppeared && !mainEvent.IsFinished)
        {
            if (!concernedAnimals.Contains(mainEvent.concernedAnimal))
                concernedAnimals.Add(mainEvent.concernedAnimal);
        }
        Stage.UpdateConcernedSpecies();
    }
    /// <summary>
    /// 获取洲濒危动物列表
    /// </summary>
    /// <returns></returns>
    public List<Animal> GetConcernedSpecies()
    {
        return concernedAnimals;
    }
    /// <summary>
    /// 获取最新统计内指定动物数量
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int? GetSpeciesAmountInLatestRecord(Animal animal)
    {
        int sum = 0;
        bool hasRecord = false;
        foreach(Area area in areas)
        {
            int? value = area.GetSpeciesAmountInLatestRecord(animal);
            if(value.HasValue)
            {
                hasRecord = true;
                sum += value.Value;
            }
        }
        return hasRecord ? (int?)sum : null;
    }
    /// <summary>
    /// 获取最新统计内指定动物增减量
    /// </summary>
    public int? GetSpeciesChangeInLatestRecord(Animal animal)
    {
        int sum = 0;
        bool hasRecord = false;
        foreach (Area area in areas)
        {
            int? value = area.GetSpeciesChangeInLatestRecord(animal);
            if (value.HasValue)
            {
                hasRecord = true;
                sum += value.Value;
            }
        }
        return hasRecord ? (int?)sum : null;
    }
    /// <summary>
    /// 添加地区
    /// </summary>
    /// <param name="area"></param>
    public void AddArea(Area area)
    {
        if (area == null)
            return;
        if (top == null)
        {
            top = bottom = left = right = area;
        } else
        {
            Vector3 pos = area.gameObject.transform.parent.position;
            if (left.transform.position.x > pos.x)
            {
                left = area;
            } else if (right.transform.position.x < pos.x)
            {
                right = area;
            }
            if (bottom.transform.position.z > pos.z)
            {
                bottom = area;
            } else if (top.transform.position.z < pos.z)
            {
                top = area;
            }
        }
        areas.Add(area);
    }
    /// <summary>
    /// 设置基地地区
    /// </summary>
    /// <param name="area"></param>
    public void SetBaseArea(Area area)
    {
        baseArea = area;
        basementLevel = 1;
        area.basementLabelHolder.AddSurrounders(basementLabelHUD.gameObject);
        basementLabelHUD.nameText.text = name + "基地";
        basementLabelHUD.levelText.text = RomanNumerals.convert(basementLevel);
        basementLabelHUD.transform.parent = Stage.CanvasForSurroundersHUD.transform;
        basementLabelHUD.gameObject.SetActive(true);
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
        reservatedAreaCount = 0;
    }
    /// <summary>
    /// 获取基地地区
    /// </summary>
    /// <returns></returns>
    public Area GetBaseArea()
    {
        return baseArea;
    }
    /// <summary>
    /// 获取洲序号
    /// </summary>
    /// <returns></returns>
    public int GetRegionId()
    {
        return regionId;
    }
    /// <summary>
    /// 获取所含地区
    /// </summary>
    /// <returns></returns>
    public List<Area> GetAreas()
    {
        return areas;
    }
    /// <summary>
    /// 统计指定环境种类数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int CountEnvironmentType(EnvironmentType type)
    {
        int count = 0;
        foreach (Area area in areas)
        {
            if (area.environmentType.Equals(type))
                ++count;
        }
        return count;
    }
    /// <summary>
    /// 添加事件流
    /// </summary>
    /// <param name="anEvent"></param>
    public void SetEvent(MainEvent anEvent) {
        mainEvent = anEvent;
    }
    /// <summary>
    /// 统计指定类型已建设建筑物数量
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
    public int CountConstructedBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        areas.ForEach(area => count += area.CountConstructedBuilding(buildingInfo));
        return count;
    }
    /// <summary>
    /// 统计指定类型建筑物数量
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
    public int CountBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        areas.ForEach(area => count += area.CountBuilding(buildingInfo));
        return count;
    }
    /// <summary>
    /// 重新计算洲中心坐标
    /// </summary>
    public void CalculateCenter()
    {
        float cx = (left.transform.position.x + right.transform.position.x) / 2, cy = (top.transform.position.z + bottom.transform.position.z) / 2;
        center = new Vector3(cx, 150f, cy);
        if (regionId != -1 && nameDisplay == null)
        {
            nameDisplay = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/Region/RegionNameDisplay"));
            nameDisplay.GetComponentInChildren<Text>().text = name;
            Transform nameDisplayTransform = nameDisplay.transform;
            nameDisplayTransform.position = new Vector3(cx, 50f, cy);
            nameDisplayTransform.GetComponentInChildren<RectTransform>().sizeDelta = 0.8f * new Vector2(Mathf.Abs(left.transform.position.x - right.transform.position.x), Mathf.Abs(top.transform.position.z - bottom.transform.position.z));
        }
    }
    /// <summary>
    /// 重新计算洲最高点
    /// </summary>
    public void CalculateHighestPosition()
    {
        List<Area> orderedAreas = areas.OrderByDescending(a => a.GetComponentInParent<HexCell>().transform.position.y).ToList();
        highestPosition = orderedAreas[0].GetComponentInParent<HexCell>().transform.position;
    }

    /// <summary>
    /// 获取洲最高点坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHighestPosition()
    {
        return highestPosition;
    }

    /// <summary>
    /// 获取洲中心坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenter()
    {
        return center;
    }
    /// <summary>
    /// 获取洲左端坐标
    /// </summary>
    /// <returns></returns>
    public float GetLeft()
    {
        return left.transform.position.x;
    }

    /// <summary>
    /// 获取洲右端坐标
    /// </summary>
    /// <returns></returns>
    public float GetRight()
    {
        return right.transform.position.x;
    }

    /// <summary>
    /// 获取洲上端坐标
    /// </summary>
    /// <returns></returns>
    public float GetTop()
    {
        return top.transform.position.z;
    }

    /// <summary>
    /// 获取洲下端坐标
    /// </summary>
    /// <returns></returns>
    public float GetBottom()
    {
        return bottom.transform.position.z;
    }

    public Region GetSouthRegion()
    {
        return southR;
    }

    public Region GetNorthRegion()
    {
        return northR;
    }
    public Region GetEastRegion()
    {
        return eastR;
    }
    public Region GetWestRegion()
    {
        return westR;
    }

    public void SetSouthRegion(Region region)
    {
        southR = region;
    }

    public void SetNorthRegion(Region region)
    {
        northR = region;
    }
    public void SetEastRegion(Region region)
    {
        eastR = region;
    }
    public void SetWestRegion(Region region)
    {
        westR = region;
    }

    /// <summary>
    /// 获取洲大小
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public float GetSizeInCamera(Camera camera)
    {
        float xSize = Mathf.Abs(camera.WorldToViewportPoint(left.transform.position).x - camera.WorldToViewportPoint(right.transform.position).x);
        float ySize = Mathf.Abs(camera.WorldToViewportPoint(top.transform.position).y - camera.WorldToViewportPoint(bottom.transform.position).y);
        return Mathf.Max(xSize, ySize);
    }
    /// <summary>
    /// 获取洲调查每日进度
    /// </summary>
    /// <returns></returns>
    public float GetReservationPower()
    {
        float power = baseReservationPower; ;
        foreach(Specialist specialist in GetSpecialistsInRegion())
        {
            power += specialist.GetLevel();
        }
        return power;
    }
    /// <summary>
    /// 获取州内所有专家
    /// </summary>
    /// <returns></returns>
    public List<Specialist> GetSpecialistsInRegion()
    {
        return Stage.GetSpecialists().FindAll(specialist =>
        {
            Area area = specialist.Area;
            return area != null && area.region.Equals(this);
        });
    }
    /// <summary>
    /// 获取洲内环境相关报告
    /// </summary>
    /// <returns></returns>
    public List<EventStage> GetEventInfosRelatedToEnvironment() //TODO: optimize this code
    {
        return mainEvent != null ? mainEvent.GetRevealedStagesRelatedToEnvironment() : new List<EventStage>();
    }
    /// <summary>
    /// 全部栖息地等级总和
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int TotalRevealedColonyLevel(Animal animal)
    {
        int totalColonyLevel = 0;
        foreach (Area area in areas)
        {
            if (area.habitat != null && area.habitat.IsRevealed && area.habitat.animal.Equals(animal))
                totalColonyLevel += area.habitat.Level;
        }
        return totalColonyLevel;
    }
}
