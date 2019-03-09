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
        [SerializeField] RectTransform _spinner;
        [SerializeField] float _spinSpeed = 1;

        Tween spinAnim;

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

        void OnEnable()
        {
            spinAnim = _spinner.DORotate(Vector3.back * 360, 1f / _spinSpeed, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);
        }

        void OnDisable()
        {
            spinAnim.Kill();
            spinAnim = null;
        }

    }

}