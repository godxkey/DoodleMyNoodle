using UnityEngine;
using UnityEngine.UI;

namespace CCC.Hidden
{
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {
        public AudioClip clip;
        public AudioAsset audioAsset;

        void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick);
        }

        void OnClick()
        {
            if(clip == null)
            {
                DefaultAudioSourceService.Instance.PlayStaticSFX(audioAsset);
            }
            else
            {
                DefaultAudioSourceService.Instance.PlayStaticSFX(clip);
            }
        }
    }
}
