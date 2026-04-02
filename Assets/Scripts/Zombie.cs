using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntity
{
    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die,
    }

    public ZombieData zombieData;

    public Transform target;

    public HitBox hitBox;

    public LayerMask targetLayer;

    private NavMeshAgent agent;
    private Animator zombieAnimator;

    public float traceDistance = 50f;
    public float attackDistance = 1f;
    public float attackInterval = 0.5f;
    private float lastAttackTime = 0f;

    public int Score;

    private float deathTime;

    private float damage;

    private BoxCollider attackCollider;

    public AudioClip deathClip;
    public AudioClip hitClip;
    public ParticleSystem bloodEffect;
    public Collider zombieCollider;

    private AudioSource zombieAudioSource;

    private Status currentStatus;

    public Status CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            var prevStatus = currentStatus;
            currentStatus = value;

            Debug.Log(currentStatus);

            switch (currentStatus)
            {
                case Status.Idle:
                    zombieAnimator.SetBool("HasTarget", false);
                    agent.isStopped = true;
                    break;
                case Status.Trace:
                    zombieAnimator.SetBool("HasTarget", true);
                    agent.isStopped = false;
                    break;
                case Status.Attack:
                    zombieAnimator.SetBool("HasTarget", false);
                    agent.isStopped = true;
                    break;
                case Status.Die:
                    zombieAnimator.SetTrigger("Die");
                    agent.isStopped = true;
                    zombieCollider.enabled = false;
                    hitBox.Colliders.Clear();
                    hitBox.gameObject.SetActive(false);
                    zombieAudioSource.PlayOneShot(deathClip);
                    deathTime = Time.time;
                    break;
            }
        }
    }

    public void Setup()
    {
        //gameObject.SetActive(false);

        startingHealth = zombieData.maxHp;
        damage = zombieData.damage;
        agent.speed = zombieData.speed;

        Score = zombieData.Score;

        //gameObject.SetActive(true);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();

        zombieAudioSource = GetComponent<AudioSource>();

        attackCollider = GetComponent<BoxCollider>();

        //zombieAnimator.SetBool("HasTarget", true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentStatus)
        {
            case Status.Idle:
                UpdateIdle();
                break;
            case Status.Trace:
                UpdateTrace();
                break;
            case Status.Attack:
                UpdateAttack();
                break;
            case Status.Die:
                UpdatDie();
                break;
        }


        //agent.SetDestination(target.position);
    }

    private void UpdatDie()
    {
        StartSinking();
        Debug.Log("Die");
    }

    private void UpdateAttack()
    {
        if (target == null)
        {
            CurrentStatus = Status.Idle;
            return;
        }

        if (target != null)
        {
            var find = hitBox.Colliders.Find(x => x.transform == target);

            if (find == null)
            {
                CurrentStatus = Status.Trace;
                return;
            }
        }

        //if (target == null || Vector3.Distance(target.position, transform.position) > attackDistance)
        //{
        //    CurrentStatus = Status.Trace;
        //    return;
        //}
        //
        var lookAt = target.position;
        lookAt.y = transform.position.y;

        transform.LookAt(lookAt);

        if (Time.time > lastAttackTime + attackInterval)
        {
            Debug.Log("damage");

            lastAttackTime = Time.time;

            var livingEntity = target.GetComponent<LivingEntity>();

            if (livingEntity != null)
            {
                if (!livingEntity.IsDead)
                {
                    livingEntity.OnDamage(damage, transform.position, -transform.forward);
                }
            }
        }
    }

    private void UpdateTrace()
    {
        if (target != null)
        {
            var find = hitBox.Colliders.Find(x => x.transform == target);

            Debug.Log(find);

            if (find != null)
            {
                CurrentStatus = Status.Attack;
                return;
            }
        }

        //if (target != null && Vector3.Distance(target.position, transform.position) < attackDistance)
        //{
        //    CurrentStatus = Status.Attack;
        //    return;
        //}

        if (target == null || Vector3.Distance(target.position, transform.position) > traceDistance)
        {
            target = null;
            CurrentStatus = Status.Idle;
            return;
        }

        agent.SetDestination(target.position);
    }

    private void UpdateIdle()
    {
        if (target != null
            && Vector3.Distance(target.position, transform.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        target = FindTarget(traceDistance);
    }

    private Transform FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);

        if (colliders.Length == 0)
        {
            return null;
        }

        var target = colliders.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First();

        return target.transform;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        agent.enabled = true;
        agent.isStopped = false;
        agent.ResetPath();

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        zombieCollider.enabled = true;

        currentStatus = Status.Idle;

    }

    private void OnDisable()
    {
        hitBox.Colliders.Clear();
        hitBox.gameObject.SetActive(false);
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsDead)
        {
            zombieAudioSource.PlayOneShot(hitClip);
        }

        base.OnDamage(damage, hitPoint, hitNormal);

        bloodEffect.transform.position = hitPoint;
        bloodEffect.transform.forward = hitNormal;
        bloodEffect.Play();

    }

    public override void Die()
    {
        CurrentStatus = Status.Die;

        if (IsDead)
            return;

        base.Die();
    }

    public void StartSinking()
    {
        if (agent != null)
        {
            agent.enabled = false;
        }
        if (Time.time > deathTime + 0.5f)
        {
            if (agent != null && agent.enabled)
            {
                agent.enabled = false;
            }

            transform.Translate(Vector3.down * 1.5f * Time.deltaTime, Space.World);
        }
    }
}

