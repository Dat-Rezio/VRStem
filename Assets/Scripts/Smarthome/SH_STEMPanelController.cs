using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SH_STEMPanelController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Kéo Main Camera (hoặc Center Eye Anchor) của XR Rig vào đây")]
    public Transform vrCamera;

    [Header("Fade Settings")]
    [Tooltip("Thời gian panel đứng im trước khi bắt đầu mờ (giây)")]
    public float delayBeforeFade = 3f;
    [Tooltip("Thời gian diễn ra hiệu ứng mờ dần (giây)")]
    public float fadeDuration = 1.5f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Lấy component CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();

        // Nếu chưa gán camera thủ công, tự động tìm Main Camera
        if (vrCamera == null && Camera.main != null)
        {
            vrCamera = Camera.main.transform;
        }

        // Bắt đầu quá trình chờ và mờ dần
        StartCoroutine(FadeOutAndDisable());
    }

    void LateUpdate()
    {
        if (vrCamera != null)
        {
            // Giữ panel đứng im tại chỗ, chỉ xoay hướng về phía camera.
            // Dùng transform.position + vrCamera.forward để UI không bị lật ngược chữ.
            transform.LookAt(transform.position + vrCamera.forward);
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        // 1. Chờ hết khoảng thời gian delay
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        // 2. Bắt đầu giảm dần Alpha về 0
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            yield return null; // Đợi frame tiếp theo
        }

        // Đảm bảo alpha hoàn toàn bằng 0 ở cuối
        canvasGroup.alpha = 0f;

        // 3. Ẩn object đi để tiết kiệm tài nguyên (hoặc dùng Destroy(gameObject) nếu muốn xóa hẳn)
        gameObject.SetActive(false);
    }
}