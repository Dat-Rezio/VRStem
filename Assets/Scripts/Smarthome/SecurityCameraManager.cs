using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SecurityCameraManager : MonoBehaviour
{
    public static SecurityCameraManager Instance;

    [System.Serializable]
    public class CameraScreen
    {
        public TextMeshProUGUI camNameText;
        public GameObject noSignalUI;
        public RawImage screenDisplay; 
        [HideInInspector] public int currentIndex = 0;
    }

    [Header("--- Cấu hình Màn hình ---")]
    public List<CameraScreen> screens;

    [Header("--- Tối ưu hóa (Optimization) ---")]
    [Tooltip("Giới hạn FPS của Camera an ninh (CCTV thực tế chỉ chạy tầm 15 FPS).")]
    public float cameraFPS = 15f;

    private List<SmartDeviceController> cameraList = new List<SmartDeviceController>();
    private Dictionary<SmartDeviceController, RenderTexture> camFeeds = new Dictionary<SmartDeviceController, RenderTexture>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        StartCoroutine(RenderCamerasAtLowFPS());
    }

    public void RegisterCamera(SmartDeviceController cam)
    {
        if (!cameraList.Contains(cam))
        {
            cameraList.Add(cam);

            if (cam.securityCamera != null && !camFeeds.ContainsKey(cam))
            {
                RenderTexture newRT = new RenderTexture(256, 256, 1);
                cam.securityCamera.enabled = false; // Ngủ đông mặc định
                cam.securityCamera.targetTexture = newRT;
                camFeeds.Add(cam, newRT);
            }

            UpdateAllDisplays();
        }
    }

    public void NextCamera(int screenIndex)
    {
        if (cameraList.Count == 0 || screenIndex >= screens.Count) return;
        screens[screenIndex].currentIndex++;
        if (screens[screenIndex].currentIndex >= cameraList.Count) screens[screenIndex].currentIndex = 0; 
        UpdateAllDisplays(); 
    }

    public void PrevCamera(int screenIndex)
    {
        if (cameraList.Count == 0 || screenIndex >= screens.Count) return;
        screens[screenIndex].currentIndex--;
        if (screens[screenIndex].currentIndex < 0) screens[screenIndex].currentIndex = cameraList.Count - 1; 
        UpdateAllDisplays();
    }

    public void UpdateAllDisplays()
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (cameraList.Count == 0) continue;

            CameraScreen screen = screens[i];
            SmartDeviceController currentCam = cameraList[screen.currentIndex];

            if (screen.camNameText != null && screen.camNameText.text != currentCam.deviceName)
            {
                screen.camNameText.text = currentCam.deviceName;
            }

            if (currentCam.isOn)
            {
                if (screen.screenDisplay != null && camFeeds.ContainsKey(currentCam))
                    screen.screenDisplay.texture = camFeeds[currentCam];

                if (screen.noSignalUI != null && screen.noSignalUI.activeSelf) 
                    screen.noSignalUI.SetActive(false);
            }
            else
            {
                if (screen.screenDisplay != null && screen.screenDisplay.texture != null) 
                    screen.screenDisplay.texture = null;

                if (screen.noSignalUI != null && !screen.noSignalUI.activeSelf) 
                    screen.noSignalUI.SetActive(true);
            }
        }
    }

    // ĐÃ SỬA LỖI: Tương thích hoàn toàn 100% với Universal Render Pipeline (URP)
    private IEnumerator RenderCamerasAtLowFPS()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1f / cameraFPS);

        while (true)
        {
            // 1. Nghỉ ngơi theo FPS đã cấu hình (Ví dụ: 15 lần / giây)
            yield return waitTime;

            List<Camera> activeCams = new List<Camera>();

            // 2. Tìm xem Camera nào đang có người xem
            foreach (var cam in cameraList)
            {
                if (cam.securityCamera == null || !cam.isOn) continue;

                bool isBeingWatched = false;
                foreach (var screen in screens)
                {
                    if (cameraList[screen.currentIndex] == cam)
                    {
                        isBeingWatched = true;
                        break;
                    }
                }

                // Nếu có người xem -> Đánh thức Camera dậy cho URP tự nhận diện
                if (isBeingWatched)
                {
                    cam.securityCamera.enabled = true;
                    activeCams.Add(cam.securityCamera);
                }
            }

            // 3. NẾU có camera được bật, ta phải đợi URP vẽ xong khung hình này
            if (activeCams.Count > 0)
            {
                yield return new WaitForEndOfFrame();

                // 4. Vẽ xong rồi -> Lập tức "bấm nút Tắt" để chúng không tốn tài nguyên cho các khung hình sau
                foreach (var cam in activeCams)
                {
                    cam.enabled = false;
                }
            }
        }
    }

    void OnDestroy()
    {
        foreach (var rt in camFeeds.Values)
        {
            if (rt != null) rt.Release(); 
        }
        camFeeds.Clear();
    }
}