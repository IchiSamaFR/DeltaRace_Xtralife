﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public bool xInd;
    public bool yInd;
    public bool zInd;

    public bool byInt = true;

    public Transform target;

    public Vector3 distanceWithTarget = new Vector3();
    
    void Start()
    {
        distanceWithTarget = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _toGo = new Vector3();
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

            Vector3 _distance = transform.position - target.position;
            if (xInd && _distance.x > 1)
            {
                _toGo.x = target.transform.position.x + 1;
            }
            else
            {
                _toGo.x = transform.position.x;
            }
            if (yInd && _distance.y > 1)
            {
                _toGo.y = target.transform.position.y;
            }
            else
            {
                _toGo.y = transform.position.y;
            }
            if (zInd && _distance.z > 1)
            {
                _toGo.z = target.transform.position.z + 1;
            }
            else
            {
                _toGo.z = transform.position.z;
            }
        }
        
        this.transform.position = _toGo;
    }
}
