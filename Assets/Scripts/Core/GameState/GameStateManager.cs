using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    #region Enums
    public enum GamePhase
    {
        Dawn,
        Day,
        Evening
    }

    public enum ResourceType
    {
        Obols,
        Herbs,
        Gemstones,
        EntertainmentSporting,
        EntertainmentCreativity,
        EntertainmentIntellectual
    }
    #endregion

    #region Events
    public event Action<GamePhase> OnPhaseChanged;
    public event Action<int> OnDayChanged;
    public event Action<ResourceType, int> OnResourceChanged;
    public event Action<string, bool> OnFlagChanged;
    public event Action<Dictionary<string, bool>> OnMultipleFlagsChanged;
    #endregion

    #region Properties
    public GamePhase CurrentPhase { get; private set; }
    public int CurrentDay { get; private set; }
    private const int MAX_DAYS = 30; // This is used to set the gameplay length

    // Resources
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    // Olympian relationships
    private Dictionary<string, int> olympianRelationships = new Dictionary<string, int>();

    // Dialogue and story flags
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();
    #endregion

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
        InitializeResources();
        InitializeOlympianRelationships();
    }

    private void InitializeResources()
    {
        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resource] = 0;
        }

        // Starting resources as per GDD
    }

    private void InitializeOlympianRelationships()
    {
        // Initialize relationships for all Olympians from GDD
        string[] olympians = {
            "Artemis", "Dionysus", "Hermes", "Athena", "Hephaestus",
            "Ares", "Demeter", "Aphrodite", "Apollo", "Hera",
            "Hades", "Poseidon", "Zeus"
        };

        foreach (string olympian in olympians)
        {
            olympianRelationships[olympian] = 0;
        }
    }
    #endregion

    #region Phase Management
    public void AdvancePhase()
    {
        switch (CurrentPhase)
        {
            case GamePhase.Dawn:
                SetPhase(GamePhase.Day);
                break;
            case GamePhase.Day:
                SetPhase(GamePhase.Evening);
                break;
            case GamePhase.Evening:
                AdvanceDay();
                SetPhase(GamePhase.Dawn);
                break;
        }
    }

    private void SetPhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;
        OnPhaseChanged?.Invoke(newPhase);
    }

    private void AdvanceDay()
    {
        CurrentDay++;
        OnDayChanged?.Invoke(CurrentDay);

        if (CurrentDay > MAX_DAYS)
        {
            TriggerGameEnd();
        }
    }

    private void TriggerGameEnd()
    {
        // Trigger ending sequence based on various conditions
        // This will be implemented when we have the ending system in place
    }
    #endregion

    #region Resource Management
    public int GetResource(ResourceType type)
    {
        return resources[type];
    }

    public bool ModifyResource(ResourceType type, int amount)
    {
        // For expenses, check if we have enough
        if (amount < 0 && resources[type] + amount < 0)
        {
            return false;
        }

        resources[type] += amount;
        OnResourceChanged?.Invoke(type, resources[type]);
        return true;
    }
    #endregion

    #region Relationship Management
    public int GetRelationship(string olympianName)
    {
        return olympianRelationships.TryGetValue(olympianName, out int value) ? value : 0;
    }

    public void ModifyRelationship(string olympianName, int amount)
    {
        if (olympianRelationships.ContainsKey(olympianName))
        {
            olympianRelationships[olympianName] += amount;
        }
    }
    #endregion

    #region Flag Management
    public void SetFlag(string flagName, bool value)
    {
        flags[flagName] = value;
        OnFlagChanged?.Invoke(flagName, value);
    }

    public bool GetFlag(string flagName)
    {
        return flags.TryGetValue(flagName, out bool value) && value;
    }

    public bool HasFlag(string flagName)
    {
        return flags.ContainsKey(flagName);
    }

    public void RemoveFlag(string flagName)
    {
        if (flags.ContainsKey(flagName))
        {
            flags.Remove(flagName);
            OnFlagChanged?.Invoke(flagName, false);
        }
    }

    // Batch Flag Operations
    public void SetFlags(Dictionary<string, bool> flagsToSet)
    {
        foreach (var flag in flagsToSet)
        {
            flags[flag.Key] = flag.Value;
        }
        OnMultipleFlagsChanged?.Invoke(flagsToSet);
    }

    public Dictionary<string, bool> GetFlagsWithPrefix(string prefix)
    {
        return flags
            .Where(kvp => kvp.Key.StartsWith(prefix))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public bool AreAllFlagsSet(params string[] flagNames)
    {
        return flagNames.All(flagName => GetFlag(flagName));
    }

    public bool IsAnyFlagSet(params string[] flagNames)
    {
        return flagNames.Any(flagName => GetFlag(flagName));
    }

    public void RemoveAllFlagsWithPrefix(string prefix)
    {
        var flagsToRemove = flags.Keys
            .Where(key => key.StartsWith(prefix))
            .ToList();

        foreach (var flag in flagsToRemove)
        {
            RemoveFlag(flag);
        }
    }
    #endregion

    #region Save/Load System Hooks
    public GameSaveData GetSaveData()
    {
        return new GameSaveData
        {
            currentDay = CurrentDay,
            currentPhase = CurrentPhase,
            resources = new Dictionary<ResourceType, int>(resources),
            relationships = new Dictionary<string, int>(olympianRelationships),
            flags = new Dictionary<string, bool>(flags)
        };
    }

    public void LoadSaveData(GameSaveData saveData)
    {
        CurrentDay = saveData.currentDay;
        CurrentPhase = saveData.currentPhase;
        resources = new Dictionary<ResourceType, int>(saveData.resources);
        olympianRelationships = new Dictionary<string, int>(saveData.relationships);
        flags = new Dictionary<string, bool>(saveData.flags);

        // Trigger events to update UI
        OnDayChanged?.Invoke(CurrentDay);
        OnPhaseChanged?.Invoke(CurrentPhase);
        foreach (var resource in resources)
        {
            OnResourceChanged?.Invoke(resource.Key, resource.Value);
        }
        foreach (var flag in flags)
        {
            OnFlagChanged?.Invoke(flag.Key, flag.Value);
        }
    }
    #endregion
}
