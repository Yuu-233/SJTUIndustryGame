﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum SpeciesDangerType
{
    [Description("正常")]
    Normal,
    [Description("濒危")]
    Danger,
    [Description("极度濒危")]
    VeryDanger,
}

/// <summary>
/// 物种
/// </summary>
[CreateAssetMenu(menuName = "Add ScriptableObjects/Animal")]
public class Animal : ScriptableObject
{
    /// <summary>
    /// 物种名称
    /// </summary>
    public string animalName;
    /// <summary>
    /// 物种描述
    /// </summary>
    [TextArea]
    public string description;
    /// <summary>
    /// 物种图片
    /// </summary>
    public Sprite image;
    [Min(1)]
    public int habitat1MinPopulation;
    [Min(1)]
    public int habitat2MinPopulation;
    [Min(1)]
    public int habitat3MinPopulation;
    [Min(1)]
    public int habitat4MinPopulation;
    [Min(1)]
    public int habitat5MinPopulation;
    /// <summary>
    /// 物种首要栖息环境
    /// </summary>
    public EnvironmentType bestEnvironmentType;
    [Serializable]
    public struct EnvironmentPreference
    {
        public EnvironmentType environment;
        [Range(-1, 1)]
        public float preference;
    }
    /// <summary>
    /// 物种各环境适应度
    /// </summary>
    [Header("物种各环境适应度")]
    public List<EnvironmentPreference> environmentPreferences;
    /// <summary>
    /// 物种模型
    /// </summary>
    public GameObject model;
    /// <summary>
    /// 物种基础每日增加
    /// </summary>
    [Min(0)]
    public int reproductionSpeed;
    /// <summary>
    /// 物种猎食对象
    /// </summary>
    public List<Animal> foods;
    /// <summary>
    /// 物种生存所需能量
    /// </summary>
    [Min(0)]
    public int energyNeeds;
    /// <summary>
    /// 物种被食提供能量
    /// </summary>
    [Min(0)]
    public int energyAsFood;
    /// <summary>
    /// 最低适应温度
    /// </summary>
    public double minTempreture;
    /// <summary>
    /// 最高适应温度
    /// </summary>
    public double maxTempreture;
    /// <summary>
    /// 迁徙规模上限
    /// </summary>
    [Min(0)]
    public int migrateLimit;

    /// <summary>
    /// 物种生活音效
    /// </summary>
    public AudioClipList sfxAudio;

    /// <summary>
    /// 猜测: 危险级别区间大小
    /// </summary>
    private List<int> dangerLimits;
    /// <summary>
    /// 猜测: 危险级别划分数
    /// </summary>
    public int mostDangerLimit;
    /// <summary>
    /// 相关的主事件
    /// </summary>
    public MainEventSO relatedMainEvent;

    public int minHabitatPopulation(int colonyLevel)
    {
        switch(colonyLevel)
        {
            case 1: return habitat1MinPopulation;
            case 2: return habitat2MinPopulation;
            case 3: return habitat3MinPopulation;
            case 4: return habitat4MinPopulation;
            case 5: return habitat5MinPopulation;
            default:
                if (colonyLevel <= 0) return 0;
                else return habitat5MinPopulation * 2;
        }
    }
    public int maxHabitatPopulation(int colonyLevel)
    {
        return minHabitatPopulation(colonyLevel + 1);
    }
    private void Start()
    {
        int mostDangerType = EnumHelper.GetMaxEnum<SpeciesDangerType>();
        int averageAmount = (int)((float)mostDangerLimit / (float)mostDangerType);
        for (int i = 0; i <= mostDangerType; i++)
        {
            dangerLimits[mostDangerType - i] = i * averageAmount;
        }
    }
    public void idle(Area currentArea, int amount)
    {
        if (amount == 0)
            return;
        //grow: check food requirements satisfied for how many units
        int foodSatisfiedAmount = currentArea.ProvideFood(foods, energyNeeds, amount);
        currentArea.changeSpeciesAmount(this, foodSatisfiedAmount * reproductionSpeed);

        //decide how many should migrate to other area
        if (migrateLimit > 0)
        {
            int migrationAmount = (int)(amount * getAreaDislikeness(currentArea, amount)); // dislikeness: -inf ~ 1.0
            if (migrationAmount > 0) // do migration
            {
                if (migrationAmount > migrateLimit)
                {
                    migrationAmount = migrateLimit;
                }
                Area migrateDst = null;
                double leastMigrateDstDislikeness = 1.0;
                foreach (Area area in currentArea.GetNeighborAreas())
                {
                    double newDislikeness = getAreaDislikeness(area, migrationAmount);
                    if (newDislikeness < leastMigrateDstDislikeness)
                    {
                        migrateDst = area;
                        leastMigrateDstDislikeness = newDislikeness;
                    }
                }
                if (migrateDst != null)
                {
                    migrate(currentArea, migrateDst, migrationAmount);
                }
            }
        }
    }

    public void migrate(Area src, Area dst, int migrationAmount)
    {
        src.changeSpeciesAmount(this, -migrationAmount);
        dst.changeSpeciesAmount(this, +migrationAmount);
    }

    public float getAreaDislikeness(Area area, int amount)
    {
        float dislikeness = 0.0f;
        //food affection
        if (energyNeeds > 0)
            dislikeness += 1.0f - (float)area.GetProvidableFoodEnergy(foods, energyNeeds) / (amount * energyNeeds);
        //environmentType affection
        bool foundInSetting = false;
        foreach (EnvironmentPreference environmentAndPreference in environmentPreferences)
        {
            if (environmentAndPreference.environment.Equals(area.environmentType))
            {
                dislikeness -= environmentAndPreference.preference;
                foundInSetting = true;
                break;
            }
        }
        if(!foundInSetting && bestEnvironmentType.Equals(area.environmentType))
        {
            dislikeness -= 1.0f;
        }
        //TODO: tempreture affection
        return dislikeness;
    }

    public SpeciesDangerType getDangerType(int amount)
    {
        int dangerType = EnumHelper.GetMaxEnum<SpeciesDangerType>();
        for (int i = 0; i < dangerLimits.Count; i++)
        {
            if (amount >= dangerLimits[i])
            {
                dangerType = i;
                break;
            }
        }
        return (SpeciesDangerType)dangerType;
    }
}
