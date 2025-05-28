using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] private OVRMicrogestureEventSource leftGestureSource;

    [SerializeField] private OVRMicrogestureEventSource rightGestureSource;

    [SerializeField] private Transform rotationTransform;
    [SerializeField] private float rotationSpeed = 45f; // degrees per second

    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private OVRHand _hand;
    [SerializeField] private GameObject destroyedEffect;

    public OVRSkeleton skeleton;

    private OVRBone _bone;
    [SerializeField] private int health = 100;

    [SerializeField] private float speed = 2f;

    public bool moving;
    private Rigidbody _rigidbody;

    private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private float rotationBlendFactor = 1f; // how much to blend hand with movement


    private Vector3 forward = Vector3.forward; // Ship's forward direction
    private Vector3 up = Vector3.up; // Ship's up direction
    private Vector3 right = Vector3.right; // Ship's right direc


    private float currentPitchInput = 0f;
    private float currentYawInput = 0f;
    private Quaternion targetRotation;


    private Quaternion baseRotation = Quaternion.identity; // Updated on swipe
    private Quaternion handRotation = Quaternion.identity; // Based on hand

    public Transform cameraRig; // Usually the XR camera or main player camera
    public float handRotationStrength = 2.0f; // 1 = normal, 2 = double strength, etc.

    void Start()
    {
        leftGestureSource.GestureRecognizedEvent.AddListener(gesture =>
            OnGestureRecognized(OVRPlugin.Hand.HandRight, gesture));
        rightGestureSource.GestureRecognizedEvent.AddListener(gesture =>
            OnGesturePowerRecognized(OVRPlugin.Hand.HandLeft, gesture));
        _rigidbody = GetComponent<Rigidbody>();
        foreach (var b in skeleton.Bones)
        {
            if (b.Id == OVRSkeleton.BoneId.XRHand_Palm)
            {
                _bone = b;
                rotationTransform = b.Transform;
                break;
            }
        }
    }

    private void OnGestureRecognized(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
    {
        var delta = Quaternion.identity;
        switch (gesture)
        {
            case OVRHand.MicrogestureType.ThumbTap:
                Shoot();
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

        baseRotation *= delta;
    }

    private void OnGesturePowerRecognized(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
    {
        switch (gesture)
        {
            case OVRHand.MicrogestureType.ThumbTap:
                moving = !moving;
                break;
        }
    }

    private void FixedUpdate()
    {
        var handRelativeRotation = Quaternion.Inverse(cameraRig.rotation) * rotationTransform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * handRelativeRotation,
            handRotationStrength * Time.deltaTime
        );
        if (!moving) return;
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
        Destroy(gameObject);
        // gameObject.SetActive(false);
    }


    private bool isFacingUp = false;
    [SerializeField] private float radiusShoot;
    [SerializeField] private float shootDistance;


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

        baseRotation *= delta;
    }

    public void Teleport(Transform portal, float offset)
    {
        transform.position = portal.position + portal.forward * offset;
        transform.rotation = Quaternion.LookRotation(portal.forward, portal.up);
        _rigidbody.linearVelocity = Vector3.zero;
    }

    public void Shoot()
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
}