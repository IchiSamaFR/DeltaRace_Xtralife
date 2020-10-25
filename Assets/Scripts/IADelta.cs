using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IADelta : DeltaMovements
{
    public List<Transform> path = new List<Transform>();
    int indexPath = 0;

    GameMaster gm;
    MapGeneration mg;

    bool hasGo = false;

    private void Start()
    {
        gm = GameMaster.instance;
        mg = MapGeneration.instance;
        _init_();
        StartCoroutine("wait");
    }

    public IEnumerator wait()
    {
        yield return new WaitForSeconds(0.2f);
        
        path = mg.GetIAPath();
    }

    private void Update()
    {
        if (!gm.started || isDead || indexPath >= path.Count)
        {
            return;
        }
        Move();
        if (indexPath < path.Count)
        {
            if (transform.position.y + 0.2f > path[indexPath].position.y)
            {
                Dive();
            }
            else if(transform.position.y - 0.2f < path[indexPath].position.y)
            {
                FlyUp();
            }

            if(transform.position.z + 0.3f >= path[indexPath].position.z && transform.position.z - 0.3f <= path[indexPath].position.z)
            {
                indexPath++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            GetDmg(other.gameObject);
        }
        else if (other.CompareTag("Water"))
        {
            GetDmg(other.gameObject, initialLives, "instantDeath");
        }
    }
}
