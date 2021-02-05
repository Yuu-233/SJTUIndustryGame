﻿using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReorderableConditionList), true)]
public class ReorderableListDrawer : PropertyDrawer
{
    private ReorderableList _list;

    private ReorderableList GetReorderableList(SerializedProperty property)
    {
        if (_list == null)
        {
            var listProperty = property.FindPropertyRelative("List");

            _list = new ReorderableList(property.serializedObject, listProperty, true, true, true, true);

            _list.drawHeaderCallback += delegate (Rect rect)
            {
                EditorGUI.LabelField(rect, property.displayName);
            };
            _list.drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused)
            {
                rect.x += 10;rect.width -= 10;
                EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index), true);
                rect.x += 65;rect.height = 20;
                EditorGUI.LabelField(rect, "- " + listProperty.GetArrayElementAtIndex(index).managedReferenceFullTypename.Substring(16));
            };
            _list.elementHeightCallback = delegate (int index)
            {
                return EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(index), true);
            };
            _list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Region/CheckBuildingCount"),
                    false, clickHandler, new CheckRegionBuilding());
                menu.AddItem(new GUIContent("Region/ReservationCount"),
                    false, clickHandler, new CheckRegionReservationCount());
                menu.ShowAsContext();
            };
        }

        return _list;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetReorderableList(property).GetHeight();
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var list = GetReorderableList(property);

        var listProperty = property.FindPropertyRelative("List");
        var height = 0f;
        for (var i = 0; i < listProperty.arraySize; i++)
        {
            height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
        }

        list.elementHeight = height;
        list.DoList(position);
    }
    private void clickHandler(object target)
    {
        SerializedProperty listProperty = _list.serializedProperty;
        SerializedProperty lastElementProperty = _list.serializedProperty.GetArrayElementAtIndex(++listProperty.arraySize - 1);
        lastElementProperty.managedReferenceValue = target;
        lastElementProperty.isExpanded = true;
        _list.serializedProperty.serializedObject.ApplyModifiedProperties();
    }
}