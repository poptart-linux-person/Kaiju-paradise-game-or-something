#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KaijuGame.EditorTools
{
    public static class GorillaPlayerRigSetup
    {
        [MenuItem("Kaiju Game/Configure Selected Model As VR Player")]
        public static void ConfigureSelectedModel()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                Debug.LogError("Select the imported rigged player model root first.");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(root, "Configure VR Player");

            var rigidbody = root.GetComponent<Rigidbody>() ?? Undo.AddComponent<Rigidbody>(root);
            rigidbody.mass = 70f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0.05f;
            rigidbody.useGravity = true;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var head = GetOrCreate(root.transform, "HeadCollider", root.transform);
            var left = GetOrCreate(root.transform, "LeftHandFollower", root.transform);
            var right = GetOrCreate(root.transform, "RightHandFollower", root.transform);
            var leftTracking = GetOrCreate(root.transform, "LeftHandTracking", root.transform);
            var rightTracking = GetOrCreate(root.transform, "RightHandTracking", root.transform);

            var headCollider = head.GetComponent<SphereCollider>() ?? Undo.AddComponent<SphereCollider>(head.gameObject);
            headCollider.radius = 0.12f;
            headCollider.isTrigger = false;

            var body = GetOrCreate(root.transform, "BodyCollider", root.transform);
            var bodyCollider = body.GetComponent<CapsuleCollider>() ?? Undo.AddComponent<CapsuleCollider>(body.gameObject);
            bodyCollider.height = 0.9f;
            bodyCollider.radius = 0.25f;
            bodyCollider.center = Vector3.zero;
            bodyCollider.isTrigger = false;

            var tracking = root.GetComponent<KaijuGame.Player.XRTrackingTargets>() ?? Undo.AddComponent<KaijuGame.Player.XRTrackingTargets>(root);
            SetSerializedField(tracking, "headTarget", head);
            SetSerializedField(tracking, "leftHandTarget", leftTracking);
            SetSerializedField(tracking, "rightHandTarget", rightTracking);

            var binder = root.GetComponent<KaijuGame.Player.GorillaPlayerRigBinder>() ?? Undo.AddComponent<KaijuGame.Player.GorillaPlayerRigBinder>(root);
            SetSerializedField(binder, "headTarget", head);
            SetSerializedField(binder, "leftHandTarget", leftTracking);
            SetSerializedField(binder, "rightHandTarget", rightTracking);
            SetSerializedField(binder, "headCollider", headCollider);
            SetSerializedField(binder, "bodyCollider", bodyCollider);

            var gorillaType = Type.GetType("GorillaLocomotion.Player, Assembly-CSharp");
            if (gorillaType != null)
            {
                var existing = root.GetComponent(gorillaType);
                if (existing == null)
                    existing = Undo.AddComponent(root, gorillaType);

                SetSerializedField(binder, "gorillaPlayer", existing);
                ConfigureGorillaPlayer(existing, root.layer);
            }
            else
            {
                Debug.LogWarning("GorillaLocomotion.Player is not installed yet. Use Kaiju Game/Install Gorilla Locomotion, then run this setup again.");
            }

            Selection.activeGameObject = root;
            EditorUtility.SetDirty(root);
            Debug.Log($"Configured {root.name} as a Gorilla-style VR player rig.");
        }

        private static Transform GetOrCreate(Transform parent, string name, Transform root)
        {
            var existing = parent.Find(name);
            if (existing != null) return existing;
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create VR Rig Target");
            go.transform.SetParent(root, false);
            return go.transform;
        }

        private static void ConfigureGorillaPlayer(Component player, int layer)
        {
            SetField(player, "velocityHistorySize", 12);
            SetField(player, "maxArmLength", 1.5f);
            SetField(player, "unStickDistance", 1f);
            SetField(player, "velocityLimit", 8f);
            SetField(player, "maxJumpSpeed", 7f);
            SetField(player, "jumpMultiplier", 1.1f);
            SetField(player, "minimumRaycastDistance", 0.05f);
            SetField(player, "defaultSlideFactor", 0.03f);
            SetField(player, "defaultPrecision", 0.995f);
            SetField(player, "locomotionEnabledLayers", LayerMask.GetMask(LayerMask.LayerToName(layer)));
        }

        private static void SetField(object target, string name, object value)
        {
            var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null && value != null && field.FieldType.IsInstanceOfType(value))
                field.SetValue(target, value);
            else if (field != null && field.FieldType == typeof(int) && value is int i)
                field.SetValue(target, i);
            else if (field != null && field.FieldType == typeof(float) && value is float f)
                field.SetValue(target, f);
            else if (field != null && field.FieldType == typeof(LayerMask) && value is LayerMask mask)
                field.SetValue(target, mask);
        }

        private static void SetSerializedField(UnityEngine.Object target, string name, object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(name);
            if (property == null) return;
            if (value is UnityEngine.Object obj)
                property.objectReferenceValue = obj;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
