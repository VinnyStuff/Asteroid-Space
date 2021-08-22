using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public GameRules gameRules;
    public Rigidbody rb;
    public bool canRotate;
    public float rotateForce;
    public GameObject[] rocketPeaces;
    public Player playerScript;
    public Camera playerCamera;
    public ParticleSystem explosion;
    public GameObject gameObjectsMoved;
    public bool playerIsDead;
    public Vector3 deathLocation;
    private int playerCurrentSkin;
    public GameObject jetParticlesReference;
    public GameObject[] jetParticles;
    public SongManagement songManagement;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canRotate = true;
        rotateForce = 1f;
        playerScript = GetComponent<Player>();
        playerIsDead = false;
        SkinManagement();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsDead == false)
        {
            PlayerMovement();
            JetParticlesUpdatePositionAndRotation();
        }
    }
    public void PlayerForwardMove()//for mobile
    {
        rb.velocity = new Vector3(3.5f, 5.5f, 0);
        rotateForce = -150f; //negative number
        songManagement.jetFire.Play();
        JetParticlesPlay();
        songManagement.ChangeTheGameplaySongWhenEnds();
    }
    void PlayerMovement()
    {
        if (transform.rotation.eulerAngles.z >= 115) //verify if can rotate
        {
            transform.rotation = Quaternion.Euler(0, 180, 115);
        }
        if (transform.rotation.eulerAngles.z <= 55)
        {
            transform.rotation = Quaternion.Euler(0, 180, 55);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) //for PC
        {
            rb.velocity = new Vector3(3.5f, 5.5f, 0);
            rotateForce = -150f; //negative number
            songManagement.jetFire.Play();
            JetParticlesPlay();
        }
        if (canRotate && gameRules.canPlay == true) //rotation
        {
            gameObject.transform.Rotate(0, 0, rotateForce * Time.deltaTime);
        }
        if (transform.rotation.eulerAngles.z >= 115) //verify if can rotate
        {
            rotateForce = 0;
        }
        else if (transform.rotation.eulerAngles.z <= 55)
        {
            rotateForce = 150;
        }
        if (transform.localPosition.y <= 0 || transform.localPosition.y >= 11) //if the player leaves the camera view his die
        {
            PlayerDeathAnimation();
        }
    }
    void PlayerDeathAnimation()
    {
        if (playerIsDead == false)
        {
            playerIsDead = true;

            for (int i = 0; i < rocketPeaces.Length; i++) //add a box collider and a rigidbody in the rocket peaces
            {
                rocketPeaces[i].GetComponent<BoxCollider>().enabled = true;
                rocketPeaces[i].AddComponent<Rigidbody>();
                rocketPeaces[i].GetComponent<Rigidbody>().useGravity = false;
            }
            deathLocation = gameObject.transform.position;
            playerCamera.GetComponent<SkyboxChanger>().CameraShake();
            explosion.Play();
            explosion.transform.parent = gameObjectsMoved.transform;
            songManagement.PlayExplosion();
            songManagement.PauseGameplaySong();
            playerScript.enabled = false; //disable the player script
        }
    }
    private void OnCollisionEnter(Collision other) //if the player collider a asteroid
    {
        Obstacle obstacle = other.transform.GetComponent<Obstacle>();
        if (obstacle)
        {
            PlayerDeathAnimation();
        }
    }
    public int jetParticlesNumber;
    void JetParticlesPlay()
    {
        for (int i = 0; i < jetParticles.Length; i++)
        {
            jetParticles[i].GetComponent<ParticleSystem>().Play();
        }
    }

    void JetParticlesUpdatePositionAndRotation()
    {
        for (int i = 0; i < jetParticles.Length; i++)
        {
            ParticleSystem ps = jetParticles[i].GetComponent<ParticleSystem>();
            var shape = ps.shape;
            shape.enabled = true;
            shape.rotation = new Vector3(-jetParticlesReference.transform.rotation.eulerAngles.z + 90, jetParticlesReference.transform.eulerAngles.y + 90, gameObject.transform.eulerAngles.z);
            shape.position = new Vector3(jetParticlesReference.transform.position.x, jetParticlesReference.transform.position.y, jetParticlesReference.transform.position.z);
        }
    }
    //SKINS
    public Material[] skinBody;
    public Material[] skinFins;
    public Material[] skinGlass;
    public Material[] skinMetal;
    public void SkinManagement()
    {
        if (gameRules.canPlay == false && gameRules.storeScene == false) // main menu scene
        {
            playerCurrentSkin = PlayerPrefs.GetInt("currentSkin", 0);
            ApplySkin(playerCurrentSkin);
        }
    }
    void ApplySkin(int skinIndex)
    {
        for (int i = 0; i < rocketPeaces.Length; i++)
        {
            if (rocketPeaces[i].GetComponent<Renderer>().material.name.Contains("Body"))
            {
                rocketPeaces[i].GetComponent<Renderer>().material = skinBody[skinIndex];
            }
            if (rocketPeaces[i].GetComponent<Renderer>().material.name.Contains("Fins"))
            {
                rocketPeaces[i].GetComponent<Renderer>().material = skinFins[skinIndex];
            }
            if (rocketPeaces[i].GetComponent<Renderer>().material.name.Contains("Metal"))
            {
                rocketPeaces[i].GetComponent<Renderer>().material = skinMetal[skinIndex];
            }
            if (rocketPeaces[i].GetComponent<Renderer>().material.name.Contains("Glass"))
            {
                rocketPeaces[i].GetComponent<Renderer>().material = skinGlass[skinIndex];
            }
        }
    }
}
