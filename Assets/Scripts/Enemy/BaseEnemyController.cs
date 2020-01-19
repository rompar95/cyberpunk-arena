﻿using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyController : MonoBehaviour
{ 
    [Header("Stats")]
    public int maxHealth;
    public int damage;
    public float respawnTime;
    public float respawnTimer;
    public int currentHealth;

    [Header("Weapons")]
    public bool multishot;
    public AudioClip shotClip;
    public AudioSource audioSource;
    public GameObject shootPos;
    public float recoil = 0;
    public GameObject bulletPool;
    public GameObject weapon;
    //public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask playerLayer;
    public float attackRate = 1f;
    public float nextAttackTime = 0f;

    public GameObject respawnTarget;

    [Header("VFX")]
    public GameObject respawnVFX;
    private Renderer renderer;
    //public GameObject deadVFX;

    public Animator anim;
    private Rigidbody rb;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    public Transform target;
    public NavMeshAgent agent;
    public bool dead = false;

    private float run = 1f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponentInChildren<Renderer>();
        //agent.updatePosition = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!dead)
            Run();
        DeathHandler();
        anim.SetBool("Dead", dead);
    }

    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public virtual void Attack()
    {
        if (multishot == true)
        {
            audioSource.PlayOneShot(shotClip);
            int[] pool = { -5, -2, 0, 2, 5 };
            for (int i = 0; i < 5; i++)
            {
                shootPos.transform.localRotation = Quaternion.Euler(Random.Range(-recoil, recoil), Random.Range(-recoil, recoil) + pool[i], 0);
                CreateBullet();
                shootPos.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            audioSource.PlayOneShot(shotClip);
            shootPos.transform.localRotation = Quaternion.Euler(Random.Range(-recoil, recoil), Random.Range(-recoil, recoil), 0);
            CreateBullet();
        }
    }   

    private void CreateBullet()
    {
        GameObject newbullet = bulletPool.transform.GetChild(0).gameObject;
        newbullet.GetComponent<BaseEnemyBulletController>().damage = this.damage;
        newbullet.transform.position = shootPos.transform.position;
        newbullet.transform.rotation = shootPos.transform.rotation;
        newbullet.transform.SetParent(null);
        newbullet.SetActive(true);
    }

    public virtual void Run()
    {
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(target.position, transform.position);

        FaceTarget();

        if (distance <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackRate;
            }            
        }

        //Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        //float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        //float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        //Vector3 deltaPosition = new Vector2(dx, dy);

        //float smooth = Mathf.Min(1f, Time.deltaTime / 0.15f);
        //smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        //if (Time.deltaTime > 1e-5f)
        //{
        //    velocity = smoothDeltaPosition / Time.deltaTime;
        //}

        //bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        //anim.SetBool("Move", shouldMove);
    }

    private void OnAnimatorMove()
    {
        transform.position = agent.nextPosition;
    }

    //private void UpdateAnimator()
    //{
    //    anim.SetFloat("Run", run);
    //    anim.SetFloat("Speed", agent.speed);
    //}

    public void DeathHandler()
    {
        if (currentHealth < 1)
        {
            agent.isStopped = true;
            dead = true;           
            Respawn();
        }
    }

    /*void DeathExtra()
    {
        if (health <= -50 && DeadExtra == false)
        {
            Instantiate(DeadVFX, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.rotation);
            Weapons.transform.GetComponent<WeaponController>().shooting = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            Weapons.SetActive(false);
            rb.velocity = Vector3.zero;
            DeadExtra = true;
            Dead = true;
        }
    }*/

    public virtual void Respawn()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer > respawnTime)
        {
            Instantiate(respawnVFX, transform.position, transform.rotation);
            currentHealth = maxHealth;
            dead = false;            
            Vector3 resp = respawnTarget.transform.GetChild(Random.Range(0, respawnTarget.transform.childCount)).transform.position;
            transform.position = resp;
            respawnTimer = 0.0f;
            agent.isStopped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullets"))
            GetDamage();
    }

    public void GetDamage()
    {
        // TODO: перенести сюда код из BulletTest
        //renderer.material.color = Color.red;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
