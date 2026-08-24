#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using KaijuGame.Animation;

namespace KaijuGame.EditorTools
{
    public static class ProceduralAnimationSetup
    {
        [MenuItem("Kaiju Game/Configure Selected Creature Procedural Animation")]
        public static void ConfigureSelectedCreature()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                Debug.LogError("Select the creature/player model root first.");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(root, "Configure Procedural Animation");
            var animation = root.GetComponent<ProceduralAnimation>() ?? Undo.AddComponent<ProceduralAnimation>(root);

            var candidates = FindLegTargets(root.transform);
            if (candidates.Count > 0)
            {
                var serialized = new SerializedObject(animation);
                var property = serialized.FindProperty("legTargets");
                property.arraySize = candidates.Count;
                for (var i = 0; i < candidates.Count; i++)
                    property.GetArrayElementAtIndex(i).objectReferenceValue = candidates[i];
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorUtility.SetDirty(root);
            Debug.Log($"Procedural animation configured on {root.name} with {candidates.Count} detected leg targets.");
        }

        private static List<Transform> FindLegTargets(Transform root)
        {
            var result = new List<Transform>();
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                var lower = child.name.ToLowerInvariant();
                if (lower.Contains("foot") || lower.Contains("paw") || lower.Contains("hoof") || lower.Contains("legtarget") || lower.Contains("handtarget"))
                    result.Add(child);
            }

            result.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            if (result.Count > 8)
                result.RemoveRange(8, result.Count - 8);
            return result;
        }
    }
}
#endif
