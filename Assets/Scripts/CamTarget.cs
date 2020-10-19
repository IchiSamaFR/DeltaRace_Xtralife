using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 toGo;

    public Vector3 distanceWithTarget = new Vector3();

    private void Start()
    {
        if (!target)
        {
            print("Error, target missing");
            return;
        }

        distanceWithTarget = transform.position - target.position;
    }

    
    void Update()
    {
        if (!target.GetComponent<PlayerMovements>().isDead)
        {
            toGo = target.transform.position + distanceWithTarget;
        }
        else
        {
            if(toGo.y < 2)
            {
                toGo.y = 2f;

            }
        }
        this.transform.position = Vector3.Lerp(transform.position, toGo, 4 * Time.deltaTime);
    }
}
