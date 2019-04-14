using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChatSystem : GameSystem<ChatSystem>
{
    public override bool isSystemReady => true;
    public event Action<ChatLine> onNewLine;

    public override void OnGameReady()
    {
        base.OnGameReady();

        GameConsole.AddCommand("chat", OnConsoleCommand_chat, "Send a chat message to all other players");
    }
    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();
        GameConsole.RemoveCommand("chat");
    }

    void OnConsoleCommand_chat(string[] args)
    {
        if(args.Length > 0)
        {
            string join = string.Join(" ", args);
            SubmitMessage(join);
        }
    }

    public abstract void SubmitMessage(string message);

    protected void AddNewLine(ChatLine chatLine)
    {
        onNewLine.Invoke(chatLine);
    }
}
