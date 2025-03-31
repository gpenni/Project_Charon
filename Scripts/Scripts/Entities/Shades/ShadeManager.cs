using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShadeManager : MonoBehaviour
{
    public ShadeSpawner spawner;
    public GameObject hiredShadesPanel; 
    public Transform hiredShadesContainer; 
    public GameObject shadeInfoPrefab;
    public Transform summaryContainer; 
    public GameObject summaryTextPrefab;

    public List<Shade> spawnedShades = new List<Shade>();
    private List<Shade> hiredShades = new List<Shade>();

    // Start is called before the first frame update
    void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("ShadeSpawner is not assigned in the Inspector!");
            return;
        }

        if (hiredShadesPanel != null)
        {
            hiredShadesPanel.SetActive(false); // Hide the panel initially
        }

        spawnedShades = spawner.SpawnShades();

        foreach (Shade shade in spawnedShades)
        {
            Debug.Log(shade);
        }
    }

    public void Hired(GameObject shadeObject)
    {
        // Find the corresponding shade based on the GameObject
        Shade shadeToHire = spawnedShades.Find(shade => shade.AssociatedGameObject == shadeObject);

        if (shadeToHire != null)
        {
            // Add the shade to the hiredShades list
            hiredShades.Add(shadeToHire);

            // Remove the shade from the spawnedShades list
            spawnedShades.Remove(shadeToHire);

            if (shadeToHire.TextBubbleInstance != null)
            {
                Destroy(shadeToHire.TextBubbleInstance);
                shadeToHire.TextBubbleInstance = null;
            }
            // Destroy the associated GameObject
            Destroy(shadeObject);

            Debug.Log($"Hired shade: {shadeToHire.Name}");
        }
        else
        {
            Debug.LogWarning("Shade not found in spawnedShades list.");
        }
    }

    public void ShowHiredShades()
    {
        if (hiredShadesPanel == null || shadeInfoPrefab == null || hiredShadesContainer == null || summaryContainer == null || summaryTextPrefab == null)
        {
            Debug.LogError("UI elements for hired shades display are not properly assigned!");
            return;
        }

        // Check if the panel is already active
        if (hiredShadesPanel.activeSelf)
        {
            CloseHiredShadesPanel();
            return; // Exit early if closing
        }

        // Clear existing entries
        foreach (Transform child in hiredShadesContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in summaryContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate the panel with hired shades
        foreach (Shade shade in hiredShades)
        {
            GameObject shadeInfo = Instantiate(shadeInfoPrefab, hiredShadesContainer);

            // Populate info
            TMP_Text infoText = shadeInfo.GetComponentInChildren<TMP_Text>();
            if (infoText != null)
            {
                infoText.text = $"Name: {shade.Name}\n" +
                                $"Origin: {shade.Origin}\n" +
                                $"Occupation: {shade.Occupation}\n" +
                                $"Life Summary: {shade.LifeSummary}\n" +
                                $"Life Action 1: {shade.LifeAction1}\n" +
                                $"Life Action 2: {shade.LifeAction2}\n" +
                                $"Level: {shade.Level}"; 
            }
        }
        // Calculate summary of origins and occupations
        Dictionary<string, int> originCounts = new Dictionary<string, int>();
        Dictionary<string, int> occupationCounts = new Dictionary<string, int>();

        foreach (Shade shade in hiredShades)
        {
            // Count origins
            if (!originCounts.ContainsKey(shade.Origin))
            {
                originCounts[shade.Origin] = 0;
            }
            originCounts[shade.Origin]++;

            // Count occupations
            if (!occupationCounts.ContainsKey(shade.Occupation))
            {
                occupationCounts[shade.Occupation] = 0;
            }
            occupationCounts[shade.Occupation]++;
        }

        // Display origin summary
        foreach (var entry in originCounts)
        {
            GameObject originSummary = Instantiate(summaryTextPrefab, summaryContainer);
            TMP_Text originText = originSummary.GetComponentInChildren<TMP_Text>();
            if (originText != null)
            {
                originText.text = $"{entry.Key}: {entry.Value}";
            }
        }

        // Display occupation summary
        foreach (var entry in occupationCounts)
        {
            GameObject occupationSummary = Instantiate(summaryTextPrefab, summaryContainer);
            TMP_Text occupationText = occupationSummary.GetComponentInChildren<TMP_Text>();
            if (occupationText != null)
            {
                occupationText.text = $"{entry.Key}: {entry.Value}";
            }
        }

        // Show the panel
        hiredShadesPanel.SetActive(true);
    }

    public void CloseHiredShadesPanel()
    {
        if (hiredShadesPanel != null)
        {
            hiredShadesPanel.SetActive(false);
        }
    }

    public void SpawnNewShades()
    {
        // Check if spawner exists
        if (spawner == null)
        {
            Debug.LogError("ShadeSpawner is not assigned in the Inspector!");
            return;
        }

        // Remove all remaining un-hired shades
        List<Shade> shadesToRemove = new List<Shade>();

        foreach (Shade shade in spawnedShades)
        {
            if (!hiredShades.Contains(shade)) // If shade is not hired
            {
                if (shade.AssociatedGameObject != null)
                {
                    Destroy(shade.AssociatedGameObject); // Destroy its GameObject
                }
                shadesToRemove.Add(shade); // Mark it for removal
            }
        }

        // Remove un-hired shades from the list
        foreach (Shade shade in shadesToRemove)
        {
            spawnedShades.Remove(shade);
        }

        // Spawn new shades
        List<Shade> newShades = spawner.SpawnShades();

        foreach (Shade shade in newShades)
        {
            Debug.Log($"New Shade Spawned: {shade}");
            spawnedShades.Add(shade); // Add new shades to the list
        }
    }

}
