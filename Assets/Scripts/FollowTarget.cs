using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public bool xInd;
    public bool yInd;
    public bool zInd;

    public bool byInt = true;

    public Transform target;

    Vector3 before = new Vector3();
    public Vector3 distanceWithTarget = new Vector3();
    
    void Start()
    {
        distanceWithTarget = transform.position - target.position;
        before = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _toGo = transform.position;
        if (!byInt)
        {
            if (xInd)
            {
                _toGo.x = target.transform.position.x + distanceWithTarget.x;
            }
            else
            {
                _toGo.x = transform.position.x;
            }

            if (yInd)
            {
                _toGo.y = target.transform.position.y + distanceWithTarget.y;
            }
            else
            {
                _toGo.y = transform.position.y;
            }

            if (zInd)
            {
                _toGo.z = target.transform.position.z + distanceWithTarget.z;
            }
            else
            {
                _toGo.z = transform.position.z;
            }
        }
        else
        {
            Vector3 _actual = target.transform.position;
            if (xInd && before.x - 10 > _actual.x)
            {
                _toGo.x -= 10;
                before.x -= 10;
            }
            else if(xInd && before.x + 10 < _actual.x)
            {
                _toGo.x += 10;
                before.x += 10;
            }

            if (yInd && before.y - 10 > _actual.y)
            {
                _toGo.y -= 10;
                before.y -= 10;
            }
            else if (yInd && before.y + 10 < _actual.y)
            {
                _toGo.y += 10;
                before.y += 10;
            }

            if (zInd && before.z - 10 > _actual.z)
            {
                _toGo.z -= 10;
                before.z -= 10;
            }
            else if (zInd && before.z + 10 < _actual.z)
            {
                _toGo.z += 10;
                before.z += 10;
            }
        }
        
        this.transform.position = _toGo;
    }
}
