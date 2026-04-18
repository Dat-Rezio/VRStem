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
        
        // THÊM MỚI: Danh sách file âm thanh tương ứng với từng câu thoại
        [Tooltip("Kéo thả file mp3/wav tương ứng với từng câu Text ở trên vào đây")]
        public AudioClip[] voiceovers; 
        
        public bool waitForAction; 
    }

    [Header("--- Cấu hình Kịch bản ---")]
    public List<TutorialStage> stages;

    [Header("--- Cấu hình UI & Âm thanh ---")]
    public GameObject tutorialUI; 
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public Button prevButton; 
    
    // THÊM MỚI: Nguồn phát âm thanh
    public AudioSource audioSource; 

    private int currentStageIndex = 0;
    private int currentDialogueIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClicked);
        if (prevButton != null) prevButton.onClick.AddListener(OnPrevButtonClicked); 
        
        StartStage(0);
    }

    public void StartStage(int stageIndex)
    {
        if (stageIndex >= stages.Count)
        {
            tutorialUI.SetActive(false);
            if (audioSource != null) audioSource.Stop(); // Tắt tiếng khi xong hướng dẫn
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

    public void OnPrevButtonClicked()
    {
        currentDialogueIndex--;

        if (currentDialogueIndex < 0)
        {
            if (currentStageIndex > 0)
            {
                currentStageIndex--;
                currentDialogueIndex = stages[currentStageIndex].dialogues.Length - 1;
            }
            else
            {
                currentDialogueIndex = 0;
            }
        }
        UpdateDialogueUI();
    }

    private void UpdateDialogueUI()
    {
        // 1. Cập nhật Text
        if (dialogueText != null)
        {
            dialogueText.text = stages[currentStageIndex].dialogues[currentDialogueIndex];
        }

        // 2. THÊM MỚI: Xử lý phát âm thanh
        PlayCurrentVoiceover();

        // 3. Xử lý nút bấm
        bool isLastSentence = currentDialogueIndex == stages[currentStageIndex].dialogues.Length - 1;
        bool waitingForTask = stages[currentStageIndex].waitForAction;
        nextButton.gameObject.SetActive(!(isLastSentence && waitingForTask));

        if (prevButton != null)
        {
            bool isFirstOverall = (currentStageIndex == 0 && currentDialogueIndex == 0);
            prevButton.gameObject.SetActive(!isFirstOverall);
        }
    }

    // THÊM MỚI: Hàm phát âm thanh an toàn
    private void PlayCurrentVoiceover()
    {
        if (audioSource == null) return;

        // Dừng câu nói cũ (nếu người chơi bấm Next quá nhanh)
        audioSource.Stop();

        // Kiểm tra xem giai đoạn này có file âm thanh không, và mảng âm thanh có đủ độ dài không
        // (Phòng trường hợp bạn lỡ gõ 3 câu Text nhưng mới chỉ kéo vào 2 file Audio)
        if (stages[currentStageIndex].voiceovers != null && 
            currentDialogueIndex < stages[currentStageIndex].voiceovers.Length)
        {
            AudioClip clip = stages[currentStageIndex].voiceovers[currentDialogueIndex];
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
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