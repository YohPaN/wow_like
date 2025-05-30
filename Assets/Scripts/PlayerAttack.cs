using UnityEngine;

namespace Assets.Scripts
{

    public class PlayerAttack : MonoBehaviour
    {
        public float attackRange = 20f;
        public int attackDamage;
        private readonly int[] levelAttackDamage = { 25, 50, 100, 175, 250, 300 };
        public float attackCooldown = 1f;
        public LayerMask enemyLayer;
        public Animator animator;
        public PlayerController player;
        private bool isAttacking = false;
        private float attackTimer = 0f;
        private Enemy currentTarget;

        void Start()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<PlayerController>();
        }

        void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking) // Left clic
            {
                Attack();
            }

            if (isAttacking)
            {
                // remove the delta time from last frame to the timer between each attack
                attackTimer -= Time.deltaTime;

                // if the timer is under 0, it's time to attack
                if (attackTimer <= 0f)
                {
                    // if there is a target and the distance between player and the enemy is less than 
                    // the attack range, start attacking
                    if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
                    {
                        // call TakeDamage method on the enemy instance
                        currentTarget.TakeDamage(attackDamage);
                        // reinitialize the timer to the attack cooldown
                        attackTimer = attackCooldown;
                    }
                    else
                    {
                        // Stop attacking if enemy is gone or too far and reset variables
                        isAttacking = false;
                        currentTarget = null;
                        // set the bool responsible of the transition between Attack animation to default animation
                        animator.SetBool("IsAttacking", false);
                    }
                }
            }
        }

        void Attack()
        {
            // create a ray from the player (transform.position) 
            // in front of it (transform.TransformDirection(Vector3.forward)) 
            // to the enemy (enemyLayer) 
            // and get the result if the ray collide with an ennemy (hit)
            // It will return true if the ray has encounter an ennemy
            if (Physics.Raycast(
                transform.position,
                transform.TransformDirection(Vector3.forward),
                out RaycastHit hit,
                Mathf.Infinity,
                enemyLayer)
            )
            {
                // get the enemy instance with the hit variable that return the collider with an ennemy
                currentTarget = hit.collider.GetComponentInParent<Enemy>();

                if (currentTarget != null)
                {
                    // attach a listener on the OnEnemyDied that is a ennemy method,
                    // if this method is trigger, it will trigger OnTargetDied 
                    currentTarget.OnEnemyDied += OnTargetDied;

                    isAttacking = true;
                    attackTimer = 0f; // attack immediately
                    // trigger the Attack to allow transition from Idle to Attack animation
                    animator.SetTrigger("Attack");
                    // set the bool that allow the transition between an Attack animation and the Idle animation
                    animator.SetBool("IsAttacking", true);
                }
            }
        }

        void OnTargetDied()
        {
            if (currentTarget != null)
            {
                // when the enemy die, remoive the listener, gain xp, recalculate attack damage 
                // based on the new level
                currentTarget.OnEnemyDied -= OnTargetDied; // ✅ unsubscribe
                player.GainExperience(currentTarget.xpGain);
                attackDamage = levelAttackDamage[player.currentLevel];
                currentTarget = null;
            }

            isAttacking = false;
            animator.SetBool("IsAttacking", false);
        }
    }
}

