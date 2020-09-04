using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class GameConsoleGUIInputField : TMP_InputField
{
    /// <summary>
    /// Handle the specified event.
    /// </summary>
    private Event _processingEvent = new Event();

    public List<KeyCode> UnhandledKeycodes = new List<KeyCode>()
    {
        KeyCode.UpArrow,
        KeyCode.DownArrow,
    };

    public List<char> UnhandledCharacters = new List<char>()
    {
        '/'
    };

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (!isFocused)
            return;

        bool consumedEvent = false;
        while (Event.PopEvent(_processingEvent))
        {
            if (_processingEvent.rawType == EventType.KeyDown)
            {
                consumedEvent = true;

                if (!UnhandledKeycodes.Contains(_processingEvent.keyCode) &&
                    !UnhandledCharacters.Contains(_processingEvent.character))
                {
                    var shouldContinue = KeyPressed(_processingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        DeactivateInputField();
                        break;
                    }
                }
            }

            switch (_processingEvent.type)
            {
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    switch (_processingEvent.commandName)
                    {
                        case "SelectAll":
                            SelectAll();
                            consumedEvent = true;
                            break;
                    }
                    break;
            }
        }

        if (consumedEvent)
            UpdateLabel();

        eventData.Use();
    }
}
