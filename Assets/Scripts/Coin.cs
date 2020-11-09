using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject model;
    bool destroyed = false;

    private void Update()
    {
        if (destroyed)
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameMaster.instance.GetCoin();

            destroyed = true;
            if (SoundsManager.active)
            {
                GetComponent<AudioSource>().Play();
            }
            Destroy(model);
        }
    }
}
