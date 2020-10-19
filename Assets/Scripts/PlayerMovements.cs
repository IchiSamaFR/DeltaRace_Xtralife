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


    void Start()
    {
        _init_();
        gm = GameMaster.instance;
        speedParticles.SetActive(false);
    }

    public override void _init_()
    {
        base._init_();
        StartCoroutine("timerBefPlay");
    }

    void Update()
    {
        if (isDead || !canPlay)
        {
            return;
        }
        if (!gm.started && (Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !gm.IsOverUI())
        {
            gm.StartGame();
            actualSpeed = initialSpeed;
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
    }

    public override void Death()
    {
        base.Death();
        gm.PlayerDeath();
    }

    public void Win()
    {
        GameObject _particules = Instantiate(winParticles, transform.position, transform.rotation);
        canPlay = false;
        gm.Win();
    }

    public void CheckParticles()
    {
        if (actualSpeed >= maxSpeed * 0.9f && !speedParticles.activeSelf)
        {
        }
        else if (actualSpeed < maxSpeed * 0.9f && speedParticles.activeSelf)
        {
        }
        speedParticles.SetActive(true);
        if (actualSpeed > maxSpeed)
        {
            speedParticles.GetComponent<ParticleSystem>().emissionRate = actualSpeed / maxSpeed * 60;
            speedParticles.GetComponent<ParticleSystem>().startSpeed = actualSpeed / maxSpeed * 200;
        }
        else
        {
            speedParticles.GetComponent<ParticleSystem>().emissionRate = actualSpeed / maxSpeed * 40;
            speedParticles.GetComponent<ParticleSystem>().startSpeed = actualSpeed / maxSpeed * 100;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            GetDmg();
        }
        else if (other.CompareTag("Water"))
        {
            GetDmg(initialLives, "instantDeath");
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
