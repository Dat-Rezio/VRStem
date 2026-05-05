using UnityEngine;
using System.Collections;

public class GameResetManager : MonoBehaviour
{
    public static GameResetManager Instance;

    private void Awake() => Instance = this;

    public void ReplayGame()
    {
        StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        // 1. Ẩn completePanel (nếu bạn gọi từ đó)
        if (PlanetQuiz.Instance.completePanel != null)
            PlanetQuiz.Instance.completePanel.SetActive(false);

        // 2. Zoom out về ban đầu
        if (SolarSystemFocus.Instance != null)
            SolarSystemFocus.Instance.ZoomOut();

        // Đợi zoom out xong
        yield return new WaitForSeconds(SolarSystemFocus.Instance.zoomSpeed + 0.2f);

        // 3. Reset quiz
        PlanetQuiz.Instance.ResetQuiz();

        // 4. Reset label "?" trên tất cả hành tinh
        PlanetVisual[] allVisuals = FindObjectsOfType<PlanetVisual>();
        foreach (var v in allVisuals)
            v.ShowQuestionMark();

        // 5. Reset dialogue đã phát
        if (AnswerTrigger.Instance != null)
        {
            foreach (var entry in AnswerTrigger.Instance.planetDialogues)
            {
                if (entry.descriptionSequence != null)
                    entry.descriptionSequence.hasBeenPlayed = false;
            }
        }
    }
}