using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    //Mechanic's values set here.
    [SerializeField] private int startingBallCount;
    [SerializeField] private float waitTimeBeforeReset;
    [SerializeField] private float minPullAmt;
    [SerializeField] private float maxPullAmt;
    [SerializeField] private float maxK;
    [SerializeField] private float minK;
    [SerializeField] private Vector2 gridLowLimit;
    [SerializeField] private Vector2 gridHighLimit;

    //Reference to gameobjects in play/prefabs.
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject PlayerObj;
    [SerializeField] private GameObject projectileObj;
    [SerializeField] private GameObject gameOverTextObj;
    [SerializeField] private GameObject highScoreTextObj;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private Targets target;
    [SerializeField] private Transform targetParent;
    [SerializeField] private Transform ballSpawnPoint;

    //UI references in play.
    [SerializeField] private Slider kSlider;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI ballCountText;
    [SerializeField] private Button resetBtn;
    [SerializeField] private Button helpBtn;
    [SerializeField] private Button closeHelpBtn;

    //Bools that control the game flow.
    private bool isGameActive;
    private bool isGameInputReady;
    private bool isGameInputFirstTouch;
    private bool isGameHelpOver;

    //Vector variables that store touch/mouse locations
    private Vector3 firstTouch;
    private Vector3 secondTouch;

    //Constant K value that is used in Hooke's law.
    private float currentK;

    //Class variables used to modulate the work and divide responsibility to various classes.
    private LifeCounter lifeCounter;
    private ShooterPhy shooterPhy;
    private ScoreKeeper scoreKeeper;
    private LevelCreator levelCreator;
    private ActionHandler actionHandler;

    //references the coroutine, that way it can be stopped more smoothly.
    private Coroutine waitCoroutine;

    /// <summary>
    /// In this function I am initializing certain things that won't need to be reinitialized again.
    /// This includes 1) adding reference to restart level button call.
    /// 2) Initializing the level creator (since object pooling is done its only initialized once so as to not loose reference to the object pool stack)
    /// </summary>
    private void Start()
    {
        target.Init();

        resetBtn.onClick.AddListener(ResetLevel);
        helpBtn.onClick.AddListener(SeekHelp);
        closeHelpBtn.onClick.AddListener(CloseHelp);

        levelCreator = new LevelCreator(target, targetParent, gridLowLimit, gridHighLimit);
        
        InitLevel();
    }

    /// <summary>
    /// All helper classes are initialized, along with some variables and player object is set to its initial position.
    /// </summary>
    private void InitLevel()
    {
        gameOverTextObj.SetActive(false);
        highScoreTextObj.SetActive(false);
        targetParent.gameObject.SetActive(true);

        lifeCounter = new LifeCounter(startingBallCount, GameOver);
        scoreKeeper = new ScoreKeeper(0);
        shooterPhy = new ShooterPhy(minPullAmt, maxPullAmt, projectileObj, ballSpawnPoint);
        actionHandler = new ActionHandler(lifeCounter, shooterPhy, scoreKeeper, ballCountText, scoreText);

        ResetProjectile();
        
        levelCreator.CreateLevel();
        scoreText.text = scoreKeeper.UpdateScore(0).ToString();
        kSlider.onValueChanged.AddListener(SetSliderVal);
        SetSliderVal(kSlider.value);
        
        isGameActive = isGameInputFirstTouch = true;
    }

    /// <summary>
    /// Connects ingame slider values to currentK, lerping to make it corresspond to the min/max value set in inspector.
    /// </summary>
    /// <param name="val"></param>
    private void SetSliderVal(float val)
    {
        currentK = Mathf.Lerp(minK, maxK, val);
    }

    /// <summary>
    /// Input system is done here.
    /// At first, it checks if 'first touch' has been made, if not then waits for that. Once the first touch is confirmed, its pixel position is taken and converted into world position.
    /// Then it waits for the touch to become zero, (all the while rotating the nozzle towards the direction it will shoot). Once the touch is lost, the latest touch point is noted, its position
    /// calculated from the pixel position and this gives us the 2nd position.
    /// </summary>
    private void Update()
    {
        if(isGameActive && isGameInputReady)
        {
            if (isGameInputFirstTouch && (Input.touchCount > 0 || Input.GetMouseButton(0)))
            {
                firstTouch = Vector3.zero;
                if (Input.touchCount > 0)
                {
                    firstTouch = cam.ScreenToWorldPoint(Input.touches[0].position);
                    firstTouch.z = 0f;
                }

                if (Input.GetMouseButton(0))
                {
                    firstTouch = cam.ScreenToWorldPoint(Input.mousePosition);
                    firstTouch.z = 0f;
                }

                if(firstTouch.x < 2f)
                    isGameInputFirstTouch = false;
            }

            if (!isGameInputFirstTouch)
            {
                if(Input.touchCount > 0 || Input.GetMouseButton(0))
                {
                    secondTouch = Vector3.zero;
                    if (Input.touchCount > 0)
                    {
                        secondTouch = cam.ScreenToWorldPoint(Input.touches[0].position);
                        secondTouch.z = 0f;
                    }

                    if (Input.GetMouseButton(0))
                    {
                        secondTouch = cam.ScreenToWorldPoint(Input.mousePosition);
                        secondTouch.z = 0f;
                    }

                    PlayerObj.transform.up = (firstTouch - secondTouch).normalized;
                }
                else
                {
                    isGameInputReady = false;
                    isGameInputFirstTouch = true;
                    CalculateLaunch();
                }
            }
        }
    }

    /// <summary>
    /// Now that we have the two vector values needed to shoot the projectile. We can pass them to the helper function responsible to deal with with physics of ball.
    /// We also initiate a countdown reset, incase the ball does not leave the screen and enter the kill box.
    /// </summary>
    private void CalculateLaunch()
    {
        helpBtn.interactable = false;
        shooterPhy.Shoot(firstTouch, secondTouch, currentK);
        waitCoroutine = StartCoroutine(ResetPlayerInput());
    }

    /// <summary>
    /// Waits for a set time then calls the function that resets the projectile.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetPlayerInput()
    {
        yield return new WaitForSeconds(waitTimeBeforeReset);
        ResetProjectile();
    }

    /// <summary>
    /// A trigger enter function to see what the ball collided with.
    /// Border refers to the kill box in the surrounding the game.
    /// Ideally each object it collided with would have its own script all implementing an interface and using this interface we would perform the action needed.
    /// But since this assignment only allows 1 monobehavior to be present, I worked around this by using the tag of the objects as a marker, and performing action corressponding to it
    /// in the helper function that deals with all consequences of hitting a target piece (it can be + points, - points, + health, etc)
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Border")
        {
            ResetProjectile();
        }

        if(other.tag.Contains("Target"))
        {
            var t = target.Dict[other.tag];
            actionHandler.PerformAction(t);
            if (t.particleEffect != null)
                Instantiate(t.particleEffect, other.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Resets the balls location to intial position and at rest.
    /// </summary>
    private void ResetProjectile()
    {
        helpBtn.interactable = true;
        if(waitCoroutine != null)
            StopCoroutine(waitCoroutine);
        ballCountText.text = lifeCounter.UpdateBallCounter(-1).ToString();
        shooterPhy.ResetPlayer();
        isGameInputReady = true;
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    private void GameOver()
    {
        isGameActive = false;
        isGameInputReady = true;
        targetParent.gameObject.SetActive(false);
        gameOverTextObj.SetActive(true);
        highScoreTextObj.SetActive(scoreKeeper.CheckHighScore());
    }

    /// <summary>
    /// Restarts the game.
    /// </summary>
    private void ResetLevel()
    {
        levelCreator.ResetLevel();
        kSlider.onValueChanged.RemoveAllListeners();
        InitLevel();
    }

    /// <summary>
    /// Opens Help Page.
    /// </summary>
    private void SeekHelp()
    {
        if(isGameActive)
        {
            isGameHelpOver = false;
            isGameInputFirstTouch = true;
        }
        else
        {
            isGameHelpOver = true;
        }

        isGameActive = false;
        PlayerObj.SetActive(false);
        targetParent.gameObject.SetActive(false);
        gameplayPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    /// <summary>
    /// Closes Help page.
    /// </summary>
    private void CloseHelp()
    {
        if(!isGameHelpOver)
        {
            isGameActive = true;
            targetParent.gameObject.SetActive(true);
        }

        PlayerObj.SetActive(true);
        gameplayPanel.SetActive(true);
        helpPanel.SetActive(false);
    }

    /// <summary>
    /// Removes the listener on the slider event.
    /// </summary>
    private void OnDestroy()
    {
        kSlider.onValueChanged.RemoveAllListeners();
    }
}