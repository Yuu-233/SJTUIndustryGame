﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelRegionSelect : MonoBehaviour
{
    public Region region;
    [Header("显示Region名称")]
    public Text RegionName;
    public Image BackgroundImage;

    private Button button;
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => FilterPanel.RefreshEventsDueToRegionSelection(region));
        button.onClick.AddListener(() => FilterPanel.instance.GetComponent<FilterPanelFocusHelper>().SelectImage(BackgroundImage, RegionName));
    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        RegionName.text = region.name;
    }


}
