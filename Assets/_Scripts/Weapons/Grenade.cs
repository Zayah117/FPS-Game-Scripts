using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour {
    Rigidbody rb;
    public float timer;
    public float explosionRadius;
    public int damage;
    public bool impact = false;
    public LayerMask obstacleMask;
    public ParticleSystem particlePrefab;
    [SerializeField] bool randomTorqueOnStart;
    [SerializeField] float torqueAmount = 300;
    [HideInInspector] public Character character;

	void Start () {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PullPin());
        if (randomTorqueOnStart) {
            rb.AddTorque(new Vector3(Random.Range(-torqueAmount, torqueAmount), Random.Range(-torqueAmount, torqueAmount), Random.Range(-torqueAmount, torqueAmount)));
        }
	}

    IEnumerator PullPin() {
        yield return new WaitForSeconds(timer);
        Explode();
    }

    void OnCollisionEnter(Collision collision) {
        if (impact) {
            Explode();
        }

        Character character = collision.gameObject.GetComponent<Character>();
        if (character) {
            Explode();
        }
    }

    void Explode() {
        Explosions.Explosion(transform.position, explosionRadius, damage, particlePrefab, character);
        Destroy(this.gameObject);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
	
}
