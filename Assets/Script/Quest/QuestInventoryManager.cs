using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestInventoryManager : MonoBehaviour
{
    public static QuestInventoryManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; // Kéo Panel Inventory vào đây
    [SerializeField] private GameObject slotPrefab;     // Prefab của slot item
    [SerializeField] private Transform contentParent;   // Content của ScrollView (nơi spawn slots)

    public List<string> items = new List<string>();    // Danh sách vật phẩm
    private List<GameObject> slotInstances = new List<GameObject>(); // Các slot UI đang hiển thị

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Toggle inventory bằng phím I
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log($"[Inventory] Added: {itemName}. Total: {items.Count}");
        
        // Chỉ refresh UI nếu inventory đang mở
        if (inventoryPanel.activeSelf)
        {
            RefreshUI();
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        // Xóa slots cũ
        ClearSlots();

        // Tạo slots mới cho từng item
        foreach (string itemName in items)
        {
            GameObject newSlot = Instantiate(slotPrefab, contentParent);
            newSlot.GetComponentInChildren<Text>().text = itemName; // Giả sử slot có Text child
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

    // Thêm hàm này vào QuestInventoryManager
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

// Hàm xóa item khỏi inventory (khi nộp quest)
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
    
    // Refresh UI nếu inventory đang mở
    if (inventoryPanel.activeSelf)
    {
        RefreshUI();
    }
}
}