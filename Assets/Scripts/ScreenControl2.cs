using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenControl2 : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject contatosPanel;
    [SerializeField] private GameObject conversaPanel;

    [Header("Conversa")]
    [SerializeField] private TMP_Text contatoNomeText;
    [SerializeField] private Button voltarButton;

    [Header("Managers")]
    [SerializeField] private ChatManager chatManager;

    private CharacterData currentCharacter;

    private void Awake()
    {
        if (chatManager == null)
        {
            chatManager = FindObjectOfType<ChatManager>();
        }

        if (voltarButton != null)
        {
            voltarButton.onClick.RemoveListener(ShowContacts);
            voltarButton.onClick.AddListener(ShowContacts);
        }
    }

    private void Start()
    {
        ShowContacts();
    }

    public void ShowContacts()
    {
        currentCharacter = null;

        if (contatosPanel != null)
        {
            contatosPanel.SetActive(true);
        }

        if (conversaPanel != null)
        {
            conversaPanel.SetActive(false);
        }

        if (contatoNomeText != null)
        {
            contatoNomeText.text = string.Empty;
        }

        if (chatManager != null
            && chatManager.inputField != null)
        {
            TMP_Text placeholder =
                chatManager.inputField.placeholder as TMP_Text;

            if (placeholder != null)
            {
                placeholder.text = "Digite uma mensagem";
            }
        }
    }

    public void OpenConversation(CharacterData character)
    {
        if (character == null)
        {
            return;
        }

        currentCharacter = character;

        if (contatosPanel != null)
        {
            contatosPanel.SetActive(false);
        }

        if (conversaPanel != null)
        {
            conversaPanel.SetActive(true);
        }

        if (contatoNomeText != null)
        {
            contatoNomeText.text = character.characterName;
        }

        if (chatManager != null)
        {
            chatManager.SelectCharacter(character);
        }

        TMP_InputField inputField = chatManager != null
            ? chatManager.inputField
            : null;

        if (inputField != null)
        {
            inputField.text = string.Empty;
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    public void OpenCurrentConversation()
    {
        if (currentCharacter != null)
        {
            OpenConversation(currentCharacter);
        }
    }

    public CharacterData CurrentCharacter
    {
        get { return currentCharacter; }
    }
}