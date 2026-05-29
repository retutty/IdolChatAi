using System.IO;
using UnityEngine;

public class ContactListManager : MonoBehaviour
{
    [Header("UI")]
    public Transform contactsContent;

    public GameObject contactPrefab;

    [Header("Managers")]
    public ChatManager chatManager;

    private CharacterDatabase database;

    void Start()
    {
        LoadCharacters();

        CreateContacts();
    }

    void LoadCharacters()
    {
        string path =
            Path.Combine(
                Application.streamingAssetsPath,
                "characters.json"
            );

        string json =
            File.ReadAllText(path);

        database =
            JsonUtility.FromJson<CharacterDatabase>(json);
    }

    void CreateContacts()
    {
        foreach (CharacterData character
                 in database.characters)
        {
            GameObject contact =
                Instantiate(
                    contactPrefab,
                    contactsContent
                );

            ContactButton button =
                contact.GetComponent<ContactButton>();

            button.Setup(
                character,
                chatManager
            );
        }
    }
}