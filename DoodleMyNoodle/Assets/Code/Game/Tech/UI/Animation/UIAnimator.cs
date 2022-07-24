using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UIAnimator : MonoBehaviour
{
    private Animator _animator;

    private Animator Animator => _animator ??= GetComponent<Animator>();

    public void SetBool(string name, bool value) { Animator.SetBool(name, value); }
    public void SetInteger(string name, int value) { Animator.SetInteger(name, value); }
    public void SetTigger(string name) { Animator.SetTrigger(name); }
}
