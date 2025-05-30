using System.Collections;
using UnityEngine;


namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] PlayerHealthUI healthUI;

        public int currentHealth = 100;
        public int maxHealth;
        public float movementSpeed = 5f;
        public float jumpHigh = 50f;
        public float gravity = -9.81f;
        public float currentXp = 0;
        public int currentLevel = 0;
        public int maxLevel;
        private readonly float[] levelCap = {
            10, 25, 75, 150, 225
        };
        private readonly int[] levelMaxHealth = { 100, 200, 275, 350, 500, 750 };

        public Transform cameraTransform;
        private CharacterController controller;
        public Animator animator;
        public event System.Action OnPlayerDied;

        private Vector3 velocity;
        private bool isMoving = false;

        void Start()
        {
            // get the controller of the player
            controller = GetComponent<CharacterController>();
            // get the animator of the player to be able to manage animation
            animator = GetComponent<Animator>();

            // initilize the max level
            maxLevel = levelCap.Length;

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        void Update()
        {
            // update the UI of the health
            healthUI.UpdateHealth(currentHealth, maxHealth);

            if (Input.GetMouseButton(1)) // right clic handle
            {
                // get mouse positions
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // calculate rotation based on mouse position
                Vector3 rotateValueX = Vector3.down * mouseX;
                Vector3 rotateValueY = Vector3.left * mouseY;

                // rotate the camera
                cameraTransform.eulerAngles -= rotateValueY;
                // mouve the camera up and down for better view
                cameraTransform.position += new Vector3(0, mouseY * 0.05f, 0);

                // rotate the player
                transform.Rotate(rotateValueX);
            }

            // get zqsd input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            //calculate the vector based on player input
            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            // if the player is moving
            if (inputDirection.magnitude >= 0.1f)
            {
                // This line makes the avatar move in the direction it is facing
                Vector3 movement = transform.TransformDirection(inputDirection);
                //move the avatar
                controller.Move(movementSpeed * Time.deltaTime * movement);

                // if player was not moving but an input from zqsd was received
                if (!isMoving)
                {
                    // set the IsMoving bool to true that is responsible to 
                    // allow transition from Idle animation to Move animation
                    animator.SetBool("IsMoving", true);
                    isMoving = true;
                }
            }
            else
            {
                // if player was moving but no input from zqsd was received 
                // disable the Move animation
                animator.SetBool("IsMoving", false);
                isMoving = false;
            }

            // get space key pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // set the velocity of the user at jumpHigh. Basically, it jump
                velocity.y = jumpHigh;
                animator.SetTrigger("Jump");

                // move the avatar in high
                controller.Move(velocity * Time.deltaTime);
            }

            // if the user is not on the ground
            if (!controller.isGrounded)
            {
                // apply the gravity to move the player down
                velocity.y += gravity * Time.deltaTime;
            }
            else
            {
                // else keep the velocity to 0
                velocity.y = 0;
            }

            // move the avatar
            controller.Move(velocity * Time.deltaTime);
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                animator.Play("GetHit01_SwordAndShield");
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

        public void GainExperience(float xp)
        {
            // if already at the max level, can't earn new xp points
            if (currentLevel >= maxLevel) return;

            currentXp += xp;

            // while current xp is under level cap of the current level, loop. 
            // We updating current level in the loop
            while (currentXp > levelCap[currentLevel])
            {
                float currentLevelCap = levelCap[currentLevel];

                // if enough xp to get a new level
                if (currentXp >= currentLevelCap)
                {
                    currentLevel++;
                    //update max health
                    maxHealth = levelMaxHealth[currentLevel];
                }

                // if not at the max level, decrease current xp by the level cap and loop
                if (currentLevel < maxLevel)
                {
                    currentXp -= currentLevelCap;
                }
                else
                {
                    // if the max level is reach, set current xp to 100%, update UI and early return
                    currentXp = currentLevelCap;
                    healthUI.UpdateXp(currentXp, levelCap[^1], currentLevel);
                    return;
                }

            }
            // update xp UI
            healthUI.UpdateXp(currentXp, levelCap[currentLevel], currentLevel);
        }
    }
}
