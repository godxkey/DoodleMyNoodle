using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChatSystem : GameSystem<ChatSystem>
{
    public override bool SystemReady => true;
    public event Action<ChatLine> OnNewLine;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        GameConsole.SetCommandOrVarEnabled("Chat", true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GameConsole.SetCommandOrVarEnabled("Chat", false);
    }

    [ConsoleCommand(Description = "Send a chat message to all other players")]
    private static void Chat(string message)
    {
        Instance?.SubmitMessage(message);
    }

    public abstract void SubmitMessage(string message);

    protected void AddNewLine(ChatLine chatLine)
    {
        OnNewLine?.Invoke(chatLine);
    }
}
