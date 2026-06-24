using System;
using System.Collections.Generic;

[Serializable]
public class GroqRequest
{
    public string model;
    public List<GroqMessage> messages;
    public int max_tokens; 
    public float temperature;
}

[Serializable]
public class GroqMessage
{
    public string role;

    public string content;

    public GroqMessage(
        string role,
        string content
    )
    {
        this.role = role;
        this.content = content;
    }
}