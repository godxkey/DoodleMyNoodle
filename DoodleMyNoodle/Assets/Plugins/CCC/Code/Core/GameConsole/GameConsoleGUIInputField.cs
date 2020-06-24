using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class GameConsoleGUIInputField : InputField
{
    /// <summary>
    /// Handle the specified event.
    /// </summary>
    private Event _processingEvent = new Event();

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

                if(_processingEvent.keyCode != KeyCode.UpArrow && _processingEvent.keyCode != KeyCode.DownArrow) // DO NOT HANDLE UP/DOWN ARROWS
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
