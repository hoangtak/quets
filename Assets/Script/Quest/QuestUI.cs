using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [SerializeField] private TextMeshProUGUI questText;

    private void Awake()
    {
        Instance = this;
        questText.text = "";
    }

    public void ShowQuest(string text)
    {
        questText.text = text;
    }

    public void ClearQuest()
    {
        questText.text = "";
    }
}
