using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class GameRules : MonoBehaviour
{
    private GameObject playerSecondLife; //use if the player watch de ads to continue with the same distance
    public GameObject[] obstacles;
    public GameObject player;
    public List<GameObject> obstaclesSpawned;
    private int currentObstacle;
    private float yLocationObstacle;
    private float yChoice;
    public int currentPositionObstacle;
    private int emptinessBetweenObstacles;
    public Text distanceText;
    public GameObject deathText;
    public Text gameName;
    public bool playerIsDead;
    public Camera playerCamera;
    public GameObject asteroidPeaces;
    public bool secondLife;
    public bool canPlay; //if it's not on the main menu
    public Button MainMenuButton;
    public Button SecondChanceButton;
    public Button store;
    public Button pause;
    public Button tapToPlay;
    public Button PlayerForwardMoveButton;
    public GameObject paused;
    public Button restart;
    public Restart restartScript;
    public Text personalRecord;
    public Text personalRecordPaused;
    public Text newRecord;
    public SongManagement songManagement;
    public AdsManager adsManager;
    // Start is called before the first frame update
    void Start()
    {
        MainMenuScene();
        adsManager.InterstitialLoadAd();
    }
    // Update is called once per frame
    void Update()
    {
        playerIsDead = player.GetComponent<Player>().playerIsDead; //checking if the player is dead
        if (canPlay)
        {
            if (playerIsDead == false)
            {
                if (player.transform.position.x > currentPositionObstacle - emptinessBetweenObstacles * 2)
                {
                    SpawnTheObstacles();
                }
                DestroyTheObstacles();
                CameraFollowThePlayer();
            }
            else
            {
                if (deathText.gameObject.activeSelf == false)
                {
                    DeathScene();
                }
                if (adsManager.AdsReady() == false)//ADS check
                {
                    Color color = Color.white;
                    color.a = 0.18f;
                    SecondChanceButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = color;
                }
                else if (adsManager.AdsReady() == true)
                {
                    Color color = Color.white;
                    color.a = 1f;
                    SecondChanceButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = color;
                }
                if (canContinue == true) // ADS 
                {
                    songManagement.PlayButtonPressed();
                    songManagement.PauseGameplaySong();
                    Destroy(player);
                    playerSecondLife.transform.position = player.GetComponent<Player>().deathLocation;
                    playerSecondLife.SetActive(true);
                    player = playerSecondLife;
                    playerCamera.GetComponent<SkyboxChanger>().playerPosition = playerSecondLife.transform;
                    playerSecondLife.GetComponent<Rigidbody>().useGravity = false;
                    playerSecondLife.GetComponent<Player>().enabled = false;
                    secondLife = true;
                    deathText.SetActive(false);
                    //adapting the scene
                    player.transform.position = new Vector3(player.transform.position.x, 5.5f, player.transform.position.z); //spawn in in the middle of the screen
                    for (int i = 0; i < obstaclesSpawned.Count; i++)
                    {
                        Destroy(obstaclesSpawned[i]);
                    }
                    obstaclesSpawned.Clear();
                    CleanHUD();
                    distanceText.gameObject.SetActive(true);
                    newRecord.gameObject.SetActive(false);
                    PlayerForwardMoveButton.gameObject.SetActive(true);
                    pause.gameObject.SetActive(true);
                    canContinue = false;
                    //adsManager.InterstitialLoadAd();
                }
            }   
            SecondLife();
            Distance();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && tapToPlay.enabled == true)//for pc
        {
            TapToPlayButton();//fake pressed button
        }
        else if (restartScript != null)
        {
            CleanHUD();
        }
        StoreManagement();
        PersonalRecord();
        PopUpSkinUnlocked();
    }
    private void FixedUpdate()//FUNCIONA POR GAMBIARRA
    {
        if (restartScript != null)//restart management ESSA MERDA FICA DANDO ERRO NO UPDATE, POR QUE CHAMA MUITO RAPIDO AAAAAA
        {
            if (restartScript.restarting == true)
            {
                TapToPlayButton();//fake pressed button
                RandomAds();
                Destroy(restartScript.gameObject);
            }
        }
        if (playerIsDead == false)
        {
            songManagement.ChangeTheGameplaySongWhenEnds();
        }
    }
    void RandomAds()
    {
        if (adsManager.InterstitialAdsReady() == true)
        {
            int randomAds = Random.Range(1, 101);
            if (randomAds <= 25)
            {
                PauseButton();
                adsManager.PlayInterstitialAd();
            }
        }
    }
    void CameraFollowThePlayer()
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z);
    }
    void SpawnTheObstacles() //spawn the obstacles infinitely
    {
        for (int i = 0; i < 2; i++)
        {
            yLocationObstacle = Random.Range(0, 3);
            if (yLocationObstacle == yChoice)
            {
                while (true)
                {
                    yLocationObstacle = Random.Range(0, 3);
                    if (yLocationObstacle != yChoice)
                    {
                        break;
                    }
                }
            }
            yChoice = yLocationObstacle;

            if (yLocationObstacle == 0)
            {
                yLocationObstacle = 1.5f;
            }
            if (yLocationObstacle == 1)
            {
                yLocationObstacle = 5.5f;
            }
            if (yLocationObstacle == 2)
            {
                yLocationObstacle = 9.5f;
            }
            currentObstacle = Random.Range(0, 21);
            GameObject obstacleClone = Instantiate(obstacles[currentObstacle], new Vector3(currentPositionObstacle, yLocationObstacle, 0), transform.rotation);
            obstaclesSpawned.Add(obstacleClone);
        }
        currentPositionObstacle = currentPositionObstacle + emptinessBetweenObstacles;
    }
    void DestroyTheObstacles()// destroy obstacles that have been passed
    {
        for (int i = 0; i < obstaclesSpawned.Count; i++)
        {
            if (player.transform.position.x - emptinessBetweenObstacles * 2 >= obstaclesSpawned[i].transform.position.x)
            {
                Destroy(obstaclesSpawned[i]);
                obstaclesSpawned.Remove(obstaclesSpawned[i]);
            }
        }
    }
    void GameStart()//When does the game begin
    {
        if (canPlay)
        {
            secondLife = false;
            playerSecondLife = Instantiate(player, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
            playerSecondLife.SetActive(false);
            emptinessBetweenObstacles = 8;
            currentPositionObstacle = emptinessBetweenObstacles;
            SpawnTheObstacles();
        }
    }
    void PersonalRecord()
    {
        personalRecord.text = "HIGSCORE: " + PlayerPrefs.GetFloat("personalRecord", 0).ToString("F2") + "km";
        if (playerIsDead == true)
        {
            if (PlayerPrefs.GetFloat("personalRecord", 0) == 0)
            {
                PlayerPrefs.SetFloat("personalRecord", player.GetComponent<Player>().deathLocation.x);
            }
            if (player.GetComponent<Player>().deathLocation.x > PlayerPrefs.GetFloat("personalRecord", 0))
            {
                if (newRecord.gameObject.activeSelf == false)
                {
                    songManagement.PlayRecord();
                    newRecord.gameObject.SetActive(true);
                }
                PlayerPrefs.SetFloat("personalRecord", player.GetComponent<Player>().deathLocation.x);
            }
        }
    }
    void Distance() //player score
    {
        if (playerIsDead == true && player.GetComponent<Player>().deathLocation.x > 0)
        {
            distanceText.text = player.GetComponent<Player>().deathLocation.x.ToString("F2") + "km";
        }
        else if (player.transform.position.x > 0 && playerIsDead == false)
        {
            distanceText.text = player.transform.position.x.ToString("F2") + "km";
        }
    }
    public void TapToPlayButton()//if player leaves the main menu and go to gameplay = press the screen = for mobile
    {
        adsManager.LoadAd();
        adsManager.InterstitialLoadAd();
        songManagement.PlayGameplayMusic(true);
        GameplayScene();
        GameStart();
    }
    public void BackToMainMenuButton()
    {
        songManagement.PlayButtonPressed();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Gameplay");
    }
    public bool canContinue = false;
    public void SecondLifeButton() //if the player watch de ads to continue with the same distance
    {
        if (player.GetComponent<Player>().playerIsDead == true && secondLife == false && adsManager.AdsReady() == true)
        {
            adsManager.PlayRewardedAd();
        }
        else
        {
            songManagement.PlayButtonPressedError();
        }
    }
    public void PauseButton() //TODO arrumar algo para quando presionar o butão funcionar apenas ele
    {
        songManagement.PlayButtonPressed();
        PauseScene();
    }
    void SecondLife() //if player watched the ads
    {
        if (secondLife == true && Input.GetKeyDown(KeyCode.UpArrow) && playerSecondLife.GetComponent<Player>().playerIsDead == false)//for pc
        {
            playerSecondLife.GetComponent<Player>().enabled = true;
            playerSecondLife.GetComponent<Rigidbody>().useGravity = true;
        }
        if (secondLife == true && playerSecondLife.GetComponent<Player>().playerIsDead == true) //for pc and mobie
        {
            Color color = Color.white;
            color.a = 0.18f;
            SecondChanceButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = color;
        }
    }
    public void SecondLifeMobile()//for mobile = is a button
    {
        if (secondLife == true && playerSecondLife.GetComponent<Player>().playerIsDead == false && playerSecondLife.GetComponent<Player>().enabled == false)
        {
            playerSecondLife.GetComponent<Player>().enabled = true;
            playerSecondLife.GetComponent<Rigidbody>().useGravity = true;
            GameplayScene();
            PlayerForwardMoveButton.onClick.AddListener(player.GetComponent<Player>().PlayerForwardMove);//add a button action again , because the player original is deleted
            Debug.Log("Add new button function"); //PERGUNTAR PARA O VICTOR DEPOIS;
        }
    }
    void CleanHUD()
    {
        deathText.gameObject.SetActive(false);
        tapToPlay.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(false);
        gameName.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
        SecondChanceButton.gameObject.SetActive(false);
        store.gameObject.SetActive(false);
        pause.gameObject.SetActive(false);
        paused.gameObject.SetActive(false);
        PlayerForwardMoveButton.gameObject.SetActive(false);
        rightButtonStore.gameObject.SetActive(false);
        leftButtonStore.gameObject.SetActive(false);
        selectThis.gameObject.SetActive(false);
        skinName.gameObject.SetActive(false);
        restart.gameObject.SetActive(false);
        personalRecord.gameObject.SetActive(false);
        popUpGameplay.gameObject.SetActive(false);
        popUpStore.gameObject.SetActive(false);
        soundEffectsText.SetActive(false);
        musicText.SetActive(false);
        soundEffectsButton.gameObject.SetActive(false);
        musicButton.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
        leaveTheStore.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
        soundEffectsCredits.gameObject.SetActive(false);
        backInCredits.gameObject.SetActive(false);
    }
    void MainMenuScene()
    {
        CleanHUD();
        AppylingVolume();
        tapToPlay.gameObject.SetActive(true);
        gameName.gameObject.SetActive(true);
        store.gameObject.SetActive(true);
        player.SetActive(true);
        personalRecord.gameObject.SetActive(true);
        settings.gameObject.SetActive(true);
        newRecord.gameObject.SetActive(false);
    }
    public void GameplayScene()
    {
        CleanHUD();
        player.GetComponent<Player>().rb.velocity = new Vector3(3.5f, 5.5f, 0);
        player.GetComponent<Rigidbody>().useGravity = true;
        canPlay = true;
        pause.gameObject.SetActive(true);
        distanceText.gameObject.SetActive(true);
        PlayerForwardMoveButton.gameObject.SetActive(true);
        newRecord.gameObject.SetActive(false);
        popUpGameplay.gameObject.SetActive(true);
        LightsEnable(true);
    }
    void DeathScene()
    {
        CleanHUD();
        deathText.gameObject.SetActive(true);
        MainMenuButton.gameObject.SetActive(true);
        SecondChanceButton.gameObject.SetActive(true);
        restart.gameObject.SetActive(true);
        distanceText.gameObject.SetActive(true);
        personalRecord.gameObject.SetActive(true);
        personalRecordPaused.text = "HIGSCORE: " + PlayerPrefs.GetFloat("personalRecord", 0).ToString("F2") + "km";
    }
    void PauseScene()
    {
        personalRecordPaused.text = "HIGSCORE: " + PlayerPrefs.GetFloat("personalRecord", 0).ToString("F2") + "km";
        songManagement.PauseGameplaySong();
        newRecord.gameObject.SetActive(false);
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            paused.gameObject.SetActive(false);
            pause.gameObject.SetActive(true);
        }
        else
        {
            pause.gameObject.SetActive(false);
            paused.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
    //STORE 
    public GameObject[] skins;
    public List<GameObject> skinsInstantiated;
    public bool storeScene;
    public Button leaveTheStore;
    public Button rightButtonStore;
    public Button leftButtonStore;
    public Button selectThis;
    public Text unlocked; //select this text
    public Image locked; //padlock
    public int currentSkin;
    public Text skinName;
    public Text popUpGameplay;
    public Text popUpStore;
    void StoreScene()
    {
        CleanHUD();
        storeScene = true;
        player.SetActive(false);
        leaveTheStore.gameObject.SetActive(true);
        rightButtonStore.gameObject.SetActive(true);
        leftButtonStore.gameObject.SetActive(true);
        selectThis.gameObject.SetActive(true);
        skinName.gameObject.SetActive(true);
        newRecord.gameObject.SetActive(false);
        popUpStore.gameObject.SetActive(true);
        LightsEnable(false);
        for (int i = 0; i < skins.Length; i++)
        {
            GameObject skinStore = Instantiate(skins[i], new Vector3(-10, 5, 0), transform.rotation);
            skinStore.SetActive(false);
            skinsInstantiated.Add(skinStore);
        }
        currentSkin = 0;
        skinsInstantiated[currentSkin].SetActive(true);
    }
    public void StoreButton() //Store button
    {
        songManagement.PlayButtonPressed();
        StoreScene();
    }
    public bool completeTheAdsForSkin;//FOR SKIN with ADS
    void StoreManagement()
    {
        if (storeScene == true)//if the player is in the store
        {
            if (adsManager.AdsReady() == false)
            {
                adsManager.LoadAd();
            }
            for (int i = 0; i < skinsInstantiated.Count; i++)
            {
                skinsInstantiated[i].transform.Rotate(0, 60 * Time.deltaTime, 0);
            }
            if (skinsInstantiated[0].activeSelf)//name skin
            {
                skinName.text = "Default";
                UnlockedSkin();
            }
            if (skinsInstantiated[1].activeSelf)//ADS
            {
                skinName.text = "Blue";
                if (completeTheAdsForSkin == true)
                {
                    PlayerPrefs.SetString("AdsSkin", "Unlocked");
                    PlayerPrefs.Save();
                }
                if (PlayerPrefs.GetString("AdsSkin") == "Unlocked")
                {
                    UnlockedSkin();
                    popUpStore.text = "";
                }
                else
                {
                    popUpStore.text = "WATCH AN AD TO UNLOCK";
                    LockedSkin();
                }
            }
            if (skinsInstantiated[2].activeSelf)//500km
            {
                skinName.text = "Purple";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 500)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
            if (skinsInstantiated[3].activeSelf)//1000km
            {
                skinName.text = "Star light";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 1000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
            if (skinsInstantiated[4].activeSelf)//2000km
            {
                skinName.text = "Dead space";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 2000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
            if (skinsInstantiated[5].activeSelf)//3000km
            {
                skinName.text = "Death ship";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 3000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
            if (skinsInstantiated[6].activeSelf)//4000km
            {
                skinName.text = "Pratiadão";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 4000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
            if (skinsInstantiated[7].activeSelf)//5000km
            {
                skinName.text = "Gold 24k";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 5000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
            if (skinsInstantiated[8].activeSelf)//6000km
            {
                skinName.text = "Sumidão";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 6000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }            
            if (skinsInstantiated[9].activeSelf)//7000km
            {
                skinName.text = "Pinkip";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 7000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }            
            if (skinsInstantiated[10].activeSelf)//8000km
            {
                skinName.text = "Azulão";
                if (PlayerPrefs.GetFloat("personalRecord", 0) >= 8000)
                {
                    UnlockedSkin();
                }
                else
                {
                    LockedSkin();
                }
            }
        }
    }
    public void PopUpSkinUnlocked()
    {
        if (player.transform.position.x >= 500 && player.transform.position.x <= 515 && PlayerPrefs.GetFloat("personalRecord", 0) <= 500)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: PURPLE";
            }
        }
        else if (player.transform.position.x >= 1000 && player.transform.position.x <= 1015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 1000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: STAR LIGHT";
            }
        }
        else if (player.transform.position.x >= 2000 && player.transform.position.x <= 2015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 2000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: DEAD SPACE";
            }
        }
        else if (player.transform.position.x >= 3000 && player.transform.position.x <= 3015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 3000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: DEATH SHIP";
            }
        }
        else if (player.transform.position.x >= 4000 && player.transform.position.x <= 4015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 4000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: PRATIADÃO";
            }
        }
        else if (player.transform.position.x >= 5000 && player.transform.position.x <= 5015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 5000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: GOLD 24K";
            }
        }
        else if (player.transform.position.x >= 6000 && player.transform.position.x <= 6015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 6000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: SUMIDÃO";
            }
        }        
        else if (player.transform.position.x >= 7000 && player.transform.position.x <= 7015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 7000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: Pinkip";
            }
        }        
        else if (player.transform.position.x >= 8000 && player.transform.position.x <= 8015 && PlayerPrefs.GetFloat("personalRecord", 0) <= 8000)
        {
            if (popUpGameplay.text == "")
            {
                songManagement.PlayUnlockedSkin();
                popUpGameplay.text = "UNLOCKED SKIN: Azulão";
            }
        }
        else
        {
            popUpGameplay.text = "";
        }
    }
    public void RightButton()
    {
        songManagement.PlayButtonPressed();
        popUpStore.text = null;
        if (skinsInstantiated[skinsInstantiated.Count - 1].activeSelf)//don't leave the array
        {
            skinsInstantiated[currentSkin].SetActive(false);
            currentSkin = 0;
            skinsInstantiated[0].SetActive(true);
        }
        else
        {
            skinsInstantiated[currentSkin].SetActive(false);
            currentSkin = currentSkin + 1;
            skinsInstantiated[currentSkin].SetActive(true);
        }
    }
    public void LeftButton()
    {
        songManagement.PlayButtonPressed();
        popUpStore.text = null;
        if (skinsInstantiated[0].activeSelf)//don't leave the array
        {
            skinsInstantiated[currentSkin].SetActive(false);
            currentSkin = skinsInstantiated.Count - 1;
            skinsInstantiated[skinsInstantiated.Count - 1].SetActive(true);
        }
        else
        {
            skinsInstantiated[currentSkin].SetActive(false);
            currentSkin = currentSkin - 1;
            skinsInstantiated[currentSkin].SetActive(true);
        }
    }
    public void SelectThisSkin()
    {
        if (locked.gameObject.activeSelf)
        {
            songManagement.PlayButtonPressedError();
            if (skinsInstantiated[1].activeSelf)//ADS
            {
                if (PlayerPrefs.GetString("AdsSkin") != "Unlocked")
                {
                    adsManager.PlayRewardedAd();
                }
            }
            else if (skinsInstantiated[2].activeSelf)//500km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>500km</b>";
            }
            else if (skinsInstantiated[3].activeSelf)//1000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>1000km</b>";
            }
            else if (skinsInstantiated[4].activeSelf)//2000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>2000km</b>";
            }
            else if (skinsInstantiated[5].activeSelf)//3000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>3000km</b>";
            }
            else if (skinsInstantiated[6].activeSelf)//4000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>4000km</b>";
            }
            else if (skinsInstantiated[7].activeSelf)//5000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>5000km</b>";
            }
            else if (skinsInstantiated[8].activeSelf)//6000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>6000km</b>";
            }            
            else if (skinsInstantiated[9].activeSelf)//7000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>7000km</b>";
            }            
            else if (skinsInstantiated[10].activeSelf)//8000km
            {
                popUpStore.text = "HIGHSCORE REQUIRED: <b>8000km</b>";
            }
            else
            {
                popUpStore.text = null;
            }
        }
        if (unlocked.gameObject.activeSelf)
        {
            songManagement.PlayButtonPressed();
            PlayerPrefs.SetInt("currentSkin", currentSkin);
            BackToMainMenuButton();
        }
    }
    public void UnlockedSkin()
    {
        unlocked.gameObject.SetActive(true);
        locked.gameObject.SetActive(false);
    }
    public void LockedSkin()
    {
        unlocked.gameObject.SetActive(false);
        locked.gameObject.SetActive(true);
    }
    //settings scene
    public GameObject soundEffectsText;
    public GameObject musicText;
    public Button soundEffectsButton;
    public Button musicButton;
    public Text soundEffectsButtonText;
    public Text musicButtonText;
    public Button settings;
    public void SettingsScene()
    {
        CleanHUD();
        soundEffectsText.SetActive(true);
        musicText.SetActive(true);
        soundEffectsButton.gameObject.SetActive(true);
        musicButton.gameObject.SetActive(true);
        leaveTheStore.gameObject.SetActive(true);
        player.SetActive(false);
        creditsButton.gameObject.SetActive(true);
        newRecord.gameObject.SetActive(false);
        if (PlayerPrefs.GetString("Music") == "")
        {
            PlayerPrefs.SetString("Music", "ON");
            musicButtonText.text = PlayerPrefs.GetString("Music");
        }
        else
        {
            musicButtonText.text = PlayerPrefs.GetString("Music");
        }
        if (PlayerPrefs.GetString("SoundsEffects") == "")
        {
            PlayerPrefs.SetString("SoundsEffects", "ON");
            soundEffectsButtonText.text = PlayerPrefs.GetString("SoundsEffects");
        }
        else
        {
            soundEffectsButtonText.text = PlayerPrefs.GetString("SoundsEffects");
        }
    }
    public void SettingsButton() //Store button
    {
        songManagement.PlayButtonPressed();
        SettingsScene();
    }
    public void MusicSettingsButton()
    {
        songManagement.PlayButtonPressed();
        if (PlayerPrefs.GetString("Music") == "ON") //turn OFF
        {
            PlayerPrefs.SetString("Music", "OFF");
            musicButtonText.text = PlayerPrefs.GetString("Music");
            AppylingVolume();
        }
        else if (PlayerPrefs.GetString("Music") == "OFF")//turn ON
        {
            PlayerPrefs.SetString("Music", "ON");
            musicButtonText.text = PlayerPrefs.GetString("Music");
            AppylingVolume();
        }
        PlayerPrefs.Save();
    }
    public void SoundEffectsSettingsButton()
    {
        songManagement.PlayButtonPressed();
        if (PlayerPrefs.GetString("SoundsEffects") == "ON")//turn OFF
        {
            PlayerPrefs.SetString("SoundsEffects", "OFF");
            soundEffectsButtonText.text = PlayerPrefs.GetString("SoundsEffects");
            AppylingVolume(); 
        }
        else if (PlayerPrefs.GetString("SoundsEffects") == "OFF")//turn ON
        {
            PlayerPrefs.SetString("SoundsEffects", "ON");
            soundEffectsButtonText.text = PlayerPrefs.GetString("SoundsEffects");
            AppylingVolume();
        }
        PlayerPrefs.Save();
    }
    public void AppylingVolume()
    {
        if (PlayerPrefs.GetString("SoundsEffects") == "ON")
        {
            songManagement.MuteSoundsEffects(false);
        }
        else if (PlayerPrefs.GetString("SoundsEffects") == "OFF")
        {
            songManagement.MuteSoundsEffects(true);
        }
        else if (PlayerPrefs.GetString("SoundsEffects") == "")
        {
            songManagement.MuteSoundsEffects(false);
        }

        if (PlayerPrefs.GetString("Music") == "ON")
        {
            songManagement.MuteMusic(false);
        }
        else if (PlayerPrefs.GetString("Music") == "OFF")//turn ON
        {
            songManagement.MuteMusic(true);
        }
        else if (PlayerPrefs.GetString("Music") == "")
        {
            songManagement.MuteMusic(false);
        }
    }
    //CREDITs
    public Button creditsButton;
    public GameObject soundEffectsCredits;
    public Button backInCredits;

    public void CreditsScene()
    {
        CleanHUD();
        backInCredits.gameObject.SetActive(true);
        soundEffectsCredits.gameObject.SetActive(true);
    }
    public void CreditsButton()
    {
        songManagement.PlayButtonPressed();
        CreditsScene();
    }
    public void BackInCredits()
    {
        songManagement.PlayButtonPressed();
        SettingsButton();
    }
    public void CreditsURLRedirect()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "www.zapsplat.com")
        {
            Application.OpenURL("http://www.zapsplat.com");
        }        
        if (EventSystem.current.currentSelectedGameObject.name == "www.mixkit.co")
        {
            Application.OpenURL("https://mixkit.co");
        }
        if (EventSystem.current.currentSelectedGameObject.name == "www.bensound.com")
        {
            Application.OpenURL("https://www.bensound.com");
        }
        if (EventSystem.current.currentSelectedGameObject.name == "www.pixabay.com")
        {
            Application.OpenURL("https://pixabay.com");
        }       
        
        if (EventSystem.current.currentSelectedGameObject.name == "Axinova")
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/40484");
        }               
        if (EventSystem.current.currentSelectedGameObject.name == "Works For Fun")
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/16526");
        }        
        if (EventSystem.current.currentSelectedGameObject.name == "VinnyStuff")
        {
            Application.OpenURL("https://www.vinnystuff.com/?utm_source=asteroidspace&utm_medium=android&utm_campaign=credits&utm_content=credits_button");
        }
    }
    public Light[] lights;
    public void LightsEnable(bool enabled)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].gameObject.SetActive(enabled);
        }
    }
}
