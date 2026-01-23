using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    public float Speed = 10;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject,2);
    }


    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = transform.forward * Speed / Time.timeScale;
    }
}
