using UnityEngine;

namespace KaijuGame.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float bossEncounterSpeedMultiplier = 1.5f;

        private CharacterController controller;
        private float verticalVelocity;
        private bool bossBoostActive;

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            var direction = (transform.right * input.x + transform.forward * input.y).normalized;
            var speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            if (bossBoostActive) speed *= bossEncounterSpeedMultiplier;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;

            var motion = direction * speed;
            motion.y = verticalVelocity;
            controller.Move(motion * Time.deltaTime);
        }

        public void SetBossBoost(bool active) => bossBoostActive = active;
    }
}
