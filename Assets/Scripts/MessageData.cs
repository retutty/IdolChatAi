using System;

[Serializable]
public class MessageData
{
    public string role;
    public string content;

    public MessageData(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}