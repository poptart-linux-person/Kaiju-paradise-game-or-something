using System;
using System.Reflection;
using UnityEngine;

namespace KaijuGame.Player
{
    public sealed class GorillaPlayerRigBinder : MonoBehaviour
    {
        [Header("Optional explicit references")]
        [SerializeField] private Component gorillaPlayer;
        [SerializeField] private Animator avatarAnimator;
        [SerializeField] private Transform headTarget;
        [SerializeField] private Transform leftHandTarget;
        [SerializeField] private Transform rightHandTarget;
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private CapsuleCollider bodyCollider;

        [Header("Avatar bone overrides")]
        [SerializeField] private Transform avatarHead;
        [SerializeField] private Transform avatarLeftHand;
        [SerializeField] private Transform avatarRightHand;

        private Type gorillaPlayerType;

        private void Awake()
        {
            ResolveReferences();
            ApplyGorillaReferences();
        }

        private void ResolveReferences()
        {
            if (gorillaPlayer == null)
            {
                gorillaPlayerType = Type.GetType("GorillaLocomotion.Player, Assembly-CSharp");
                if (gorillaPlayerType != null)
                    gorillaPlayer = GetComponentInChildren(gorillaPlayerType);
            }

            if (avatarAnimator == null)
                avatarAnimator = GetComponentInChildren<Animator>();

            if (avatarAnimator != null)
            {
                avatarHead ??= avatarAnimator.GetBoneTransform(HumanBodyBones.Head);
                avatarLeftHand ??= avatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
                avatarRightHand ??= avatarAnimator.GetBoneTransform(HumanBodyBones.RightHand);
            }

            if (headCollider == null) headCollider = GetComponentInChildren<SphereCollider>();
            if (bodyCollider == null) bodyCollider = GetComponentInChildren<CapsuleCollider>();
        }

        private void ApplyGorillaReferences()
        {
            if (gorillaPlayer == null)
            {
                Debug.LogWarning("GorillaLocomotion.Player was not found. Import Gorilla Locomotion and assign the player component.", this);
                return;
            }

            SetField("headCollider", headCollider);
            SetField("bodyCollider", bodyCollider);
            SetField("leftHandFollower", leftHandTarget);
            SetField("rightHandFollower", rightHandTarget);
            SetField("leftHandTransform", leftHandTarget);
            SetField("rightHandTransform", rightHandTarget);
        }

        private void SetField(string fieldName, object value)
        {
            if (gorillaPlayer == null) return;
            var field = gorillaPlayer.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (field == null) return;
            if (value == null || field.FieldType.IsInstanceOfType(value))
                field.SetValue(gorillaPlayer, value);
        }

        private Component GetComponentInChildren(Type type)
        {
            foreach (var component in GetComponentsInChildren<Component>(true))
            {
                if (component != null && type.IsInstanceOfType(component))
                    return component;
            }
            return null;
        }
    }
}
