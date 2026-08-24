using UnityEngine;

namespace KaijuGame.Player
{
    public sealed class SpectatorTargetCycler : MonoBehaviour
    {
        [SerializeField] private Camera spectatorCamera;
        [SerializeField] private float lookSpeed = 7f;

        private int targetIndex;
        private Transform currentTarget;

        public bool IsSpectating { get; private set; }

        public void Begin()
        {
            IsSpectating = true;
            targetIndex = -1;
            SelectNext();
        }

        public void End()
        {
            IsSpectating = false;
            currentTarget = null;
        }

        public void SelectNext()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) return;
            targetIndex = (targetIndex + 1) % players.Length;
            currentTarget = players[targetIndex] != null ? players[targetIndex].transform : null;
        }

        private void LateUpdate()
        {
            if (!IsSpectating || spectatorCamera == null || currentTarget == null) return;
            var desired = currentTarget.position + Vector3.up * 1.6f;
            spectatorCamera.transform.position = Vector3.Lerp(spectatorCamera.transform.position, desired, Time.deltaTime * lookSpeed);
            var direction = currentTarget.position - spectatorCamera.transform.position;
            if (direction.sqrMagnitude > 0.001f)
                spectatorCamera.transform.rotation = Quaternion.Slerp(spectatorCamera.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * lookSpeed);
        }
    }
}
