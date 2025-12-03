﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choice UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    private static DialogueManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Có nhiều hơn 1 dialogue manager trong scene");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        
        // Bind external function
        currentStory.BindExternalFunction("get_quest_state", (string quest_id) => 
        {
            int state = QuestManager.Instance.GetQuestState(quest_id);
            Debug.Log($"[Ink] get_quest_state({quest_id}) = {state}");
            return state;
        });
        
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        
        if (currentStory.canContinue)
        {
            ContinueStory();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            HandleTags(currentStory.currentTags);
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    // ========== GENERIC TAG HANDLER ==========
private void HandleTags(List<string> tags)
{
    foreach (string tag in tags)
    {
        Debug.Log($"[Tag] {tag}");
        
        string[] parts = tag.Split(':');
        string command = parts[0].Trim();

        // ========== COLLECT QUEST (ƯU TIÊN) ==========
        if (command == "COLLECT_QUEST" && parts.Length >= 4)
        {
            string questID = parts[1].Trim();
            string itemName = parts[2].Trim();
            int count = int.Parse(parts[3].Trim());
            
            Debug.Log($"[Dialogue] Starting Collect Quest: {questID} - {itemName} x{count}");
            QuestManager.Instance.StartCollectQuest(questID, itemName, count);
        }
        
        // ========== CHECK QUEST (MỚI) ==========
        else if (command == "CHECK_QUEST" && parts.Length >= 2)
        {
            string questID = parts[1].Trim();
            
            if (QuestManager.Instance.quests.ContainsKey(questID))
            {
                var quest = QuestManager.Instance.quests[questID];
                
                // ✅ CHỈ KIỂM TRA NẾU QUEST ĐANG Ở STATE InProgress (1)
                if (quest.state == QuestState.InProgress)
                {
                    // Đếm lại item trong inventory
                    int inventoryCount = QuestInventoryManager.Instance.CountItem(quest.targetName);
                    quest.currentCount = inventoryCount;
                    
                    Debug.Log($"[Dialogue] Checking {questID}: {inventoryCount}/{quest.targetCount} in inventory");
                    
                    // Nếu đủ → chuyển state sang WaitingReport
                    if (inventoryCount >= quest.targetCount)
                    {
                        quest.state = QuestState.WaitingReport;
                        QuestManager.Instance.ShowQuestUI($"Đã có đủ {quest.targetName}! Hãy nộp cho NPC");
                        Debug.Log($"[Quest] {questID} completed after inventory check!");
                    }
                    else
                    {
                        QuestManager.Instance.ShowQuestUI($"Chưa đủ: {inventoryCount}/{quest.targetCount} {quest.targetName}");
                        Debug.Log($"[Quest] {questID} not complete: {inventoryCount}/{quest.targetCount}");
                    }
                }
                else if (quest.state == QuestState.WaitingReport)
                {
                    // ✅ NẾU ĐÃ Ở STATE 2 (WaitingReport) → KHÔNG LÀM GÌ CẢ
                    Debug.Log($"[Quest] {questID} already completed, waiting for NPC dialogue");
                }
                else
                {
                    Debug.Log($"[Quest] {questID} is in state {quest.state}, no check needed");
                }
            }
        }
        // ========== REMOVE ITEMS ==========
        else if (command == "REMOVE_ITEMS" && parts.Length >= 3)
        {
            string itemName = parts[1].Trim();
            int amount = int.Parse(parts[2].Trim());
            
            QuestInventoryManager.Instance.RemoveItems(itemName, amount);
            Debug.Log($"[Dialogue] Removed {amount}x {itemName} from inventory");
        }
        
        // ========== KILL QUEST ==========
        else if (command == "KILL_QUEST" && parts.Length >= 4)
        {
            string questID = parts[1].Trim();
            string enemyName = parts[2].Trim();
            int count = int.Parse(parts[3].Trim());
            QuestManager.Instance.StartKillQuest(questID, enemyName, count);
        }

        // ========== QUEST COMPLETE ==========
        else if (command == "QUEST_COMPLETE" && parts.Length >= 2)
        {
            string questID = parts[1].Trim();
            QuestManager.Instance.SetQuestState(questID, 5); 
            Debug.Log($"[Dialogue] Quest {questID} fully completed (State 5)");
        }

        // ========== REWARD ==========
        else if (command == "REWARD" && parts.Length >= 3)
        {
            string rewardType = parts[1].Trim();
            int amount = int.Parse(parts[2].Trim());
            QuestManager.Instance.GiveReward(rewardType, amount);
        }

        // ========== QUEST PROGRESS ==========
        else if (command == "QUEST_PROGRESS" && parts.Length >= 3)
        {
            string questID = parts[1].Trim();
            int newState = int.Parse(parts[2].Trim());
            QuestManager.Instance.SetQuestState(questID, newState);
        }

        // ========== SHOW UI ==========
        else if (command == "SHOW_UI" && parts.Length >= 2)
        {
            string message = string.Join(":", parts, 1, parts.Length - 1).Trim();
            QuestManager.Instance.ShowQuestUI(message);
        }
    }
}

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}