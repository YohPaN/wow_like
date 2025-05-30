using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] PlayerHealthUI healthUI;
        public int currentHealth = 100;
        public int maxHealth = 100;
        public float chaseRange = 10f;
        public float attackRange = 30f;
        public float xpGain = 4;
        public event System.Action OnEnemyDied;
        public PlayerController target; // Set this to the player in inspector or script
        public Animator animator;
        private NavMeshAgent agent;
        public EnemyAttack attack;

        void Start()
        {
            // get the agent (the enemy)
            agent = GetComponent<NavMeshAgent>();
            // get the target to make a path finding to the player 
            target = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            animator = GetComponent<Animator>();
            attack = GetComponent<EnemyAttack>();
        }

        void Update()
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);

            if (target == null) return;

            // get the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, target.transform.position);

            // if a target was find, that the enemy can detect it (distance <= chaseRange) 
            // and that the enemy is not already next to the player 
            if (distance <= chaseRange && distance >= attackRange)
            {
                // give a destination to the enemy that will be manage by AI with NavMesh
                agent.SetDestination(target.transform.position);
                animator.SetBool("IsMoving", true);
            }
            else if (distance <= attackRange)
            {
                agent.ResetPath();
                animator.SetBool("IsMoving", false);

                attack.TryAttack(target);
            }
            else
            {
                // if there is no target or the target is too far or the target is already next to the enemy, 
                // disable Moving animation 
                agent.ResetPath();
                animator.SetBool("IsMoving", false);
            }
        }

        // A function that manage currentHealth and trigger Die if no more life point
        public void TakeDamage(int amount)
        {
            healthUI.ShowUI(true);

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Die();
            }

            animator.Play("GetHit01_SwordAndShield");
        }

        void Die()
        {
            healthUI.ShowUI(false);
            OnEnemyDied?.Invoke(); // notify spawner
            Destroy(gameObject); // Remove the game object
        }
    }
}
