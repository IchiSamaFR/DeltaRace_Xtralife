using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float multipl = 1.5f;
    public bool destroy = true;

    public GameObject FX;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("IA"))
        {
            other.GetComponent<DeltaMovements>().SpeedBoost(multipl);
            if (destroy)
            {
                Destroy(this.gameObject);
            }
            if (FX && other.GetComponent<PlayerMovements>())
            {
                GameObject _fx = Instantiate(FX, GameMaster.instance.canvas.transform);
                Destroy(_fx, 2);
            }
        }
    }
}
