﻿using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Stage : MonoBehaviour
{
    private static Stage instance;

    private Area[] areas;
    private Area baseArea;

    public string stageName;
    [TextArea]
    public string description;
    [Header("Money allowed for this stage")]
    public int stageMoney;
    [Header("Time allowed for this stage")]
    public int stageTime;
    public List<Event> events;
    [Serializable]
    public struct AnimalInitialAmount
    {
        public Animal animal;
        public int amount;
    }
    [Header("初始物种数")]
    public List<AnimalInitialAmount> animalInitialAmounts;

    //基地资源
    private int lastDay;
    int lestMoney;
    List<Specialist> specialists = new List<Specialist>();
    List<Action> enabledActions = new List<Action>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        //collect all Area Components in children gameObject
        areas = GetComponentsInChildren<Area>();
        //pick random area as basement area
        if(areas.Length > 0)
        {
            baseArea = areas[UnityEngine.Random.Range(0, areas.Length)];
        }
    }
    void Start()
    {
        lastDay = Timer.GetDay();
        foreach (AnimalInitialAmount animalInitialAmount in animalInitialAmounts)
        {
            EnvironmentType environment = animalInitialAmount.animal.environment;
            int count = 0;
            foreach(Area area in areas)
            {
                if(area.environmentType == environment)
                {
                    ++count;
                }
            }
            if (count > 0)
            {
                int baseAmount = animalInitialAmount.amount / count;
                System.Random random = new System.Random();
                foreach (Area area in areas)
                {
                    if (area.environmentType == environment)
                    {
                        area.changeSpeciesAmount(animalInitialAmount.animal, (int)(baseAmount * (0.95 + 0.10 * random.NextDouble())));
                    }
                }
            }
        }
    }
    void Update()
    {
        Timer.idle();
        if(lastDay != Timer.GetDay()) //day change happened
        {
            lastDay = Timer.GetDay();
            foreach (Area area in areas)
            {
                area.dayIdle();
            }
            foreach(Specialist specialist in specialists)
            {
                specialist.dayIdle();
            }
        }
        foreach(Event eachEvent in events) {
            eachEvent.idle();
        }
    }

    public static Area[] getAreas()
    {
        return instance.areas;
    }
    public static int getSpeciesAmount(Animal species)
    {
        int total = 0;
        foreach(Area area in instance.areas) {
            total += area.getSpeciesAmount(species);
        }
        return total;
    }
    public static int getSpeciesChange(Animal species)
    {
        int total = 0;
        foreach (Area area in instance.areas)
        {
            total += area.getSpeciesChange(species);
        }
        return total;
    }
    public static Area getBaseArea()
    {
        return instance.baseArea;
    }
    public static List<Specialist> GetSpecialists()
    {
        return instance.specialists;
    }
}
