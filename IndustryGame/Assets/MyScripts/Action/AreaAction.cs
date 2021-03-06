﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - BlankAreaAction")]
public class AreaAction : Action
{
    [Header("可用前需完成的措施(地区)")]
    public List<AreaAction> preFinishActions;
    private bool _isEnabled;
    public sealed override void finishAction(Area area)
    {
        actionEffect(area);
        Stage.AddActionFinishCount(this);
        area.AddFinishedAction(this);
    }
    public bool finishedOnceIn(Area area)
    {
        return area.ContainsFinishedAction(this);
    }
    public virtual void actionEffect(Area area) { }
    public virtual bool enabled(Area area)
    {
        return preFinishActions.Find(action => !action.finishedOnceIn(area)) == null;
    }
}
