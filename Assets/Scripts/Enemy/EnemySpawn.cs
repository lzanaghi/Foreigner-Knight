using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform player; // Referência ao jogador

    void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Aqui atribuímos a referência do jogador ao inimigo recém-instanciado
            Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.player = player;
            }
        }
    }
}