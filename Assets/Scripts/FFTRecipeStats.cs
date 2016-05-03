using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTRecipeStats : System.Object {

    // DIFFICULTY

    public static float GetDifficulty(FFTRecipeMaker recipeMaker)
    {
        int highestGreenZone = 0;
        if (recipeMaker.Counter != null)
        {
            foreach (FFTDish dish in recipeMaker.Counter.RecipeCard.Dishes)
            {
                if (dish.StepDataObjects != null)
                {
                    foreach (FFTStep step in dish.StepDataObjects)
                    {
                        int currentGreenZone = GetGreenzoneNumber(step.Parameters.Cooked, step.Parameters.IsBurnable);
                        if (currentGreenZone > highestGreenZone)
                            highestGreenZone = currentGreenZone;
                    }
                }
            }
        }

        return GetDifficulty(GetStepsPerSecond(recipeMaker.TotalSteps, recipeMaker.TimeLimit), highestGreenZone);

    }

    public static float GetDifficulty(float stepsPerSecond, float greenZoneNumber)
    {
        if (greenZoneNumber > 0)
            return stepsPerSecond * greenZoneNumber;
        else
            return 0;
    }

    public static float GetStepsPerSecond(int totalNumberOfSteps, float totalRecipeTimeInSeconds)
    {
        if (totalRecipeTimeInSeconds == 0)
            return 0;
        return (float)totalNumberOfSteps / totalRecipeTimeInSeconds;
    }

    public static int GetGreenzoneNumber(float secondsInGreenZone, bool isBurnable)
    {
        if (!isBurnable)
            return 0; // 0 = unlimited greenzone time/unburnable
        else
        {
            if (secondsInGreenZone >= 30f)
                return 1; // 1 = 30+ seconds in greenzone
            else if (secondsInGreenZone >= 15f) // 2 = 15 - 29s
                return 2;
            else if (secondsInGreenZone >= 5f) // 3 = 5 - 15s
                return 3;
            else // 4 = <5s
                return 4;
        }
    }

    // COMPLEXITY

    public static float GetComplexity(FFTRecipeMaker recipeMaker)
    {
        FFTCounter counter = recipeMaker.Counter;
        FFTKitchen kitchen = recipeMaker.Kitchen;
        if (recipeMaker.TimedLevel)
            return GetComplexity(counter, kitchen, recipeMaker.TimeLimit);
        else
            return 0;
    }


    public static float GetComplexity(FFTCounter counter, FFTKitchen kitchen, float timeLimit)
    {
        return GetStationComplexity(FFTStation.Type.Chop, counter, kitchen, timeLimit) +
               GetStationComplexity(FFTStation.Type.Cook, counter, kitchen, timeLimit) +
               GetStationComplexity(FFTStation.Type.Spice, counter, kitchen, timeLimit) +
               GetStationComplexity(FFTStation.Type.Prep, counter, kitchen, timeLimit);
    }

    static float GetStationComplexity(FFTStation.Type type, FFTCounter counter, FFTKitchen kitchen, float timeLimit)
    {
        /*
         * Step 1. Total time required by steps at that station for entire recipe / cooking time available 
         * (defined as the recipe time x number of slots at that station).  Time required at a station is 
         * defined as the amount of time it takes to complete the yellow/underdone phase.

         * Step 2. Multiply your number from above by the number of elements using that station in the recipe 
         * (where elements is defined as number of separate times an ingredient must be placed on that station; 
         * note that an ingredient may have more than one element for a station, if it requires being used on 
         * that station more than once during the recipe).

         * Step 3.  Repeat the above for all four stations and sum the values.

         */

        List<FFTStep> stepsUsingStation = new List<FFTStep>();

        float totalUndoneTime = 0f;

        foreach (FFTDish dish in counter.RecipeCard.Dishes)
        {
            if (dish.StepDataObjects != null)
            {
                foreach (FFTStep step in dish.StepDataObjects)
                {
                    if (step.Destination == type)
                        stepsUsingStation.Add(step);
                    totalUndoneTime += step.Parameters.Uncooked;
                }
            }
        }
        if (kitchen.Stations.ContainsKey(type))
        {
            float totalTimeRequired = timeLimit * kitchen.Stations[type].SlotList.Count;
            if (totalTimeRequired == 0)
                return 0; //avoid division by 0, with no Stations this would be the result anyway.

            float output = stepsUsingStation.Count * (totalUndoneTime / totalTimeRequired);

            Debug.Log(type.ToString() + " " + output);

            return output;
        }

        return 0; // no stations of this type exist.

    }
}
