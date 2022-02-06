using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


namespace Internals.MenuSessionChoice
{
    public class SessionButton : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      Public                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        public event System.Action<SessionButton> onClick;
        public bool selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                UpdateSelectedView();
            }
        }
        public INetworkInterfaceSession session
        {
            get { return _session; }
            set
            {
                _session = value;
                UpdateSessionView();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Private                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] TextMeshProUGUI _playerCount;
        [SerializeField] Button _button;
        [SerializeField] Image _background;
        [SerializeField] Color _selectedBackgroundTint;
        [SerializeField] Color _normalBackgroundTint;

        INetworkInterfaceSession _session;
        bool _selected;

        void Awake()
        {
            _button.onClick.AddListener(OnClick);
            UpdateSessionView();
        }

        void OnClick()
        {
            onClick?.Invoke(this);
        }

        void UpdateSessionView()
        {
            if (_session != null)
            {
                _title.text = _session.HostName;
                _playerCount.text = _session.ConnectionsCurrent.ToString() + '/' + _session.ConnectionsMax;
            }
            else
            {
                _title.text = "ERROR NULL SESSION";
                _playerCount.text = "0/0";
            }
        }

        void UpdateSelectedView()
        {
            if (_selected)
            {
                _background.color = _selectedBackgroundTint;
            }
            else
            {
                _background.color = _normalBackgroundTint;
            }
        }
    }
}
