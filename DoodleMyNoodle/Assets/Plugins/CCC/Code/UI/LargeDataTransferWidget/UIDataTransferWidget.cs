using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDataTransferWidget : MonoBehaviour
{
    public UIDataTransferWidgetElement ElementPrefab;
    public Transform ElementsSpawnContainer;
    public AudioPlayable NewTransferSFX; 

    SessionInterface _session;
    List<UIDataTransferWidgetElement> _elements = new List<UIDataTransferWidgetElement>();

    void Update()
    {
        if (!IsHooked)
        {
            TryHook();
        }


        if (IsHooked)
        {
            var incoming = _session.IncomingDataTransfers;
            var outgoing = _session.OutgoingDataTransfer;

            int elementIterator = 0;
            for (int i = 0; i < incoming.Count; i++)
            {
                var transfer = incoming[i];
                var element = GetOrAddActiveElement(elementIterator);

                element.Description.Set(transfer.Description);
                element.TotalDataSize.Set(transfer.DataSize);
                element.CurrentDataSize.Set(Mathf.FloorToInt(transfer.Progress * transfer.DataSize));
                element.IsIncoming.Set(true);
                element.UpdateDisplay();

                elementIterator++;
            }
            for (int i = 0; i < outgoing.Count; i++)
            {
                var transfer = outgoing[i];
                var element = GetOrAddActiveElement(elementIterator);

                element.Description.Set(transfer.Description);
                element.TotalDataSize.Set(transfer.DataSize);
                element.CurrentDataSize.Set(Mathf.FloorToInt(transfer.Progress * transfer.DataSize));
                element.IsIncoming.Set(false);
                element.UpdateDisplay();

                elementIterator++;
            }

            DeactivateElementsFrom(elementIterator);
        }
    }

    private void OnDisable()
    {
        Unhook();
    }

    UIDataTransferWidgetElement GetOrAddActiveElement(int i)
    {
        while (i >= _elements.Count)
        {
            _elements.Add(Instantiate(ElementPrefab, ElementsSpawnContainer).GetComponent<UIDataTransferWidgetElement>());
        }

        var element = _elements[i];

        // active element if necessary
        if (!element.gameObject.activeSelf)
            element.gameObject.SetActive(true);

        return element;
    }

    void DeactivateElementsFrom(int i)
    {
        for (; i < _elements.Count; i++)
        {
            if (_elements[i].gameObject.activeSelf)
                _elements[i].gameObject.SetActive(false);
        }
    }

    bool IsHooked => _session != null;

    bool TryHook()
    {
        var online = OnlineService.OnlineInterface;
        if (online != null)
        {
            var session = online.SessionInterface;
            if (session != null)
            {
                session.OnBeginReceiveLargeDataTransfer += OnBeginReceiveLargeDataTransfer;
                session.OnTerminate += Unhook;
                _session = session;
                return true;
            }
        }
        return false;
    }

    void Unhook()
    {
        if (_session != null)
        {
            _session.OnTerminate -= Unhook;
            _session.OnBeginReceiveLargeDataTransfer -= OnBeginReceiveLargeDataTransfer;
            _session = null;
        }
    }

    private void OnBeginReceiveLargeDataTransfer(ReceiveDataTransferOperation arg1, INetworkInterfaceConnection arg2)
    {
        DefaultAudioSourceService.Instance.PlayStaticSFX(NewTransferSFX);
    }
}
