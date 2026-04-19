using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuToggle : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject menuCanvas; // Kéo Canvas cần bật/tắt vào đây

    [Header("Input Action")]
    [SerializeField] private InputActionReference toggleAction; // Gán nút bấm (ví dụ: XRI LeftHand/Menu)

    private void OnEnable()
    {
        // Kiểm tra nếu action tồn tại thì mới đăng ký
        if (toggleAction != null)
        {
            toggleAction.action.Enable();
            toggleAction.action.performed += OnToggleMenu;
        }
    }

    private void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnToggleMenu;
        }
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        Toggle();
    }

    // Hàm này để gọi từ nút bấm vật lý (Controller)
    public void Toggle()
    {
        if (menuCanvas != null)
        {
            bool currentState = menuCanvas.activeSelf;
            menuCanvas.SetActive(!currentState);
            
            Debug.Log("Menu đang: " + (!currentState ? "Hiện" : "Ẩn"));
        }
    }

    // Hàm này có thể gọi từ các nút UI "Close" hoặc "Back"
    public void SetMenuState(bool state)
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(state);
        }
    }
}