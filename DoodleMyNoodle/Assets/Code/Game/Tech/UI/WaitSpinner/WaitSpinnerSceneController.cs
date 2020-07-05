using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Internals.WaitSpinnerService
{
    public class WaitSpinnerSceneController : MonoBehaviour
    {
        public bool BlockInput
        {
            get => _inputBlocker.enabled;
            set => _inputBlocker.enabled = value;
        }

        [SerializeField] Image _inputBlocker;

        void Awake()
        {
            Deactivate();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}