﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Building")]
public class Building : ScriptableObject
{
    public string buildingName;
    [TextArea]
    public string description;
    public GameObject model;
    [Header("建筑物效果")]
    public List<Buff> buffs;
    
    public void applied()
    {
        foreach (Buff buff in buffs)
        {
            buff.applied();
        }
    }
    public void idle()
    {
        foreach (Buff buff in buffs)
        {
            buff.idle();
        }
    }
    public void removed()
    {
        foreach (Buff buff in buffs)
        {
            buff.removed();
        }
    }
}
