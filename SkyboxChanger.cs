using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public float rotateSpeed; //skybox spinning 
    private Player player;
    public Transform playerPosition;
    public Material[] skyboxAppearance;
    public int currentSkyboxAppearance;
    public int score;
    public Light universeLight;
    public Material backdrop;
    public float fade;
    public bool hasFlipped;
    public SongManagement songManagement;
    // Start is called before the first frame update
    void Start()
    {
        currentSkyboxAppearance = 0;
        fade = 5;
        score = 150;
        rotateSpeed = 3f;
        Color color = backdrop.color;
        color.a = 0;
        backdrop.color = color;
    }
    // Update is called once per frame
    void Update()
    {
        player = playerPosition.GetComponent<Player>(); //checking if the player is dead
        if (player.playerIsDead == false)
        {
            RotateBackground();
            ChangeBackground();
        }
    }
    void ChangeBackground()
    {
        if (playerPosition.position.x >= score - fade && playerPosition.position.x <= score)//Fade in
        {
            Color color = backdrop.color;
            color.a = 1 * ((playerPosition.position.x - (score - fade)) / (score - (score - fade)));
            backdrop.color = color;
        }
        if (playerPosition.position.x >= score && hasFlipped == false)
        {
            hasFlipped = true;
            currentSkyboxAppearance++;
            songManagement.PlayNextLevel();
            RenderSettings.skybox = skyboxAppearance[currentSkyboxAppearance % skyboxAppearance.Length];
        }
        if (playerPosition.position.x >= score && playerPosition.position.x <= score + fade)//Fade out
        {
            Color color = backdrop.color;
            color.a = 1 - (playerPosition.position.x - score) / ((score + fade) - score); //fade out
            backdrop.color = color;
        }
        if (playerPosition.position.x >= score + fade)
        {
            score = score + 150;
            hasFlipped = false;
        }
    }
    void RotateBackground()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
    public AnimationCurve curve;
    public float duration = 1f;
    public IEnumerator Shake()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strenght = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strenght;
            yield return null;
        }
        transform.position = startPosition;
    }
    public void CameraShake()
    {
        StartCoroutine(Shake());
    }
}
