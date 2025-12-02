using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
public class Quest
{
    public string questID;
    public QuestState state;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Dictionary để lưu tất cả quest
    private Dictionary<string, QuestState> questStates = new Dictionary<string, QuestState>();

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

    // Khởi tạo quest
    public void InitQuest(string questID)
    {
        if (!questStates.ContainsKey(questID))
        {
            questStates[questID] = QuestState.NotStarted;
            Debug.Log($"[Quest] Initialized quest: {questID}");
        }
    }

    // Bắt đầu quest
    public void StartQuest(string questID)
    {
        InitQuest(questID);
        
        if (questStates[questID] == QuestState.NotStarted)
        {
            questStates[questID] = QuestState.InProgress;
            Debug.Log($"[Quest] Started: {questID} → State = 1");
        }
    }

    // Cập nhật state của quest
    public void SetQuestState(string questID, int newState)
    {
        InitQuest(questID);
        
        questStates[questID] = (QuestState)newState;
        Debug.Log($"[Quest] {questID} → State = {newState}");
    }

    // Hoàn thành quest
    public void CompleteQuest(string questID)
    {
        InitQuest(questID);
        
        questStates[questID] = QuestState.Completed;
        Debug.Log($"[Quest] Completed: {questID}");
    }

    // Lấy state của quest (cho Ink)
    public int GetQuestState(string questID)
    {
        InitQuest(questID);
        return (int)questStates[questID];
    }

    // Hiển thị UI quest
    public void ShowQuestUI(string message)
    {
        QuestUI.Instance.ShowQuest(message);
        Debug.Log($"[Quest UI] {message}");
    }

    // Tặng thưởng
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
    }
}