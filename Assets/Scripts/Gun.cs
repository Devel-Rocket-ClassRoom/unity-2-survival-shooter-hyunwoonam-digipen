using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public Transform fireTransform;

    private AudioSource gunAudioPlayer;
    public AudioClip shotClip;

    public LayerMask targetLayer;
    private LineRenderer bulletLineEffect;

    public ParticleSystem gunShotEfect;

    private float lastFireTime;
    private float fireDistance = 50f;

    public float timeBetFire = 0.12f;

    public int damage = 10;

    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineEffect = GetComponent<LineRenderer>();
    }
    public void Fire()
    {
        if (Time.time > lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;

            Shot();
        }
    }

    private void Shot()
    {
        Vector3 hitPosition = Vector3.zero;

        Ray ray = new Ray(fireTransform.position, fireTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            hitPosition = hit.point;

            Debug.Log("shot");
            var target = hit.collider.GetComponent<LivingEntity>();
            
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(CoShotEffect(hitPosition));
    }

    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        gunShotEfect.Play();

        gunAudioPlayer.PlayOneShot(shotClip);

        bulletLineEffect.SetPosition(0, fireTransform.position);
        bulletLineEffect.SetPosition(1, hitPosition);
        bulletLineEffect.enabled = true;

        yield return new WaitForSeconds(0.03f);

        bulletLineEffect.enabled = false;
    }

}
