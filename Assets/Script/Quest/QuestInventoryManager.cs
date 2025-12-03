using UnityEngine;
using UnityEngine.UI;
using TMPro;  // ✅ THÊM DÒNG NÀY
using System.Collections.Generic;

public class QuestInventoryManager : MonoBehaviour
{
    public static QuestInventoryManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform contentParent;

    [Header("Item Database")]
    [SerializeField] private ItemData[] itemDatabase;
    private Dictionary<string, Sprite> itemIcons = new Dictionary<string, Sprite>();

    public List<string> items = new List<string>();
    private List<GameObject> slotInstances = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildItemIconDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
private void BuildItemIconDictionary()
{
    itemIcons.Clear();
    foreach (ItemData item in itemDatabase)
    {
        if (item != null && !string.IsNullOrEmpty(item.itemName))
        {
            if (itemIcons.ContainsKey(item.itemName))
            {
                Debug.LogWarning($"[Inventory] Trùng tên item: {item.itemName}");
            }
            else
            {
                itemIcons.Add(item.itemName, item.icon);
                Debug.Log($"[Inventory] Đã load icon cho: {item.itemName}");
            }
        }
    }
}
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log($"[Inventory] Added: {itemName}. Total: {items.Count}");
        
        if (inventoryPanel.activeSelf)
        {
            RefreshUI();
        }
    }

    public void ToggleInventory()
    {
        bool isOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpen);
        
        if (isOpen)
        {
            RefreshUI();
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

private void RefreshUI()
{
    ClearSlots();

    Dictionary<string, int> itemCounts = new Dictionary<string, int>();
    foreach (string itemName in items)
    {
        if (itemCounts.ContainsKey(itemName))
            itemCounts[itemName]++;
        else
            itemCounts[itemName] = 1;
    }

    foreach (var kvp in itemCounts)
    {
        GameObject newSlot = Instantiate(slotPrefab, contentParent);

        // === ICON ===
        Image iconImage = newSlot.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null)
        {
            
            if (itemIcons.TryGetValue(kvp.Key, out Sprite icon) && icon != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false; // Ẩn nếu không có icon
            }
        }

        // === SỐ LƯỢNG - TÌM ĐÚNG TÊN "CountText" ===
        TextMeshProUGUI countText = newSlot.transform.Find("CountText")?.GetComponent<TextMeshProUGUI>();
        if (countText != null)
        {
            countText.text = kvp.Value > 1 ? $"x{kvp.Value}" : ""; // Ẩn x1 nếu muốn gọn
        }

        slotInstances.Add(newSlot);
    }
}

    private void ClearSlots()
    {
        foreach (GameObject slot in slotInstances)
        {
            Destroy(slot);
        }
        slotInstances.Clear();
    }

    public int CountItem(string itemName)
    {
        int count = 0;
        foreach (string item in items)
        {
            if (item == itemName)
            {
                count++;
            }
        }
        return count;
    }

    public bool HasItems(string itemName, int amount)
    {
        return CountItem(itemName) >= amount;
    }

    public void RemoveItems(string itemName, int amount)
    {
        int removed = 0;
        for (int i = items.Count - 1; i >= 0 && removed < amount; i--)
        {
            if (items[i] == itemName)
            {
                items.RemoveAt(i);
                removed++;
            }
        }
        
        Debug.Log($"[Inventory] Removed {removed}x {itemName}. Remaining: {items.Count}");
        
        if (inventoryPanel.activeSelf)
        {
            RefreshUI();
        }
    }
}