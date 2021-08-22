using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Rigidbody rb;
    public float rotateForce;
    // Start is called before the first frame updateWWWW
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotateForce = Random.Range(0.1f, 0.5f);
        rb.angularVelocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * rotateForce; //spin slowly
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.z != 0)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0); //keep in the 0 z position
        }
    }
    private void OnCollisionEnter(Collision other) //explosion the asteroid
    {
        Player player = other.transform.GetComponent<Player>();
        if (player && player.playerIsDead == false)
        {
            GameObject asteroidPeaces = Instantiate(player.gameRules.asteroidPeaces, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), transform.rotation);
            player.gameRules.obstaclesSpawned.Add(asteroidPeaces);
            gameObject.SetActive(false);
        }
    }
}
