using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Docks : MonoBehaviour
{
    // Queue for shades awaiting processing
    private Queue<Shade> processingQueue = new Queue<Shade>();

    // Number of shades processed per day
    [SerializeField]
    private int dailyProcessingLimit = 5;

    // Event for when a shade is processed
    public delegate void ShadeProcessedHandler(Shade processedShade);
    public event ShadeProcessedHandler OnShadeProcessed;

    // Function to assign a shade to the docks for processing
    public void AssignShadeToDocks(Shade shade)
    {
        if (shade != null)
        {
            processingQueue.Enqueue(shade);
            Debug.Log($"{shade.Name} has been added to the processing queue.");
        }
        else
        {
            Debug.LogWarning("Attempted to assign a null shade to the docks.");
        }
    }

    // Function to process shades daily
    public void ProcessShades()
    {
        int processedCount = 0;

        while (processingQueue.Count > 0 && processedCount < dailyProcessingLimit)
        {
            Shade shadeToProcess = processingQueue.Dequeue();
            ProcessShade(shadeToProcess);
            processedCount++;
        }

        Debug.Log($"Docks processed {processedCount} shade(s) today.");
    }

    // Internal function to handle the processing logic for a single shade
    private void ProcessShade(Shade shade)
    {
        // Logic for processing the shade (e.g., send to assigned afterlife)
        Debug.Log($"Processing shade: {shade.Name}, assigned to {shade.AssignedAfterlife}");

        // Check if the assigned afterlife matches the correct afterlife
        if (shade.AssignedAfterlife == shade.CorrectAfterlife)
        {
            Debug.Log($"Shade {shade.Name} was correctly assigned to {shade.AssignedAfterlife}.");
            // Add logic here for correctly assigned shades (e.g., reward player, increase score, etc.)
        }
        else
        {
            Debug.Log($"Shade {shade.Name} was incorrectly assigned to {shade.AssignedAfterlife}. Correct afterlife was {shade.CorrectAfterlife}.");
            // Add logic here for incorrectly assigned shades (e.g., penalty, feedback to player, etc.)
        }
        // Trigger event for additional logic if needed
        OnShadeProcessed?.Invoke(shade);

        // Clean up or additional processing logic
        // For example, removing from the game or marking as processed
    }

    // Function to get the current queue count
    public int GetQueueCount()
    {
        return processingQueue.Count;
    }
}
