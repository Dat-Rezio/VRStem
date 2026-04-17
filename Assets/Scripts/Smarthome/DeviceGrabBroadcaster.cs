using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DeviceGrabBroadcaster : MonoBehaviour
{
    [Header("--- Phân loại Thiết bị ---")]
    public string deviceSocketType = "Ceiling";

    public static Action<string> OnAnyDeviceGrabbed;
    public static Action OnAnyDeviceReleased;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) Debug.LogError($"[Broadcaster] {gameObject.name} không có XRGrabInteractable!");
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // THÊM CHỐT KIỂM TRA: Ai là người vừa cầm vật thể này lên?
        // args.interactorObject chính là "Kẻ" vừa thực hiện hành động Grab.
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor)
        {
            // Nếu kẻ đó là một cái Ổ Cắm -> Ra lệnh tắt Highlight để dọn dẹp và thoát hàm!
            OnAnyDeviceReleased?.Invoke();
            return; 
        }

        // NẾU KẺ ĐÓ KHÔNG PHẢI Ổ CẮM (Tức là tay người chơi): Bật đài phát thanh!
        Debug.Log($"[Broadcaster] TAY NGƯỜI đã CẦM thiết bị: {gameObject.name} | Gửi đi từ khóa: '{deviceSocketType}'");
        
        if (OnAnyDeviceGrabbed == null)
        {
            Debug.LogWarning("[Broadcaster] Cảnh báo: Kênh phát thanh đang trống, KHÔNG CÓ Ổ CẮM NÀO đang nghe!");
        }
        else
        {
            OnAnyDeviceGrabbed.Invoke(deviceSocketType);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // TRẠM GÁC 2: Báo cáo buông tay
        Debug.Log($"[Broadcaster] Đã BUÔNG thiết bị: {gameObject.name}");
        OnAnyDeviceReleased?.Invoke();
    }

    // Hàm dọn dẹp
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStaticEvents()
    {
        // Cắt đứt mọi liên lạc của các ổ cắm cũ từ lần Play trước
        OnAnyDeviceGrabbed = null;
        OnAnyDeviceReleased = null;
    }
}