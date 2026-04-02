using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;

    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource playerAudioSource;
    private Animator playerAnimator;

    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    public Image damageScreen;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);
    private bool isDamage = false;

    private void Awake()
    {
        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (healthSlider != null)
        {
            healthSlider.maxValue = startingHealth; 
            healthSlider.value = Health;            
        }

        playerMovement.enabled = true;
        playerShooter.enabled = true;

    }

    public void Update()
    {
        if (!IsDead)
        {
            if (damageScreen != null)
            {
                if (isDamage)
                {
                    damageScreen.color = flashColor;
                }
                else
                {
                    damageScreen.color = Color.Lerp(damageScreen.color, Color.clear, 5f * Time.deltaTime);
                }
            }

            isDamage = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //var item = other.GetComponent<IItem>();
        //
        //if (item != null)
        //{
        //    item.Use(gameObject);
        //}
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsDead)
        {
            playerAudioSource.PlayOneShot(hitClip);
        }

        base.OnDamage(damage, hitPoint, hitNormal);

        isDamage = true;

        if (healthSlider != null)
        {
            healthSlider.value = Health;
        }
    }

    public override void OnHeal(float add)
    {
        base.OnHeal(add);

        if (healthSlider != null)
        {
            healthSlider.value = Health;
        }
    }

    public override void Die()
    {
        if (IsDead)
            return;

        base.Die();

        damageScreen.color = new Color(1f, 0f, 0f, 0.01f);

        playerAudioSource.PlayOneShot(deathClip);
        playerAnimator.SetTrigger("Die");

        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }
}
