using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DeltaMovements : MonoBehaviour
{
    [Header("Speed stats")]
    public float initialSpeed;
    public float actualSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float dive_SpeedMult = 0.5f;
    public float flyup_SpeedMult = 0.1f;

    [Header("Transform stats")]
    public float timeToDive = 1;
    float timerDive = 0;
    public float dive_TransfMult = 1f;
    public float dive_Max = 45;
    public float timeToFylup = 1;
    float timerFlyup = 0;
    public float flyup_TransfMult = 1f;
    public float flyup_Max = 45;

    public float minTimer = 0.5f;


    [Header("Stats")]
    public float maxHeight = 10;
    public float minHeight = 2;
    public int initialLives = 1;
    [SerializeField]
    int lives;
    public bool isDead;
    public float timeBeforeNextDmg = 2;
    public float speedMultiplDmg = 0.7f;
    bool canTakeDmg = true;
    public GameObject deadParticles;

    GameObject lastHitted;
    GameObject lastBoost;
    Animator anim;

    public virtual void _init_()
    {
        if (!deadParticles)
        {
            Debug.LogError("Particules of death missings.");
        }
        lives = initialLives;
        isDead = false;
        actualSpeed = initialSpeed;
        anim = GetComponent<Animator>();
        anim.SetBool("isDead", false);

        timerDive = minTimer;
        timerFlyup = minTimer;
    }

    /* Dive when hold
     * > void type
     * 
     * Change the rotation and the speed by multiplier
     */
    public void Dive()
    {
        /* Get the actual speed and increases.
         * Get The actual rotation and decreases.
         */
        if (actualSpeed > maxSpeed)
        {
            Decelerate();
        }
        else if (actualSpeed < minSpeed)
        {
            Accelerate();
        }
        if(actualSpeed * 1 + (dive_SpeedMult * Time.deltaTime) < maxSpeed)
        {
            actualSpeed *= 1 + (dive_SpeedMult * Time.deltaTime);
        }
        else
        {
            Decelerate();
        }

        Vector3 _rotation = transform.rotation.eulerAngles;

        float _multiplier = dive_TransfMult;
        if (_rotation.x + _multiplier * Time.deltaTime > dive_Max && _rotation.x < 180)
        {
            transform.rotation = Quaternion.Euler(dive_Max, _rotation.y, _rotation.z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(_rotation.x + (_multiplier * Time.deltaTime) * (timerDive / timeToDive), _rotation.y, _rotation.z);
        }


        if (timerFlyup > minTimer)
        {
            timerFlyup -= Time.deltaTime / timeToFylup;
        }
        else
        {
            timerFlyup = minTimer;
        }

        if (timerDive < 1)
        {
            timerDive += Time.deltaTime / timeToDive;
        }
        else
        {
            timerDive = 1;
        }
    }

    /* Fly up when release
     * > void type
     * 
     * Change the rotation and the speed by multiplier
     */
    public void FlyUp()
    {
        /* Get the actual speed and decreases.
         * Get The actual rotation and increases.
         */
        if (actualSpeed > maxSpeed)
        {
            Decelerate();
        }
        else if (actualSpeed < minSpeed)
        {
            Accelerate();
        }
        if(actualSpeed * 1 + (flyup_SpeedMult * Time.deltaTime) > minSpeed)
        {
            actualSpeed *= 1 - (flyup_SpeedMult * Time.deltaTime);
        }
        else
        {
            Accelerate();
        }

        Vector3 _rotation = transform.rotation.eulerAngles;

        float _multiplier = flyup_TransfMult;
        if (_rotation.x - _multiplier * Time.deltaTime < 360 - flyup_Max && _rotation.x > 180)
        {
            transform.rotation = Quaternion.Euler(-flyup_Max, _rotation.y, _rotation.z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(_rotation.x - (_multiplier * Time.deltaTime) * (timerFlyup / timeToFylup), _rotation.y, _rotation.z);
        }

        if(timerDive > minTimer)
        {
            timerDive -= Time.deltaTime / timeToDive;
        }
        else
        {
            timerDive = minTimer;
        }

        if (timerFlyup < 1)
        {
            timerFlyup += Time.deltaTime / timeToFylup;
        }
        else
        {
            timerFlyup = 1;
        }
    }

    /* Function to move forward by speed
     * > void type
     */
    public void Move()
    {
        transform.position += transform.forward * actualSpeed * Time.deltaTime;
    }

    void Decelerate()
    {
        float _diff = actualSpeed - maxSpeed;

        actualSpeed = Mathf.Lerp(actualSpeed, maxSpeed, _diff / 3 * Time.deltaTime);
    }

    void Accelerate()
    {
        float _diff = actualSpeed - minSpeed;

        actualSpeed = Mathf.Lerp(actualSpeed, minSpeed, _diff * Time.deltaTime);
    }

    /* Get dmg like when collide with a cliff / wall
     * 
     */
    public void GetDmg(GameObject cliff, int _dmg = 1, string type = "")
    {
        if (SoundsManager.active && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Play();
        }

        if (!canTakeDmg && type != "instantDeath" || lastHitted == cliff)
        {
            return;
        }
        lives -= _dmg;
        if(lives <= 0)
        {
            Death();
        }
        else
        {
            lastHitted = cliff;
            actualSpeed *= speedMultiplDmg;
            anim.SetBool("getDmg", true);
            StartCoroutine("invicible");
        }
    }
    public virtual void Death()
    {
        isDead = true;
        GameObject _particules = Instantiate(deadParticles, transform.position, transform.rotation);
        anim.SetBool("isDead", true);
    }

    /* Ienumerator to wait until the invicibility is done
     */
    public IEnumerator invicible()
    {
        canTakeDmg = false;
        yield return new WaitForSeconds(timeBeforeNextDmg);
        canTakeDmg = true;
        anim.SetBool("getDmg", false);
    }

    public bool SpeedBoost(float multipl)
    {
        actualSpeed *= multipl;
        return true;
    }
    public bool SpeedBoost(float multipl, GameObject cliff)
    {
        if(lastHitted != cliff && lastBoost != cliff)
        {
            lastBoost = cliff;
            actualSpeed *= multipl;
            return true;
        }
        return false;
    }

}
