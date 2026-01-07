using UnityEngine;

public class WeaponHandIK : MonoBehaviour
{
    [Header("IK Targets")]
    public Transform leftHandTarget;

    [Header("IK Weights")]
    [Range(0f, 1f)] public float leftHandWeight = 0f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator == null) return;

        // LEFT HAND LOGIC
        // We only apply IK if there is a target and the weight is greater than 0
        if (leftHandTarget != null && leftHandWeight > 0f)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
        else
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
        }

        // RIGHT HAND NOTE: 
        // We do not use IK for the right hand because the weapon is parented 
        // directly to the RightHand bone in PlayerWeaponHandler.
    }

    public void SetLeftHandTarget(Transform target)
    {
        leftHandTarget = target;
        // Instantly set weight to 1 if we have a target, 0 if not.
        leftHandWeight = (target != null) ? 1f : 0f;
    }
}