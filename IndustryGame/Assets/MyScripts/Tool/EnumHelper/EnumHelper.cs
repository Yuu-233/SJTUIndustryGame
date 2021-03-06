﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class EnumHelper
{
    public static string GetDescription(Enum enumValue)
    {
        string enumName = enumValue.ToString();
        //获取描述属性
        object[] objs = enumValue.GetType().GetField(enumName).GetCustomAttributes(typeof(DescriptionAttribute), false);
        //当描述属性没有时，直接返回名称
        return objs.Length == 0 ? enumName : ((DescriptionAttribute)objs[0]).Description;
    }
    public static T GetRandomValue<T>()
    {
        T[] enums = (T[])Enum.GetValues(typeof(T));
        return enums[UnityEngine.Random.Range(0, enums.Length)];
    }
    public static int GetRandomIndex<T>()
    {
        T[] enums = (T[])Enum.GetValues(typeof(T));
        return UnityEngine.Random.Range(0, enums.Length);
    }
    public static int GetMaxEnum<T>()
    {
        System.Type t = typeof(T);
        int maxT = 0;
        var arrT = System.Enum.GetValues(t);

        foreach (var oneT in arrT)
        {
            if((int)oneT > (int)maxT)
            {
                maxT = (int)oneT;
            }
        }

        return maxT;
    }

    // public static string GetEnumName<T>(this int status)
    // {
    //     return System.Enum.GetName(typeof(T), status);
    // }

    // public static string[] GetNamesArr<T>()
    // {
    //     return System.Enum.GetNames(typeof(T));
    // }

    // public static Dictionary<string, int> getEnumDic<T>()
    // {

    //     Dictionary<string, int> resultList = new Dictionary<string, int>();
    //     System.Type type = typeof(T);
    //     var strList = GetNamesArr<T>().ToList();
    //     foreach (string key in strList)
    //     {
    //         string val = System.Enum.Format(type, System.Enum.Parse(type, key), "d");
    //         resultList.Add(key, int.Parse(val));
    //     }
    //     return resultList;
    // }

    // public static Dictionary<string, int> GetDic<TEnum>()
    // {
    //     Dictionary<string, int> dic = new Dictionary<string, int>();
    //     System.Type t = typeof(TEnum);
    //     var arr = System.Enum.GetValues(t);
    //     foreach (var item in arr)
    //     {
    //         dic.Add(item.ToString(), (int)item);
    //     }

    //     return dic;
    // }

}