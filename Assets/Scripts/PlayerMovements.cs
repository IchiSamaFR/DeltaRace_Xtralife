using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : DeltaMovements
{
    GameMaster gm;
    [Header("PlayerMovements")]
    public GameObject speedParticles;
    public GameObject winParticles;

    public float timeBefGame = 1;
    public bool canPlay = false;

    [Header("Sounds")]
    public AudioSource windAudio;
    public AudioSource coinsAudio;


    void Start()
    {
        CheckAudio();
        _init_();
        gm = GameMaster.instance;
        speedParticles.SetActive(false);
    }

    public override void _init_()
    {
        base._init_();
        StartCoroutine("timerBefPlay");
    }

    /* Rotate by inputs and launch game too
     */
    void Update()
    {
        if (isDead || !canPlay)
        {
            return;
        }
        if (!gm.started && (Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !gm.IsOverUI())
        {
            gm.StartGame();
        }

        if (!isDead && gm.started)
        {
            Move();
            if (((Input.GetMouseButton(0) || Input.touchCount > 0) && transform.position.y > minHeight) 
                || transform.position.y > maxHeight)
            {
                Dive();
            }
            else
            {
                FlyUp();
            }
        }
        CheckParticles();
        CheckAudio();
    }

    /* Set death to the game master
     */
    public override void Death()
    {
        base.Death();
        gm.PlayerDeath();
    }

    /* Set win to the game master
     */
    public void Win()
    {
        GameObject _particules = Instantiate(winParticles, transform.position, transform.rotation);
        Destroy(_particules, 5);
        canPlay = false;
        gm.Win();
    }

    /* Change particles speed by speed
     */
    public void CheckParticles()
    {
        if (actualSpeed >= maxSpeed * 0.9f && !speedParticles.activeSelf)
        {
        }
        else if (actualSpeed < maxSpeed * 0.9f && speedParticles.activeSelf)
        {
        }
        speedParticles.SetActive(true);

        Vector3 _rotation = transform.rotation.eulerAngles;
        speedParticles.GetComponent<ParticleSystem>().emissionRate = actualSpeed / maxSpeed * 40;
        if (_rotation.x > 0 && _rotation.x < 180)
        {
            speedParticles.GetComponent<ParticleSystem>().startSpeed = actualSpeed / maxSpeed * 150;
        }
        else if (actualSpeed > maxSpeed)
        {
            speedParticles.GetComponent<ParticleSystem>().startSpeed = actualSpeed / maxSpeed * 120;
        }
        else
        {
            speedParticles.GetComponent<ParticleSystem>().startSpeed = actualSpeed / maxSpeed * 100;
        }
    }

    float volumeBase = 0.35f;
    public void CheckAudio()
    {
        if (!SoundsManager.active)
        {
            windAudio.volume = 0;
            return;
        }
        Vector3 _rotation = transform.rotation.eulerAngles;
        if (_rotation.x > 0 && _rotation.x < 180)
        {
            float vol = volumeBase + _rotation.x / dive_Max;
            if(vol > 1)
            {
                vol = 1;
            }
            windAudio.volume = vol;
        }
        else
        {
            if(windAudio.volume > volumeBase)
            {
                windAudio.volume = Mathf.Lerp(windAudio.volume, volumeBase, Time.deltaTime);
            }
            else
            {
                windAudio.volume = volumeBase;
            }
        }
    }

    public void GetCoins()
    {
        coinsAudio.Play();
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
        else if (other.CompareTag("Finish"))
        {
            Win();
        }
    }

    public IEnumerator timerBefPlay()
    {
        canPlay = false;
        yield return new WaitForSeconds(timeBefGame);
        canPlay = true;
    }
}
