using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlagManager : MonoBehaviour
{
    [SerializeField] private List<FlagInteractable> flagList;
    [SerializeField] private TextMeshProUGUI flagCountText;
    [SerializeField] private GameObject _lobbyPanel;

    private Dictionary<string, FlagInteractable> flagDictionary;

    void Awake()
    {
        flagDictionary = new Dictionary<string, FlagInteractable>();

        foreach (FlagInteractable flag in flagList)
        {
            if (flag == null)
            {
                Debug.LogWarning("Null flag reference found in list. Skipping.");
                continue;
            }

            string key = flag.name; // Can use custom IDs instead

            if (!flagDictionary.ContainsKey(key))
            {
                flagDictionary.Add(key, flag);
                flag.gameObject.SetActive(true); // Enable flag that’s in dictionary
            }
            else
            {
                Debug.LogWarning($"Duplicate flag key '{key}' found. Disabling duplicate.");
                flag.gameObject.SetActive(false); // Disable duplicates
            }
            UpdateFlagCountText();
        }

        // Disable any flags not added to the dictionary (safety net)
        foreach (FlagInteractable flag in flagList)
        {
            if (flag == null || !flagDictionary.ContainsValue(flag))
            {
                flag?.gameObject.SetActive(false);
            }
        }
    }
    private void UpdateFlagCountText()
    {
        if (flagCountText != null)
        {
            flagCountText.text = $"Flags Remaining: {flagDictionary.Count}";
        }

        if (flagDictionary.Count < 1)
        {
            Victory();
        }
    }
    
    public void InteractWithFlag(string flagName)
    {
        if (flagDictionary.TryGetValue(flagName, out FlagInteractable flag))
        {
            flag.SendMessage("PrimaryInputHandler", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning($"Flag '{flagName}' not found in the dictionary.");
        }
    }

    public FlagInteractable GetFlag(string flagName)
    {
        flagDictionary.TryGetValue(flagName, out FlagInteractable flag);
        return flag;
    }

    public void Victory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _lobbyPanel.SetActive(!_lobbyPanel.activeSelf);
    }

    public void RemoveFlag(FlagInteractable flag)
    {
        string keyToRemove = null;

        foreach (var pair in flagDictionary)
        {
            if (pair.Value == flag)
            {
                keyToRemove = pair.Key;
                break;
            }
        }

        if (keyToRemove != null)
        {
            flagDictionary.Remove(keyToRemove);
            Debug.Log($"Flag '{keyToRemove}' removed from the dictionary.");
            UpdateFlagCountText();
        }
    }
}