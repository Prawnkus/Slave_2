using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public GameObject Explosion;

    public float speed = 1;
    public float LifeTime = 5;
    public int damage = 2;

	// Use this for initialization
	void Start () {
        Destroy(gameObject,LifeTime);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
	}

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(Explosion,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
