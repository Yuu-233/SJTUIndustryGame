﻿using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - Research")]
public class ActionResearch : Action
{
    public override void effect(Area area)
    {
    }
    public override bool requireArea()
    {
        return false;
    }
}
