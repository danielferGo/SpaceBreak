using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;
    [SerializeField] private OVRSkeleton skeleton;
    [SerializeField] private OVRMicrogestureEventSource leftGestureSource;
    [SerializeField] private OVRMicrogestureEventSource rightGestureSource;
    [SerializeField] private float handRotationStrength = 2.0f;

    [SerializeField] private int health = 100;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float radiusShoot;
    [SerializeField] private float shootDistance;

    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private GameObject destroyedEffect;
    [SerializeField] private AudioSource shootAudioSource;

    private bool _moving;
    private Rigidbody _rigidbody;
    private Transform _rotationTransform;
    private Quaternion _baseRotation = Quaternion.identity;


    private void Start()
    {
        rightGestureSource.GestureRecognizedEvent.AddListener(gesture =>
            ControlSpaceShip(OVRPlugin.Hand.HandRight, gesture));
        leftGestureSource.GestureRecognizedEvent.AddListener(gesture =>
            ActivePower(OVRPlugin.Hand.HandLeft, gesture));
        _rigidbody = GetComponent<Rigidbody>();
        foreach (var bone in skeleton.Bones)
        {
            if (bone.Id != OVRSkeleton.BoneId.XRHand_Palm) continue;
            _rotationTransform = bone.Transform;
            break;
        }

        if (!_rotationTransform) _rotationTransform = transform;
    }

    private void ControlSpaceShip(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
    {
        var delta = Quaternion.identity;
        switch (gesture)
        {
            case OVRHand.MicrogestureType.ThumbTap:
                _moving = !_moving;
                break;

            case OVRHand.MicrogestureType.SwipeLeft:
                delta = Quaternion.Euler(0, -90f, 0);
                break;

            case OVRHand.MicrogestureType.SwipeRight:
                delta = Quaternion.Euler(0, 90f, 0);
                break;

            case OVRHand.MicrogestureType.SwipeForward:
                delta = Quaternion.Euler(-90f, 0, 0);
                break;

            case OVRHand.MicrogestureType.SwipeBackward:
                delta = Quaternion.Euler(90f, 0, 0);
                break;
        }

        _baseRotation *= delta;
    }

    private void ActivePower(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
    {
        switch (gesture)
        {
            case OVRHand.MicrogestureType.ThumbTap:
                Shoot();
                break;
        }
    }

    private void FixedUpdate()
    {
        var handRelativeRotation = Quaternion.Inverse(cameraRig.rotation) * _rotationTransform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, _baseRotation * handRelativeRotation,
            handRotationStrength * Time.deltaTime
        );
        if (!_moving) return;
        _rigidbody.linearVelocity = transform.forward * speed;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Spaceship destroyed!");
        var destroy = Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        destroy.SetActive(true);
        Destroy(destroy, 3f);
        gameObject.SetActive(false);
        // Destroy(gameObject);
    }

    public void ChangeDirection(int eventType)
    {
        var delta = Quaternion.identity;
        switch (eventType)
        {
            case 0: // Swipe Left
                delta = Quaternion.Euler(0, -90f, 0);
                break;
            case 1: // Swipe Right
                delta = Quaternion.Euler(0, 90f, 0);
                break;
            case 2: // Swipe Forward
                delta = Quaternion.Euler(-90f, 0, 0);
                break;
            case 3: // Swipe Backward
                delta = Quaternion.Euler(90f, 0, 0);
                break;
        }

        _baseRotation *= delta;
    }

    public void Teleport(Transform portal, float offset)
    {
        transform.position = portal.position + portal.forward * offset;
        transform.rotation = Quaternion.LookRotation(portal.forward, portal.up);
        _rigidbody.linearVelocity = Vector3.zero;
    }

    private void Shoot()
    {
        if (!shootEffect.isPlaying)
        {
            shootAudioSource.Play();
            shootEffect.Play();
        }

        var hits = Physics.SphereCastAll(transform.position, radiusShoot, transform.forward, shootDistance);
        foreach (var hit in hits)
        {
            if (!hit.collider.TryGetComponent<Alien>(out var alien)) continue;
            alien.Die();
        }
    }
    
    public bool IsAlive()
    {
        return health > 0;
    }
}