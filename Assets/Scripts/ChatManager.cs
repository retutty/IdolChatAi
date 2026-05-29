using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [Header("UI")]
    public Transform messagesContent;

    public GameObject userMessagePrefab;
    public GameObject aiMessagePrefab;

    public TMP_InputField inputField;

    private CharacterDatabase database;

    private CharacterData currentCharacter;

    private Dictionary<string, List<MessageData>>
        conversations =
        new Dictionary<string, List<MessageData>>();

    private string apiKey;

    void Start()
    {
        LoadConfig();
        LoadCharacters();
    }

    void LoadConfig()
    {
        string path =
            Path.Combine(
                Application.streamingAssetsPath,
                "config.json"
            );

        string json =
            File.ReadAllText(path);

        ConfigData config =
            JsonUtility.FromJson<ConfigData>(json);

        apiKey = config.apiKey;
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

    public void SelectCharacter(string characterId)
    {
        currentCharacter =
            database.characters.Find(
                c => c.id == characterId
            );

        if (!conversations.ContainsKey(characterId))
        {
            conversations[characterId] =
                new List<MessageData>();
        }

        LoadConversation(characterId);
    }

    void LoadConversation(string characterId)
    {
        ClearMessages();

        foreach (MessageData msg
                 in conversations[characterId])
        {
            bool isUser =
                msg.role == "user";

            AddMessage(
                msg.content,
                isUser
            );
        }
    }

    public void SubmitMessage(string value)
    {
        if (currentCharacter == null)
            return;

        if (string.IsNullOrWhiteSpace(value))
            return;

        MessageData userMessage =
            new MessageData(
                "user",
                value
            );

        conversations[currentCharacter.id]
            .Add(userMessage);

        AddMessage(value, true);

        inputField.text = "";

        inputField.ActivateInputField();

        StartCoroutine(
            SendToGroq()
        );
    }

    IEnumerator SendToGroq()
    {
        List<GroqMessage> messages =
            new List<GroqMessage>();

        // prompt do personagem
        messages.Add(
            new GroqMessage(
                "system",
                currentCharacter.systemPrompt
            )
        );

        // histórico
        foreach (MessageData msg
                 in conversations[currentCharacter.id])
        {
            messages.Add(
                new GroqMessage(
                    msg.role,
                    msg.content
                )
            );
        }

        GroqRequest requestData =
            new GroqRequest();

        requestData.model =
            "llama-3.3-70b-versatile";

        requestData.messages =
            messages;

        string json =
            JsonUtility.ToJson(requestData);

        byte[] bodyRaw =
            Encoding.UTF8.GetBytes(json);

        UnityWebRequest request =
            new UnityWebRequest(
                "https://api.groq.com/openai/v1/chat/completions",
                "POST"
            );

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + apiKey
        );

        yield return request.SendWebRequest();

        if (request.result
            != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                request.error
            );

            yield break;
        }

        string responseJson =
            request.downloadHandler.text;

        GroqResponse response =
            JsonUtility.FromJson<GroqResponse>(
                responseJson
            );

        string aiText =
            response.choices[0]
            .message.content;

        MessageData aiMessage =
            new MessageData(
                "assistant",
                aiText
            );

        conversations[currentCharacter.id]
            .Add(aiMessage);

        AddMessage(aiText, false);
    }

    void AddMessage(
        string text,
        bool isUser
    )
    {
        GameObject prefab =
            isUser
            ? userMessagePrefab
            : aiMessagePrefab;

        GameObject obj =
            Instantiate(
                prefab,
                messagesContent
            );

        TMP_Text tmp =
            obj.GetComponentInChildren<TMP_Text>();

        tmp.text = text;

        LayoutRebuilder
            .ForceRebuildLayoutImmediate(
                obj.GetComponent<RectTransform>()
            );

        Canvas.ForceUpdateCanvases();
    }

    void ClearMessages()
    {
        foreach (Transform child
                 in messagesContent)
        {
            Destroy(child.gameObject);
        }
    }
}