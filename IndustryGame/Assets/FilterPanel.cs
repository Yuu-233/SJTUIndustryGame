﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterPanel : MonoBehaviour
{
    public static FilterPanel instance;
    [Header("生成Region选项的位置")]
    public GameObject RegionSelectionGeneratePosition;
    [Header("Region选项的Prefab")]
    public GameObject RegionSelectionPrefab;
    private List<GameObject> GeneratedRegionSelections = new List<GameObject>();
    [Header("生成Event选项的位置")]
    public GameObject EventSelectionGeneratePosition;
    [Header("Event选项的Prefab")]
    public GameObject EventSelectionPrefab;
    private List<GameObject> GeneratedEventSelections = new List<GameObject>();
    public FilterPanelFocusHelper focusHelperForRegion;
    public FilterPanelEventFocusHelper focusHelperForEvent;

    public GameObject AnimalSelectionPanel;

    private GameObject GeneratedAnimalSelectionPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        RefreshRegions();
        FilterPanelRegionSelect StartSelectedRegion = instance.GeneratedRegionSelections[0].GetComponent<FilterPanelRegionSelect>();
        RefreshEventsDueToRegionSelection(StartSelectedRegion.region);
        focusHelperForRegion.SelectImage(StartSelectedRegion.BackgroundImage, StartSelectedRegion.RegionName);
    }

    void Update()
    {
    }

    public static void RefreshRegions()
    {
        Helper.ClearList(instance.GeneratedRegionSelections);

        foreach (Region region in Stage.GetRegions())
        {
            if (region.regionId == -1)
            {
                continue;
            }
            GameObject clone = Instantiate(instance.RegionSelectionPrefab, instance.RegionSelectionGeneratePosition.transform, false);
            clone.GetComponent<FilterPanelRegionSelect>().region = region;
            instance.GeneratedRegionSelections.Add(clone);
        }
    }

    public static void RefreshEventsDueToRegionSelection(Region region)
    {
        Helper.ClearList(instance.GeneratedEventSelections);
        foreach (MainEvent mainEvent in region.GetRevealedEvents())
        {
            GameObject clone = Instantiate(instance.EventSelectionPrefab, instance.EventSelectionGeneratePosition.transform, false);
            clone.GetComponent<FilterPanelEventSelect>().mainEvent = mainEvent;
            instance.GeneratedEventSelections.Add(clone);
        }
    }

    public static void GenerateAnimalSelectionPanel(MainEvent mainEvent)
    {
        if (instance.GeneratedAnimalSelectionPanel != null)
        {
            Destroy(instance.GeneratedAnimalSelectionPanel);
        }
        instance.GeneratedAnimalSelectionPanel = Instantiate(instance.AnimalSelectionPanel, instance.transform, false);
        instance.GeneratedAnimalSelectionPanel.GetComponent<FilterPanelAnimalPanel>().mainEvent = mainEvent;
        instance.GeneratedAnimalSelectionPanel.GetComponent<FilterPanelAnimalPanel>().RefreshUI();
    }
}
