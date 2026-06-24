using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WriteBarTel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform messagePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private ChatManager chatManager;

    [Header("Keyboard Settings")]
    [SerializeField] private float bottomPadding = 16f;
    [SerializeField] private float smoothTime = 0.08f;
    [SerializeField] private bool debugKeyboard = false;

    private Canvas parentCanvas;
    private Vector2 baseAnchoredPosition;
    private Vector2 velocity;
    private bool initialized;
    private bool keyboardWasVisible;

    private void Awake()
    {
        if (chatManager == null)
        {
            chatManager = FindObjectOfType<ChatManager>();
        }

        if (inputField == null && chatManager != null)
        {
            inputField = chatManager.inputField;
        }

        if (messagePanel == null && inputField != null)
        {
            messagePanel = inputField.transform.parent as RectTransform;
        }

        if (messagePanel != null)
        {
            parentCanvas = messagePanel.GetComponentInParent<Canvas>();
            baseAnchoredPosition = messagePanel.anchoredPosition;
            initialized = true;
        }
    }

    private void Start()
    {
        if (sendButton != null)
        {
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(OnSendPressed);
        }

        if (inputField != null)
        {
            inputField.onSelect.AddListener(OnInputSelected);
            inputField.onDeselect.AddListener(OnInputDeselected);
        }

        if (!initialized)
        {
            Debug.LogWarning("WriteBarTel: messagePanel ou inputField não configurados. Arraste as referências no Inspector.", this);
        }

        if (sendButton == null)
        {
            Debug.LogWarning("WriteBarTel: sendButton não atribuído. O botão de enviar não funcionará.", this);
        }
    }

    private void Update()
    {
        if (!initialized)
            return;

        bool keyboardVisible = TouchScreenKeyboard.visible || (inputField != null && inputField.isFocused);
        float targetY = baseAnchoredPosition.y;

        if (keyboardVisible)
        {
            float keyboardHeight = GetKeyboardHeight();
            float canvasScale = parentCanvas != null ? parentCanvas.scaleFactor : 1f;
            float offset = (keyboardHeight / canvasScale) + bottomPadding;
            targetY = baseAnchoredPosition.y + offset;
            keyboardWasVisible = true;
        }
        else if (keyboardWasVisible)
        {
            targetY = baseAnchoredPosition.y;
            keyboardWasVisible = false;
        }

        Vector2 targetPosition = new Vector2(baseAnchoredPosition.x, targetY);
        messagePanel.anchoredPosition = Vector2.SmoothDamp(messagePanel.anchoredPosition, targetPosition, ref velocity, smoothTime);

        if (debugKeyboard)
        {
            Debug.Log($"WriteBarTel: visible={keyboardVisible}, keyboardHeight={GetKeyboardHeight()}, targetY={targetY}");
        }
    }

    private float GetKeyboardHeight()
    {
        if (TouchScreenKeyboard.visible)
        {
            float height = TouchScreenKeyboard.area.height;
            if (height > 0f)
            {
                return height;
            }
        }

        Rect safeArea = Screen.safeArea;
        float safeBottom = safeArea.yMin;
        float fallbackHeight = Screen.height * 0.36f;

        if (safeBottom > 0f)
        {
            float keyboardGuess = Screen.height - safeArea.height - safeBottom;
            if (keyboardGuess > 0f)
            {
                return keyboardGuess;
            }
        }

        return fallbackHeight;
    }

    private void OnInputSelected(string text)
    {
        if (debugKeyboard)
        {
            Debug.Log("WriteBarTel: input selected");
        }
    }

    private void OnInputDeselected(string text)
    {
        if (messagePanel != null)
        {
            messagePanel.anchoredPosition = baseAnchoredPosition;
        }
    }

    private void OnSendPressed()
    {
        if (chatManager == null || inputField == null)
            return;

        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message))
            return;

        chatManager.SubmitMessage(message);
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }
}
