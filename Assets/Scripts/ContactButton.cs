using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactButton : MonoBehaviour
{
    public TMP_Text contactNameText;

    private string characterId;

    private ChatManager chatManager;

    public void Setup(
        CharacterData character,
        ChatManager manager
    )
    {
        contactNameText.text =
            character.characterName;

        characterId = character.id;

        chatManager = manager;

        GetComponent<Button>()
            .onClick
            .AddListener(OnClick);
    }

    void OnClick()
    {
        chatManager.SelectCharacter(characterId);
    }
}