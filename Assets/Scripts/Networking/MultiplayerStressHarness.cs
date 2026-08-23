using System.Collections.Generic;
using UnityEngine;

namespace KaijuGame.Networking
{
    /// <summary>
    /// Local pre-Photon stress harness. It approximates an 8-player session by
    /// simulating peer bodies and high-volume physics objects. It is intentionally
    /// separate from Photon so it can run before an App ID is configured.
    /// </summary>
    public sealed class MultiplayerStressHarness : MonoBehaviour
    {
        [SerializeField] private int simulatedPeers = 8;
        [SerializeField] private int simulatedItems = 120;
        [SerializeField] private float radius = 18f;
        [SerializeField] private float launchStrength = 4f;
        [SerializeField] private bool autoRunOnPlay;

        private readonly List<GameObject> spawnedPeers = new();
        private readonly List<GameObject> spawnedItems = new();
        private float elapsed;

        private void Start()
        {
            if (autoRunOnPlay)
                RunStressTest();
        }

        [ContextMenu("Run 8 Peer Local Stress Test")]
        public void RunStressTest()
        {
            Clear();

            var peerCount = Mathf.Clamp(simulatedPeers, 2, 16);
            var itemCount = Mathf.Clamp(simulatedItems, 10, 500);

            for (var i = 0; i < peerCount; i++)
            {
                var angle = i / (float)peerCount * Mathf.PI * 2f;
                var position = new Vector3(Mathf.Cos(angle), 1f, Mathf.Sin(angle)) * (radius * 0.45f);
                var peer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                peer.name = $"StressPeer_{i + 1:00}";
                peer.transform.position = position;
                var body = peer.AddComponent<Rigidbody>();
                body.mass = 70f;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                body.AddForce(Random.onUnitSphere * launchStrength, ForceMode.VelocityChange);
                spawnedPeers.Add(peer);
            }

            for (var i = 0; i < itemCount; i++)
            {
                var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
                item.name = $"StressItem_{i + 1:000}";
                item.transform.localScale = Vector3.one * Random.Range(0.08f, 0.25f);
                item.transform.position = Random.insideUnitSphere * radius + Vector3.up * Random.Range(1f, 4f);
                var body = item.AddComponent<Rigidbody>();
                body.mass = Random.Range(0.25f, 5f);
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                body.AddForce(Random.onUnitSphere * Random.Range(1f, 7f), ForceMode.VelocityChange);
                spawnedItems.Add(item);
            }

            elapsed = 0f;
            Debug.Log($"[Stress] Spawned {peerCount} simulated peers and {itemCount} physics items.");
        }

        private void FixedUpdate()
        {
            if (spawnedPeers.Count == 0) return;
            elapsed += Time.fixedDeltaTime;
            if (elapsed < 0.25f) return;
            elapsed = 0f;

            foreach (var peer in spawnedPeers)
            {
                if (peer == null) continue;
                var body = peer.GetComponent<Rigidbody>();
                if (body == null) continue;
                body.AddForce(Random.insideUnitSphere * 0.8f, ForceMode.VelocityChange);
                if (body.velocity.magnitude > 10f)
                    body.velocity = body.velocity.normalized * 10f;
            }
        }

        [ContextMenu("Clear Stress Test")]
        public void Clear()
        {
            foreach (var peer in spawnedPeers)
                if (peer != null) DestroyImmediate(peer);
            foreach (var item in spawnedItems)
                if (item != null) DestroyImmediate(item);
            spawnedPeers.Clear();
            spawnedItems.Clear();
        }
    }
}
