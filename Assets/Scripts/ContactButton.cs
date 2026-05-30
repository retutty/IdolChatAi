using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactButton : MonoBehaviour
{
    public TMP_Text contactNameText;

    private CharacterData character;

    private ScreenControl2 screenControl2;

    public void Setup(
        CharacterData character,
        ScreenControl2 screenControl2
    )
    {
        this.character = character;
        this.screenControl2 = screenControl2;

        contactNameText.text =
            character.characterName;

        GetComponent<Button>()
            .onClick
            .AddListener(OnClick);
    }

    void OnClick()
    {
        if (screenControl2 != null)
        {
            screenControl2.OpenConversation(character);
        }
    }
}