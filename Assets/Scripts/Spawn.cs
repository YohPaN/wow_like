using UnityEngine;

namespace Assets.Scripts
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public Transform[] spawnPoints; // you can add multiple spawn positions
        public float spawnInterval = 3f;
        public int maxEnemies = 5;
        public BoxCollider spawnZone;
        private int currentEnemies = 0;

        void Start()
        {
            // invok repeatedly the methode SpawnEnemy with an interval
            InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
        }

        void SpawnEnemy()
        {
            // limit the number of annemies and return it there is already anough ennemy
            if (currentEnemies >= maxEnemies) return;

            // get the bound of the spawn box
            Bounds bounds = spawnZone.bounds;

            // get a random position to spawn the ennemy. 
            // It use Random.range that choose a value between bounds.min.foo and bounds.max.foo
            Vector3 spawnPoint = new(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // instanciate a new ennemy instance
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            currentEnemies++;

            // attach a listner on OnEnemyDied (Method that came from Enemy class) 
            // that will decrease the enemy counter 
            enemy.GetComponent<Enemy>().OnEnemyDied += () => currentEnemies--;
        }
    }
}
