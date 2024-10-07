using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBombShot : MonoBehaviour
{
    public GameObject bomb;
    public Transform bombPos;


    public void ThrowBomb()
    {
        GameObject newBomb = Instantiate(bomb, bombPos.position, Quaternion.identity);
        // Atribui a referência do Goblin para a bomba
        GoblinBomb bombScript = newBomb.GetComponent<GoblinBomb>();
        if (bombScript != null)
        {
            bombScript.SetGoblinReference(GetComponent<Goblin>());
        }
    }
}