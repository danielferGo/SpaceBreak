using UnityEngine;

public class Alien : MonoBehaviour
{
    [SerializeField] private Transform spaceShip;
    [SerializeField] private float velocity = 0.5f;
    [SerializeField] private float shootDistance = 10f;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private LayerMask spaceshipLayer;
    [SerializeField] private float radius;
    [SerializeField] private AudioSource shooting;
    [SerializeField] private GameObject destroyedEffect;

    private SpaceShip _spaceShipComponent;
    private float _lastShootTime;

    private void Start()
    {
        _spaceShipComponent = spaceShip.GetComponent<SpaceShip>();
    }

    public void Init(Transform player)
    {
        spaceShip = player;
    }

    private void FixedUpdate()
    {
        if(!_spaceShipComponent.IsAlive()) return;
        transform.position = Vector3.MoveTowards(transform.position, spaceShip.position, velocity * Time.deltaTime);
        transform.LookAt(spaceShip);

        var distance = Vector3.Distance(transform.position, spaceShip.position);
        if (distance <= shootDistance + 0.1f && Time.time - _lastShootTime >= shootCooldown)
        {
            Shoot();
            _lastShootTime = Time.time;
        }

        if (!(distance <= 0.1f)) return;
        _spaceShipComponent.TakeDamage(2);
        Die();
    }

    private void Shoot()
    {
        if (!shootEffect.isPlaying)
        {
            shooting.Play();
            shootEffect.Play();
        }

        if (!Physics.SphereCast(transform.position, radius, transform.forward,
                out var hit, shootDistance, spaceshipLayer)) return;
        if (hit.transform == spaceShip)
        {
            _spaceShipComponent.TakeDamage(1);
        }
    }

    public void Die()
    {
        var destroy = Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        destroy.SetActive(true);
        Destroy(destroy, 0.5f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        var distance = Vector3.Distance(transform.position, spaceShip.position);
        Debug.Log(distance);
        Gizmos.color = Color.red;
        if (Physics.SphereCast(transform.position, radius, transform.forward,
                out RaycastHit hit, shootDistance, spaceshipLayer))
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.DrawWireSphere(hit.point, radius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position + transform.forward * shootDistance, radius);
        }
    }
}