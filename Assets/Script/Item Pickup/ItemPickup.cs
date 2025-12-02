using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private string itemName = "Thảo Dược Đỏ";  // Phải đúng chính tả, dấu
    [SerializeField] private string questID = "";
    
    [Header("Visual")]
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 2 DÒNG QUAN TRỌNG NHẤT - ĐÃ THIẾU TRƯỚC ĐÂY!!!
            QuestManager.Instance.OnItemCollected(itemName);        // ← TĂNG COUNT QUEST + UPDATE UI
            QuestInventoryManager.Instance.AddItem(itemName);       // ← THÊM VÀO TÚI ĐỒ

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Debug.Log($"[Pickup] Collected: {itemName}");
            Destroy(gameObject);
        }
    }
}