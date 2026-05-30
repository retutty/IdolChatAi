using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ContactListManager : MonoBehaviour
{
    [Header("UI")]
    public Transform contactsContent;

    public GameObject contactPrefab;

    [Header("Managers")]
    public ScreenControl2 screenControl2;

    private CharacterDatabase database;

    private void Awake()
    {
        if (screenControl2 == null)
        {
            screenControl2 = FindAnyObjectByType<ScreenControl2>();
        }
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return LoadStreamingAssetsText(
            "characters.json",
            json =>
            {
                database = JsonUtility.FromJson<CharacterDatabase>(json);
            }
        );

        if (database == null || database.characters == null)
        {
            Debug.LogError("Nenhum contato foi carregado porque a base de personagens está vazia.");
            yield break;
        }

        CreateContacts();
    }

    IEnumerator LoadStreamingAssetsText(
        string fileName,
        Action<string> onSuccess
    )
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            fileName
        );

        if (Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(
                        $"Falha ao carregar {fileName}: {request.error}"
                    );

                    yield break;
                }

                onSuccess?.Invoke(request.downloadHandler.text);
            }

            yield break;
        }

        if (!File.Exists(path))
        {
            Debug.LogError(
                $"Arquivo não encontrado em StreamingAssets: {path}"
            );

            yield break;
        }

        onSuccess?.Invoke(File.ReadAllText(path));
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
                screenControl2
            );
        }
    }
}