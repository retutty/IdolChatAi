using System;
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
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return LoadStreamingAssetsText(
            "config.json",
            json =>
            {
                ConfigData config = JsonUtility.FromJson<ConfigData>(json);
                apiKey = config.apiKey;
            }
        );

        yield return LoadStreamingAssetsText(
            "characters.json",
            json =>
            {
                database = JsonUtility.FromJson<CharacterDatabase>(json);
            }
        );
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

    void EnsureConversation(string characterId)
    {
        if (!conversations.ContainsKey(characterId))
        {
            conversations[characterId] =
                new List<MessageData>();
        }
    }

    public void SelectCharacter(CharacterData character)
    {
        if (character == null)
            return;

        currentCharacter = character;

        EnsureConversation(character.id);

        LoadConversation(character.id);
    }

    public void SelectCharacter(string characterId)
    {
        if (database == null || database.characters == null)
        {
            StartCoroutine(LoadCharactersAndSelect(characterId));
            return;
        }

        if (database == null || database.characters == null)
            return;

        currentCharacter =
            database.characters.Find(
                c => c.id == characterId
            );

        if (currentCharacter == null)
            return;

        EnsureConversation(characterId);

        LoadConversation(characterId);
    }

    IEnumerator LoadCharactersAndSelect(string characterId)
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
            yield break;
        }

        SelectCharacter(characterId);
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

        // prompt do personagem com instrução de brevidade
        string systemPromptWithLengthLimit =
            currentCharacter.systemPrompt +
            "\n\n[IMPORTANTE] Suas respostas devem ser CURTAS e CONCISAS, com no máximo 6-8 linhas. " +
            "Seja direto, responda de forma natural como em uma conversa por mensagens. " +
            "Evite parágrafos longos e explicações detalhadas.";

        messages.Add(
            new GroqMessage(
                "system",
                systemPromptWithLengthLimit
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