﻿using System.Collections.Generic;
using UnityEngine;

/**
 *  usage: 
 *  refresh: SpecialistEmployList.refresh();
 *  access: SpecialistEmployList.getSpecialists();
 *  hire: SpecialistEmployList.hireSpecialist(Specialist specialist)
 */
public static class SpecialistEmployList {
    private static SpecialistTemplate[] indoorSpecialistTemplates = Resources.LoadAll<SpecialistTemplate>("Specialist/Indoor");
    private static SpecialistTemplate[] outdoorSpecialistTemplates = Resources.LoadAll<SpecialistTemplate>("Specialist/Outdoor");
    private static List<Specialist> specialists = new List<Specialist>();
    public static int listSize = 5;

    public static void refresh()
    {
        System.Random random = new System.Random();
        for(int i = 0; i < listSize; ++i)
        {
            Ability speciality = EnumHelper.GetRandomValue<Ability>();
            InGameLog.AddLog("ability: " + speciality.ToString());
            Specialist specialist = new Specialist();
            specialist.name = Resources.Load<NameTemplates>("NameTemplates/SpecialistName").pickRandomOne();
            specialist.birthday = "randomBirthDay";
            specialist.birthplace = Resources.Load<NameTemplates>("NameTemplates/CityName").pickRandomOne();
            int abilityLevelTotal = 0;
            if (random.NextDouble() < 0.5f) { //indoor
                specialist.specialistTemplate = indoorSpecialistTemplates[random.Next(0, indoorSpecialistTemplates.Length)];
                abilityLevelTotal += specialist.addSpeciality_range(speciality, 7, 10);
                for(int j = 0; j < 2; ++j)
                {
                    abilityLevelTotal += specialist.addSpeciality_range(EnumHelper.GetRandomValue<Ability>(), 0, 2);
                }
            } else { //outdoor
                specialist.specialistTemplate = outdoorSpecialistTemplates[random.Next(0, outdoorSpecialistTemplates.Length)];
                abilityLevelTotal += specialist.addSpeciality_range(speciality, 4, 6);
                for (int j = 0; j < 4; ++j)
                {
                    abilityLevelTotal += specialist.addSpeciality_range(EnumHelper.GetRandomValue<Ability>(), 1, 3);
                }
            }
            specialist.hireCost = abilityLevelTotal * 20; //level * 20 = cost
            specialists.Add(specialist);
        }
    }
    public static List<Specialist> getSpecialists()
    {
        return specialists;
    }
    public static void hireSpecialist(Specialist specialist)
    {
        if(specialists.Remove(specialist))
        {
            Stage.GetSpecialists().Add(specialist);
            specialist.moveToArea(Stage.getBaseArea()); //spawn in basement
            Stage.subMoney(specialist.hireCost);
        } else
        {
            InGameLog.AddLog("hireSpecialist received a non-existent specialist", Color.red);
        }
    }
}
