using UnityEngine;

namespace Assets.Scripts
{

    public class EnemyAttack : MonoBehaviour
    {
        public float attackRange = 20f; 
        public int attackDamage = 5;
        public float attackCooldown = 1f;
        public LayerMask playerLayer;
        public Animator animator;
        //USE THIS TO CHECK IF THE ENEMY IS ATTACKING


        private bool isAttacking = false;
        private float attackTimer = 0f;
        private PlayerController currentTarget;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void TryAttack(PlayerController target)
        {
            currentTarget = target;

            if (!isAttacking)
            {
                Attack();
            }

            if (isAttacking)
            {
                attackTimer -= Time.deltaTime;

                if (attackTimer <= 0f)
                {
                    if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
                    {
                        currentTarget.TakeDamage(attackDamage);
                        attackTimer = attackCooldown;
                    }
                    else
                    {
                        isAttacking = false;
                        currentTarget = null;
                        animator.SetBool("IsAttacking", false);
                    }
                }
            }
        }

        void Attack()
        {
            // WE REMOVE THE CONDITION AND NOW IT ATTACK BUT MY PLAYER DONT TAKE DAMAGE
            if (currentTarget != null)
            {
                currentTarget.OnPlayerDied += OnTargetDied;

                isAttacking = true;
                attackTimer = 0f; // attack immediately
                animator.SetTrigger("Attack");
                animator.SetBool("IsAttacking", true);
            }
        }

        private void OnTargetDied()
        {
            if (currentTarget != null)
            {
                currentTarget.OnPlayerDied -= OnTargetDied;
                currentTarget = null;
            }

            isAttacking = false;
            animator.SetBool("IsAttacking", false);
        }
    }
}

