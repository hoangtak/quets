using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum QuestType 
{ 
    Dialogue,    // Nói chuyện NPC
    Kill,        // Tiêu diệt quái
    Collect      // Thu thập vật phẩm
}

public enum QuestState 
{ 
    NotStarted = 0,
    InProgress = 1,
    WaitingReport = 2,
    InProgress2 = 3,
    WaitingFinal = 4,
    Completed = 5
}

[System.Serializable]
public class QuestData
{
    public string questID;
    public QuestType type;
    public QuestState state;
    
    // Cho quest Kill và Collect
    public int currentCount;  // Số lượng hiện tại
    public int targetCount;   // Mục tiêu cần đạt
    public string targetName; // Tên quái/vật phẩm
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Dictionary lưu tất cả quest
    public Dictionary<string, QuestData> quests = new Dictionary<string, QuestData>();

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

    // ========== KHỞI TẠO QUEST ==========
    
public void InitQuest(string questID, QuestType type = QuestType.Dialogue)
{
    if (!quests.ContainsKey(questID))
    {
        quests[questID] = new QuestData
        {
            questID = questID,
            type = type,
            state = QuestState.NotStarted,
            currentCount = 0,
            targetCount = 0
        };
        Debug.Log($"[Quest] Initialized: {questID} (Type: {type})");
    }
    else
    {
        // ✅ THÊM: Cập nhật type nếu quest đã tồn tại
        if (quests[questID].type != type)
        {
            Debug.Log($"[Quest] Updating {questID} type: {quests[questID].type} → {type}");
            quests[questID].type = type;
        }
    }
}

    // ========== DIALOGUE QUEST (GIỮ NGUYÊN) ==========
    
    public void StartQuest(string questID)
    {
        InitQuest(questID);
        
        if (quests[questID].state == QuestState.NotStarted)
        {
            quests[questID].state = QuestState.InProgress;
            Debug.Log($"[Quest] Started: {questID} → State = 1");
        }
    }

    public void SetQuestState(string questID, int newState)
    {
        InitQuest(questID);
        quests[questID].state = (QuestState)newState;
        Debug.Log($"[Quest] {questID} → State = {newState}");
    }

    public int GetQuestState(string questID)
    {
        InitQuest(questID);
        return (int)quests[questID].state;
    }

    // ========== KILL QUEST (MỚI) ==========
    
    public void StartKillQuest(string questID, string enemyName, int targetCount)
    {
        InitQuest(questID, QuestType.Kill);
        
        quests[questID].state = QuestState.InProgress;
        quests[questID].targetName = enemyName;
        quests[questID].targetCount = targetCount;
        quests[questID].currentCount = 0;
        
        ShowQuestUI($"Tiêu diệt {enemyName}: 0/{targetCount}");
        Debug.Log($"[Quest] Kill Quest Started: {questID} - Kill {targetCount} {enemyName}");
    }

    public void OnEnemyKilled(string enemyName)
    {
        foreach (var quest in quests.Values)
        {
            if (quest.type == QuestType.Kill && 
                quest.state == QuestState.InProgress && 
                quest.targetName == enemyName)
            {
                quest.currentCount++;
                
                ShowQuestUI($"Tiêu diệt {enemyName}: {quest.currentCount}/{quest.targetCount}");
                Debug.Log($"[Quest] Enemy killed: {quest.currentCount}/{quest.targetCount}");
                
                // Kiểm tra hoàn thành
                if (quest.currentCount >= quest.targetCount)
                {
                    quest.state = QuestState.WaitingReport;
                    ShowQuestUI($"Đã tiêu diệt đủ! Quay lại báo cáo NPC");
                }
            }
        }
    }

    // ========== COLLECT QUEST (MỚI) ==========
    
public void StartCollectQuest(string questID, string itemName, int targetCount)
{
    InitQuest(questID, QuestType.Collect);
    
    quests[questID].state = QuestState.InProgress;
    quests[questID].targetName = itemName;
    quests[questID].targetCount = targetCount;
    
    // ✅ KIỂM TRA INVENTORY TRƯỚC KHI BẮT ĐẦU
    int inventoryCount = QuestInventoryManager.Instance.CountItem(itemName);
    quests[questID].currentCount = inventoryCount;  // Đặt count = số đã có
    
    Debug.Log($"[Quest] Collect Quest Started: {questID} - {itemName} ({inventoryCount}/{targetCount} already collected)");
    
    // ✅ NẾU ĐÃ ĐỦ → HOÀN THÀNH LUÔN
    if (inventoryCount >= targetCount)
    {
        quests[questID].state = QuestState.WaitingReport;
        ShowQuestUI($"Đã có đủ {itemName}! Hãy nộp cho NPC");
        Debug.Log($"[Quest] {questID} auto-completed! (Had {inventoryCount}/{targetCount})");
    }
    else
    {
        ShowQuestUI($"Thu thập {itemName}: {inventoryCount}/{targetCount}");
    }
}

public void OnItemCollected(string itemName)
{
    Debug.Log($"[Quest] OnItemCollected called: '{itemName}'");
    
    foreach (var quest in quests.Values)
    {
        Debug.Log($"[Quest] Checking {quest.questID}: Type={quest.type}, State={quest.state}, Target='{quest.targetName}'");
        
        if (quest.type == QuestType.Collect && 
            quest.state == QuestState.InProgress && 
            quest.targetName == itemName)
        {
            quest.currentCount++;
            
            Debug.Log($"[Quest] ✅ MATCH! {quest.questID}: {quest.currentCount}/{quest.targetCount}");
            
            ShowQuestUI($"Thu thập {itemName}: {quest.currentCount}/{quest.targetCount}");
            
            if (quest.currentCount >= quest.targetCount)
            {
                quest.state = QuestState.WaitingReport;
                Debug.Log($"[Quest] 🎉 {quest.questID} COMPLETED! State → 2");
                ShowQuestUI($"Đã thu thập đủ! Quay lại báo cáo NPC");
            }
            return; // Thoát ngay khi tìm thấy
        }
    }
    
    Debug.LogWarning($"[Quest] ❌ No matching quest found for item: '{itemName}'");
}

    // ========== QUEST PROGRESS (MỚI) ==========
    
    public int GetQuestProgress(string questID)
    {
        if (quests.ContainsKey(questID))
        {
            return quests[questID].currentCount;
        }
        return 0;
    }

    public bool IsQuestComplete(string questID)
    {
        if (quests.ContainsKey(questID))
        {
            var quest = quests[questID];
            
            // Dialogue quest
            if (quest.type == QuestType.Dialogue)
            {
                return quest.state == QuestState.Completed;
            }
            
            // Kill/Collect quest
            return quest.currentCount >= quest.targetCount;
        }
        return false;
    }

    // ========== HOÀN THÀNH QUEST ==========
    
    public void CompleteQuest(string questID)
    {
        if (quests.ContainsKey(questID))
        {
            quests[questID].state = QuestState.Completed;
            Debug.Log($"[Quest] Completed: {questID}");
        }
    }

    // ========== UI & REWARD ==========
    
    public void ResetQuest(string questID) {
    if (quests.ContainsKey(questID)) {
        quests[questID].state = QuestState.NotStarted;
        quests[questID].currentCount = 0;
    }
    Debug.Log($"[RESET] {questID} reset");
}

    public void ShowQuestUI(string message)
    {
        QuestUI.Instance.ShowQuest(message);
        Debug.Log($"[Quest UI] {message}");
    }

    public void GiveReward(string rewardType, int amount)
    {
        Debug.Log($"[Reward] +{amount} {rewardType}");
        
        // TODO: Implement inventory system
        if (rewardType == "gold")
        {
            // PlayerInventory.AddGold(amount);
        }
        else if (rewardType == "potion")
        {
            // PlayerInventory.AddPotion(amount);
        }
        else if (rewardType == "sword")
        {
            // PlayerInventory.AddWeapon("sword");
        }
    }
}