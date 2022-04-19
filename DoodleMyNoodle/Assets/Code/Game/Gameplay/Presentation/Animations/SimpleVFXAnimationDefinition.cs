using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Simple VFX")]
public class SimpleVFXAnimationDefinition : AnimationDefinition
{
    [SerializeField] private GameObject _vfxPrefab;
    [SerializeField] private bool _attachToInstigator;
    [SerializeField] private Vector2 _instigatorOffset;

    public override void TriggerAnimation(TriggerInput input, ref TriggerOuput ouput)
    {
        var position = (Vector2)input.PresentationTarget.Root.transform.position + _instigatorOffset;
        Instantiate(_vfxPrefab, position, Quaternion.identity, _attachToInstigator ? input.PresentationTarget.Root.transform : null);
    }

    public override void StopAnimation(StopInput input) { }
}
