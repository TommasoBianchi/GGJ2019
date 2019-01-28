using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    [SerializeField]
    private GameObject particleSystemPrefab;

    public float HitDamage;

    private void Awake()
    {
        Destroy(Instantiate(particleSystemPrefab, transform.position, Quaternion.identity), 10);
    }

    public void SetSpeed(float speed)
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "PlayerShell")
        {
            Destroy(gameObject);
        }
    }
}
