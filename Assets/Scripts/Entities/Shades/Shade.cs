using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade
{
    // Properties
    public string Name { get; set; }
    public string Origin { get; set; }
    public string Occupation { get; set; }
    public string LifeSummary { get; set; }
    public string LifeAction1 { get; set; }
    public string LifeAction2 { get; set; }
    public string CorrectAfterlife { get; set; }
    public string AssignedAfterlife { get; set; }
    public int Level { get; set; }
    public Sprite ShadeImage { get; set; }
    public GameObject TextBubbleInstance { get; set; }
    public GameObject AssociatedGameObject { get; set; }
    // Constructor
    public Shade(string name, string origin, string occupation, string lifeSummary, Sprite shadeImage, string lifeAction1, string lifeAction2, int level)
    {
        Name = name;
        Origin = origin;
        Occupation = occupation;
        LifeSummary = lifeSummary;
        LifeAction1 = lifeAction1;
        LifeAction2 = lifeAction2;
        Level = level;
        ShadeImage = shadeImage;

        // Determine the correct afterlife
        int goodActions = 0;
        if (ShadeSpawner.GoodActions.Contains(lifeAction1)) goodActions++;
        if (ShadeSpawner.GoodActions.Contains(lifeAction2)) goodActions++;

        if (goodActions == 2)
        {
            CorrectAfterlife = "Elysium";
        }
        else if (goodActions == 0)
        {
            CorrectAfterlife = "Tartarus";
        }
        else
        {
            CorrectAfterlife = "Asphodel";
        }

        AssignedAfterlife = null;
        TextBubbleInstance = null;
        AssociatedGameObject = null;
    }

    // Override ToString to include the new properties
    public override string ToString()
    {
        return $"Name: {Name}\n" +
               $"Origin: {Origin}\n" +
               $"Occupation: {Occupation}\n" +
               $"Life Summary: {LifeSummary}\n" +
               $"Life Action 1: {LifeAction1}\n" +
               $"Life Action 2: {LifeAction2}\n" +
               $"Level: {Level}";
    }
}
