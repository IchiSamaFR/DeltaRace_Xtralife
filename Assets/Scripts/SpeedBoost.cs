using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float multipl = 1.5f;
    public bool destroy = true;

    public GameObject FX;
    public GameObject cliff;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("IA"))
        {
            if (cliff)
            {
                bool result = other.GetComponent<DeltaMovements>().SpeedBoost(multipl, cliff);
                if (result && FX && other.GetComponent<PlayerMovements>())
                {
                    GameObject _fx = Instantiate(FX, GameMaster.instance.canvas.transform);
                    Destroy(_fx, 2);
                }
            }
            else
            {
                other.GetComponent<DeltaMovements>().SpeedBoost(multipl);

                if (FX && other.GetComponent<PlayerMovements>())
                {
                    GameObject _fx = Instantiate(FX, GameMaster.instance.canvas.transform);
                    Destroy(_fx, 2);
                }
            }
            if (destroy)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
