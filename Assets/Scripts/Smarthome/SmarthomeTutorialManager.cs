using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SmarthomeTutorialManager : MonoBehaviour
{
    public static SmarthomeTutorialManager Instance;

    [System.Serializable]
    public class TutorialStage
    {
        public string stageName; 
        [TextArea(2, 5)] 
        public string[] dialogues; 
        
        [Tooltip("Bật tick nếu yêu cầu người dùng PHẢI làm xong nhiệm vụ mới được đi tiếp")]
        public bool waitForAction; 
    }

    [Header("--- Cấu hình Kịch bản ---")]
    public List<TutorialStage> stages;

    [Header("--- Cấu hình UI ---")]
    public GameObject tutorialUI; 
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public Button prevButton; // THÊM MỚI: Nút quay lại

    private int currentStageIndex = 0;
    private int currentDialogueIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Gắn sự kiện cho các nút bấm
        if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClicked);
        if (prevButton != null) prevButton.onClick.AddListener(OnPrevButtonClicked); // ĐĂNG KÝ SỰ KIỆN QUAY LẠI
        
        // Bắt đầu kịch bản
        StartStage(0);
    }

    public void StartStage(int stageIndex)
    {
        if (stageIndex >= stages.Count)
        {
            tutorialUI.SetActive(false);
            return;
        }

        currentStageIndex = stageIndex;
        currentDialogueIndex = 0;
        tutorialUI.SetActive(true);
        UpdateDialogueUI();
    }

    public void OnNextButtonClicked()
    {
        currentDialogueIndex++;
        
        if (currentDialogueIndex >= stages[currentStageIndex].dialogues.Length)
        {
            if (stages[currentStageIndex].waitForAction)
            {
                nextButton.gameObject.SetActive(false);
                currentDialogueIndex--; 
            }
            else
            {
                StartStage(currentStageIndex + 1);
            }
        }
        else
        {
            UpdateDialogueUI();
        }
    }

    // HÀM XỬ LÝ KHI BẤM NÚT QUAY LẠI
    public void OnPrevButtonClicked()
    {
        currentDialogueIndex--;

        // Nếu lùi quá câu đầu tiên của giai đoạn hiện tại
        if (currentDialogueIndex < 0)
        {
            // Nếu vẫn còn giai đoạn phía trước thì lùi về câu cuối cùng của giai đoạn đó
            if (currentStageIndex > 0)
            {
                currentStageIndex--;
                currentDialogueIndex = stages[currentStageIndex].dialogues.Length - 1;
            }
            else
            {
                // Nếu đang ở câu đầu tiên của toàn bộ kịch bản thì giữ nguyên
                currentDialogueIndex = 0;
            }
        }
        UpdateDialogueUI();
    }

    private void UpdateDialogueUI()
    {
        if (dialogueText != null)
        {
            dialogueText.text = stages[currentStageIndex].dialogues[currentDialogueIndex];
        }

        // Xử lý hiển thị nút Next
        bool isLastSentence = currentDialogueIndex == stages[currentStageIndex].dialogues.Length - 1;
        bool waitingForTask = stages[currentStageIndex].waitForAction;
        nextButton.gameObject.SetActive(!(isLastSentence && waitingForTask));

        // Xử lý hiển thị nút Prev (Ẩn đi nếu là câu đầu tiên của toàn bộ kịch bản)
        if (prevButton != null)
        {
            bool isFirstOverall = (currentStageIndex == 0 && currentDialogueIndex == 0);
            prevButton.gameObject.SetActive(!isFirstOverall);
        }
    }

    public void CompleteTask(string taskName)
    {
        if (stages[currentStageIndex].waitForAction && stages[currentStageIndex].stageName == taskName)
        {
            StartStage(currentStageIndex + 1);
        }
    }
}