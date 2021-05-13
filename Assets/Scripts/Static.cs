using UnityEngine;
public class Static : MonoBehaviour
{
    new Rigidbody rigidbody;
    public Vector3 position, velocity, angularVelocity;
    public bool isColliding;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isColliding)
        {
            position = rigidbody.position;
            velocity = rigidbody.velocity;
            angularVelocity = rigidbody.angularVelocity;
        }
    }

    void LateUpdate()
    {
        if (isColliding)
        {
            rigidbody.position = position;
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
            isColliding = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
            isColliding = false;
    }
}