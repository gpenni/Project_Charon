using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ShadeClickHandler : MonoBehaviour
{
    private Shade shade;
    private GameObject textBubblePrefab;
    private Canvas uiCanvas;
    private GameObject activeBubble;
    public ShadeManager shadeManager;
    public Docks docks;

    public void Initialize(Shade shade, GameObject textBubblePrefab, Canvas uiCanvas, ShadeManager shadeManager, Docks docks)
    {
        this.shade = shade;
        this.textBubblePrefab = textBubblePrefab;
        this.uiCanvas = uiCanvas;
        this.shadeManager = shadeManager;
        this.docks = docks;
    }

    private void OnMouseDown()
    {
        // Prevent destruction if a UI element was clicked
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (activeBubble != null)
        {
            Destroy(activeBubble); // Close existing bubble if clicked again
            activeBubble = null;
            shade.TextBubbleInstance = null;
            return;
        }

        // Create a new text bubble
        activeBubble = Instantiate(textBubblePrefab, uiCanvas.transform);
        shade.TextBubbleInstance = activeBubble;


        // Set the text using TextMeshPro
        TMP_Text infoText = activeBubble.GetComponentInChildren<TMP_Text>();
        if (infoText != null)
        {
            infoText.text = $"Name: {shade.Name}\nOrigin: {shade.Origin}\nOccupation: {shade.Occupation}\nLife Summary: {shade.LifeSummary}\nLife Action 1: {shade.LifeAction1}\n Life Action 2: {shade.LifeAction2}\nLevel: {shade.Level}";
        }
        else
        {
            Debug.LogWarning("No TMP_Text component found in the text bubble prefab.");
        }

        Button[] buttons = activeBubble.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            if (button.name == "HiredButton" && shadeManager != null)
            {
                button.onClick.AddListener(() => shadeManager.Hired(this.gameObject));
            }
            else if (button.name == "DocksButton")
            {
                button.onClick.AddListener(() => OpenAssignAfterlifeMenu());
            }
        }


        // Position the bubble above the shade
        Vector3 worldPosition = transform.position + new Vector3(0, 4f, 0); // Offset above the Shade
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        activeBubble.transform.position = screenPosition;
    }
    private void OpenAssignAfterlifeMenu()
    {
        // Create a new panel or menu for selecting afterlife
        GameObject assignAfterlifePanel = new GameObject("AssignAfterlifePanel");
        assignAfterlifePanel.transform.SetParent(uiCanvas.transform, false);

        RectTransform panelRect = assignAfterlifePanel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(300, 300);
        panelRect.anchoredPosition = Vector2.zero;

        CanvasGroup canvasGroup = assignAfterlifePanel.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;

        // Background image for the panel
        Image panelImage = assignAfterlifePanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black background

        // Add buttons for each afterlife
        string[] afterlifeOptions = { "Elysium", "Asphodel", "Tartarus" };
        Vector2 buttonSize = new Vector2(200, 50);
        float buttonSpacing = 60f;

        for (int i = 0; i < afterlifeOptions.Length; i++)
        {
            string afterlife = afterlifeOptions[i];

            // Create button
            GameObject buttonObject = new GameObject(afterlife + "Button");
            buttonObject.transform.SetParent(assignAfterlifePanel.transform, false);

            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.sizeDelta = buttonSize;
            buttonRect.anchoredPosition = new Vector2(0, (afterlifeOptions.Length - 1 - i) * -buttonSpacing + 30);

            Button button = buttonObject.AddComponent<Button>();
            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(1, 1, 1, 0.9f); // White with slight transparency

            // Add text to button
            GameObject buttonTextObject = new GameObject("Text");
            buttonTextObject.transform.SetParent(buttonObject.transform, false);

            TextMeshProUGUI buttonText = buttonTextObject.AddComponent<TextMeshProUGUI>();
            buttonText.text = afterlife;
            buttonText.fontSize = 24;
            buttonText.color = Color.black;
            buttonText.alignment = TextAlignmentOptions.Center;

            RectTransform textRect = buttonTextObject.GetComponent<RectTransform>();
            textRect.sizeDelta = buttonSize;
            textRect.anchoredPosition = Vector2.zero;

            // Assign functionality to button
            button.onClick.AddListener(() =>
            {
                AssignAfterlife(afterlife);
                Destroy(assignAfterlifePanel); // Close the menu after selection
            });
        }
    }
    private void AssignAfterlife(string afterlife)
    {
        shade.AssignedAfterlife = afterlife;
        Debug.Log($"{shade.Name} assigned to {shade.AssignedAfterlife}");

        // Remove the shade from the list of spawnedShades
        Shade shadeToRemove = shadeManager.spawnedShades.Find(s => s == shade);
        if (shadeToRemove != null)
        {
            // Assign the shade to the docks
            if (docks != null)
            {
                docks.AssignShadeToDocks(shadeToRemove);
            }
            else
            {
                Debug.LogError("Docks instance is not assigned.");
            }
            shadeManager.spawnedShades.Remove(shadeToRemove);

            if (shadeToRemove.TextBubbleInstance != null)
            {
                Destroy(shadeToRemove.TextBubbleInstance);
                shadeToRemove.TextBubbleInstance = null;
            }

            // Destroy the associated GameObject
            if (shade.AssociatedGameObject != null)
            {
                Destroy(shade.AssociatedGameObject);
            }

            Debug.Log($"Removed shade: {shadeToRemove.Name} after assigning to {shadeToRemove.AssignedAfterlife}");
        }
        else
        {
            Debug.LogWarning("Shade not found in spawnedShades list.");
        }
        RefreshTextBubble();
    }
    private void RefreshTextBubble()
    {
        if (activeBubble != null)
        {
            TMP_Text infoText = activeBubble.GetComponentInChildren<TMP_Text>();
            infoText.text = $"Name: {shade.Name}\nOrigin: {shade.Origin}\nOccupation: {shade.Occupation}\nLife Summary: \n{shade.LifeSummary}\n" +
                            $"Life Action 1: {shade.LifeAction1}\nLife Action 2: {shade.LifeAction2}\n" +
                            $"Correct Afterlife: {shade.CorrectAfterlife}\n" +
                            $"Assigned Afterlife: {(shade.AssignedAfterlife ?? "Not assigned")}";
        }
    }


}
