using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float multipl = 1.5f;
    public bool destroy = true;

    public GameObject FX;
    public GameObject cliff;

    bool needDestroy = false;

    public GameObject model;
    AudioSource src;

    private void Start()
    {
        src = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (destroy && needDestroy && ((src && !src.isPlaying) || !src))
        {
            Destroy(this.gameObject);
        }
    }


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

                    needDestroy = true;

                    if (SoundsManager.active && src)
                    {
                        src.Play();
                    }
                }
            }
            if (destroy && !src)
            {
                Destroy(this.gameObject);
            }
            else if (model)
            {
                Destroy(model);
            }
        }
    }
}
