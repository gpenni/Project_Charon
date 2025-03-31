using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeSpawner : MonoBehaviour
{
    public GameObject TextBubblePrefab;
    public Canvas UICanvas;

    public int maxSpawnableLevel = 1;

    private static readonly List<string> Names = new List<string>
    {
        "Ephialtes", "Ajax", "Sappho", "Leonidas", "Pythia", "Hector", "Achilles", "Orpheus", "Pandora", "Antigone"
    };

    private static readonly List<string> Origins = new List<string>
    {
        "Hermit", "Noble", "Urchin", "Artisan", "Pilgrim", "Hero", "Iconoclast", "Outlander", "Apprentice"
    };

    private static readonly List<string> Occupations = new List<string>
    {
        "Labourer", "Servant", "Scholar", "Merchant", "Entertainer", "Gladiator", "Warrior", "Sailor"
    };

    private static readonly List<string> LifeSummaries = new List<string>
    {
        "A brave soul who met their end in a tragic battle.",
        "A kind healer known for their selfless acts of compassion.",
        "A cunning merchant who amassed great wealth.",
        "A poet whose words inspired generations.",
        "A sinner who sought redemption in their final moments.",
        "A warrior who defended their homeland against invaders.",
        "A philosopher who sought the meaning of existence.",
        "A hunter skilled in tracking and survival.",
        "A farmer who toiled the earth with dedication.",
        "A scribe who recorded the histories of their time."
    };
    public static readonly List<string> GoodActions = new List<string>
    {
        "Rescued a village from invaders.",
        "Discovered a cure for a deadly disease.",
        "Built a shelter for the homeless.",
        "Taught a generation of scholars.",
        "Protected endangered wildlife."
    };
    public static readonly List<string> BadActions = new List<string>
    {
        "Stole from the royal treasury.",
        "Betrayed their comrades in battle.",
        "Burned down a sacred temple.",
        "Spread rumors that caused chaos.",
        "Sabotaged a critical mission."
    };

    public List<Sprite> ShadeSprites;

    public List<Shade> SpawnShades()
    {
        List<Shade> shades = new List<Shade>();

        Vector3 startPosition = new Vector3(-6, 0, 0); // Adjust starting position as needed
        float spacing = 3.0f;

        for (int i = 0; i < 5; i++)
        {
            string name = Names[UnityEngine.Random.Range(0, Names.Count)];
            string origin = Origins[UnityEngine.Random.Range(0, Origins.Count)];
            string occupation = Occupations[UnityEngine.Random.Range(0, Occupations.Count)];
            string lifeSummary = LifeSummaries[UnityEngine.Random.Range(0, LifeSummaries.Count)];

            string lifeAction1 = UnityEngine.Random.value > 0.5f
                ? GoodActions[UnityEngine.Random.Range(0, GoodActions.Count)]
                : BadActions[UnityEngine.Random.Range(0, BadActions.Count)];

            string lifeAction2 = UnityEngine.Random.value > 0.5f
                ? GoodActions[UnityEngine.Random.Range(0, GoodActions.Count)]
                : BadActions[UnityEngine.Random.Range(0, BadActions.Count)];

            int level = UnityEngine.Random.Range(1, maxSpawnableLevel);

            Sprite shadeImage = null;
            if (ShadeSprites != null && ShadeSprites.Count > 0)
            {
                shadeImage = ShadeSprites[UnityEngine.Random.Range(0, ShadeSprites.Count)];
            }

            // Create and configure the shade GameObject
            GameObject shadeObject = new GameObject($"Shade_{name}"); // Name the GameObject
            shadeObject.transform.position = startPosition + new Vector3(i * spacing, 0, 0); // Set position with spacing
            SpriteRenderer renderer = shadeObject.AddComponent<SpriteRenderer>(); // Add SpriteRenderer
            renderer.sprite = shadeImage; // Assign sprite

            shadeObject.AddComponent<BoxCollider2D>(); // Ensure there's a collider for clicks

            // Create the Shade object
            Shade shade = new Shade(name, origin, occupation, lifeSummary, shadeImage, lifeAction1, lifeAction2, level)
            {
                AssociatedGameObject = shadeObject // Link the GameObject to the Shade
            };
            shades.Add(shade);

            // Attach ShadeClickHandler to handle clicks
            ShadeClickHandler clickHandler = shadeObject.AddComponent<ShadeClickHandler>();
            clickHandler.Initialize(shade, TextBubblePrefab, UICanvas, FindObjectOfType<ShadeManager>(), FindObjectOfType<Docks>());

            Debug.Log($"Shade {shade.Name} is assigned to the afterlife: {shade.CorrectAfterlife}");

            // Log if shade image is null
            if (shadeImage == null)
            {
                Debug.LogWarning($"Shade {name} has a null image. Check ShadeSprites in the inspector.");
            }
        }

        return shades;
    }

}
