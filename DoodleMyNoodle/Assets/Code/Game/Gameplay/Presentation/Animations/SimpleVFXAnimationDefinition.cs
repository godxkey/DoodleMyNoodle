using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Simple VFX")]
public class SimpleVFXAnimationDefinition : AnimationDefinition
{
    [SerializeField] private GameObject _vfxPrefab;
    [SerializeField] private bool _attachToInstigator;
    [SerializeField] private Vector2 _instigatorOffset;

    public override void TriggerAnimation(TriggerInput input, ref TriggerOuput ouput)
    {
        if (_attachToInstigator)
        {
            Instantiate(_vfxPrefab, _instigatorOffset, Quaternion.identity, input.PresentationTarget.Root.transform);
        }
        else
        {
            Instantiate(_vfxPrefab, (Vector2)input.PresentationTarget.Root.transform.position + _instigatorOffset, Quaternion.identity);
        }
    }

    public override void StopAnimation(StopInput input) { }
}
