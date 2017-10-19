using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEngine.Advertisements;

public class SnakeMovement : MonoBehaviour
{
    //	##### added by Yue Chen #####
	private int moveWay ;    // It determines how to control the movement of snake, gained from initial interface
	private int skinID ;     // It determines the skin of the snake, gained from initial interface
	private string nickName;

    public List<Transform> bodyParts = new List<Transform>();   // Records the location information of body parts of the snake
    public List<GameObject> Robots = new List<GameObject>();    // Records the information of Robots
    
    private float snakeWalkSpeed = 3.5f; // Called in SnakeMove()
    private float snakeRunSpeed = 7.0f;  // Called in SnakeRun()
    private bool isRunning; // Called in SnakeRun()
    //public float runBodyPartSmoothTime = 0.1f; // // Called in SnakeRun()
   // private float cameraSmoothTime = 0.13f;  // Called in CameraFollowSnake()

	public float runBodyPartSmoothTime = 0.1f; // // Called in SnakeRun()
	private float cameraSmoothTime = 0.13f;  // Called in CameraFollowSnake()

    public Transform addBodyPart;   // Called in OnCollisionEnter(), it is the thing added behind the snake after eating food

    //Called in SizeUp(), determine after eating how much food the snake will grow its size
    private int[] sizeUpArray = {2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 5096, 10192, 20348, 999999};
    //Called in OnCollisionEnter(), determine after eating how many food points will the snake add a body part
    private int[] foodUpArray = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 6, 10 };
    public int foodUpCounter=0;
    public int foodCounter;    // Called in OnCollisionEnter(), the number of food the snake eats so far
    public int curSizeLevel;    // Called in OnCollisionEnter()
    public Vector3 curSize = Vector3.one;  //Called in SizeUp()
    private float growRate = 0.1f;   //Called in OnCollisionEnter(), how much to grow snake size
    private float cameraGrowRate = 0.03f;    // Called in OnCollisionEnter(), when snake gets larger, camera size gets larger as well
    private float bodyPartSmoothTime = 0.2f; //Called in OnCollisionEnter(), the same value as in SnakeBodyActions.cs
    private float bodySmoothTime; // Called in SetBodySizeAndSmoothTime()

    //	##### added by Yue Chen #####
    public Text countText;
    public int length;
	
    // use this for initialization
    void Start() {
        GenerateFoodBeforeBegin();
        GenerateStoneBeforeBegin();
        GenerateRobotBeforeBegin();
		//	##### added by Yue Chen #####
		moveWay = PlayerPrefs.GetInt("moveWayID",1);    // It determines how to control the movement of snake, gained from initial interface
		// It determines the skin of the snake, gained from initial interface
		skinID = PlayerPrefs.GetInt("skinID",1);
		nickName = PlayerPrefs.GetString("nickname","");
    }
	
    // update is called once per frame
    void Update()
    {
        ChooseControlMethod(moveWay);
		ColorSnake(skinID);
        GenerateFoodAndItem();
        SnakeRun();
		SetScore (length);
    }

    void FixedUpdate()
    {
        SnakeMove();
        SetBodySizeAndSmoothTime();
        CameraFollowSnake();
        SnakeGlowing(isRunning);
        SnakeMoveAdjust();
        //	##### added by Yue Chen #####
        countText.text = "G o o d  j o b  !  " + nickName + "\nY o u r  L e n g t h  :  " + length;
    }

    /* When the head encounters an object, figure out what to do*/
    void OnCollisionEnter(Collision obj)
    {
        if (obj.transform.tag == "Food")
        {
            length++;
            Destroy(obj.gameObject);
            curAmountOfFood--;
            if (foodUpCounter + 1 == foodUpArray[curSizeLevel])
            {
                if (SizeUp(foodCounter) == false)
                {
                    foodCounter++;
                    // The contents in 'if' shouldn't be exectued in logic as we always have several body parts
                    Vector3 currentPos;
                    if (bodyParts.Count == 0)
                    {
                        currentPos = transform.position;
                    }
                    else
                    {
                        currentPos = bodyParts[bodyParts.Count - 1].position;
                    }
                    Transform newPart = Instantiate(addBodyPart, currentPos, Quaternion.identity) as Transform;
                    newPart.parent = GameObject.Find("SnakeBodies").transform;
                    bodyParts.Add(newPart);
                }
                else
                {
                    curSize += Vector3.one * growRate;
                    bodyPartSmoothTime += 0.01f;
                    transform.localScale = curSize;
                    // Scale up camera
                    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                    camera.GetComponent<Camera>().orthographicSize += camera.GetComponent<Camera>().orthographicSize * cameraGrowRate;
                }
                foodUpCounter = 0;
            }
            else
            {
                foodUpCounter++;
            }
        }
        //	##### added by Morgan #####
        else if (obj.transform.tag == "Item")
        {
            if (obj.transform.GetComponent<ParticleSystem>().startColor == new Color32(255, 0, 255, 255))
            {
                Destroy(obj.gameObject);
                curAmountOfItem--;
                snakeWalkSpeed += 3.5f;
                StartCoroutine("speedUpTime");
            }
            if (obj.transform.GetComponent<ParticleSystem>().startColor == new Color32(0, 255, 0, 255))
            {
                Destroy(obj.gameObject);
                curAmountOfItem--;
                if (bodyParts.Count > 4)
                { 
                    isRunning = true;
                    StartCoroutine("punishTime");
                }
            }
        }
        else if (obj.transform.tag == "Boundary")
        {
            while (bodyParts.Count > 0)
            {
                int lastIndex = bodyParts.Count - 1;
                Transform lastBodyPart = bodyParts[lastIndex].transform;
                bodyParts.RemoveAt(lastIndex);
                GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], lastBodyPart.position, Quaternion.identity) as GameObject;
                newFood.transform.parent = GameObject.Find("Foods").transform;
                Destroy(lastBodyPart.gameObject);
            }
            GameObject head = GameObject.FindGameObjectWithTag("Player");
            Destroy(head);
            //	##### added by Yue Chen #####
            if (PlayerPrefs.GetInt("removeAds", 0) == 0)
            {
                ShowAd();
            }
            SceneManager.LoadScene("Menu");

        }
        else if ((obj.transform.tag == "Body") || (obj.transform.tag == "Robot"))
        {
            while (bodyParts.Count > 0)
            {
                int lastIndex = bodyParts.Count - 1;
                Transform lastBodyPart = bodyParts[lastIndex].transform;
                bodyParts.RemoveAt(lastIndex);
                GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], lastBodyPart.position, Quaternion.identity) as GameObject;
                newFood.transform.parent = GameObject.Find("Foods").transform;
                Destroy(lastBodyPart.gameObject);
            }
            GameObject head = GameObject.FindGameObjectWithTag("Player");

            Destroy(head);
            //	##### added by Yue Chen #####

            if (PlayerPrefs.GetInt("removeAds", 0) == 0)
            {
                ShowAd();
            }
            SceneManager.LoadScene("Menu");
        }
        else if (obj.transform.tag == "Snake")
        {
            Vector3 temp = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1);
            gameObject.transform.position = temp;
        }
    }

    //	##### added by Morgan #####
    IEnumerator speedUpTime()
    {
        yield return new WaitForSeconds(2);
        snakeWalkSpeed -= 3.5f;
    }
    IEnumerator punishTime()
    {
        yield return new WaitForSeconds(2);
        isRunning = false;
		snakeWalkSpeed = 3.5f;
     
    }


    /* When losing body parts, snake size down*/

    void SnakeScaleChange() {
        if (curSizeLevel > 0 && foodCounter <= sizeUpArray[curSizeLevel - 1]) {
            curSizeLevel--;
            curSize -= Vector3.one * growRate;
            bodyPartSmoothTime -= 0.01f;
            transform.localScale = curSize;
            // Scale down camera
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<Camera>().orthographicSize -= camera.GetComponent<Camera>().orthographicSize * cameraGrowRate;
        }
    }

    /* Figure out whether snake size increases after eating*/
    bool SizeUp(int x)
    {
        if (x == sizeUpArray[curSizeLevel]) {
            curSizeLevel++;
            return true;
        }
        return false;
    }

    /* Set the size and smooth time of snake body parts every frame*/
    void SetBodySizeAndSmoothTime()
    {
        transform.localScale = curSize;
        if (snakeWalkSpeed >= snakeRunSpeed)
        {
            bodySmoothTime = runBodyPartSmoothTime;
        }
        else
        {
            bodySmoothTime = bodyPartSmoothTime;
        }
        foreach (Transform part in bodyParts)
        {
            part.localScale = curSize;
            part.GetComponent<SnakeBodyActions>().smoothTime = bodySmoothTime;
        }
    }

    public int curAmountOfRobot, maxAmountOfRobot = 30;  // The max amount of robots in the map
    public GameObject[] robotGenerateTarget;     // Store the objects of robot snakes

    // create robots
    void GenerateRobotBeforeBegin()
    {
        int i = 0;
        while (i < 20)
        {
            int r = Random.Range(0, 2);
            Vector3 robotPos;
            if (r == 0)
            {
                robotPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else
            {
                robotPos = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0);
            }
            GameObject newRobot = Instantiate(robotGenerateTarget[Random.Range(0, robotGenerateTarget.Length)],
                robotPos, Quaternion.identity) as GameObject;
            newRobot.name = "Robot" + i;
            Robots.Add(newRobot);
            newRobot.GetComponent<RobotAction>().SkinId = Random.Range(1, 4);
            newRobot.transform.parent = GameObject.Find("Robots").transform;
            curAmountOfRobot++;
            i++;
        }
    }

    /* Gernate 200 food points before game start*/
    void GenerateFoodBeforeBegin()
    {
        int i = 0;
        while (i < 200)
        {
            int r = Random.Range(0, 2);
            Vector3 foodPos;
            if (r == 0)
            {
                foodPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else
            {
                foodPos = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), 0);
            }
            GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], foodPos, Quaternion.identity) as GameObject;
            newFood.transform.parent = GameObject.Find("Foods").transform;
            curAmountOfFood++;
            i++;
        }
    }

    void GenerateStoneBeforeBegin()
    {
        int i = 0;
        while (i < 400)
        {
            Vector3 stonePos = new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), 0);
            GameObject newStone = Instantiate(stone, stonePos, Quaternion.identity) as GameObject;
            newStone.transform.parent = GameObject.Find("Stones").transform;
            curAmountOfFood++;
            i++;
        }
    }

    /* Generate food points every few seconds until there are enough points on the map*/
    public int curAmountOfFood, maxAmountOfFood = 600;  // The max amount of food in the map
    public int curAmountOfItem, maxAmountOfItem = 60;  // The max amount of item in the map
    private float foodGenerateEveryXSecond = 0.1f;   // Generate a food point every 3 seconds
    public GameObject[] foodGenerateTarget;     // Store the objects of food points
    public GameObject[] itemGenerateTarget;     // Store the objects of item
    public GameObject stone; // the object of stone
    void GenerateFoodAndItem()
    {
        StartCoroutine("RunGenerateFoodAndItem", foodGenerateEveryXSecond);
    }
    IEnumerator RunGenerateFoodAndItem(float time)
    {
        yield return new WaitForSeconds(time);
        StopCoroutine("RunGenerateFoodAndItem");
        if (curAmountOfFood < maxAmountOfFood)
        {
            int r = Random.Range(0, 4);
            Vector3 foodPos;
            if (r == 0)
            {
                foodPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else if (r <= 1)
            {
                foodPos = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), 0);
            }
            else if (r <= 2)
            {
                foodPos = new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), 0);
            }
            else
            {
                foodPos = new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), 0);
            }
            GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], foodPos, Quaternion.identity) as GameObject;
            newFood.transform.parent = GameObject.Find("Foods").transform;
            curAmountOfFood++;
        }
        if (curAmountOfItem < maxAmountOfItem)
        {
            int r = Random.Range(0, 4);
            Vector3 itemPos;
            if (r == 0)
            {
                itemPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else if (r <= 1)
            {
                itemPos = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), 0);
            }
            else if (r <= 2)
            {
                itemPos = new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), 0);
            }
            else
            {
                itemPos = new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), 0);
            }
            GameObject newItem = Instantiate(itemGenerateTarget[Random.Range(0, itemGenerateTarget.Length)], itemPos, Quaternion.identity) as GameObject;
            newItem.transform.parent = GameObject.Find("Items").transform;
            curAmountOfItem++;
        }
    }

    /* Make the snake head move forward all the time*/
    void SnakeMove()
    {
        transform.position += transform.forward * snakeWalkSpeed * Time.deltaTime;
    }
    /* Make the camera follow the snake when it moves*/
    void CameraFollowSnake()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 velocity = Vector3.zero;
        // Reach from current position to target position smoothly
        camera.position = Vector3.SmoothDamp(camera.position,
            new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10)
            , ref velocity, cameraSmoothTime);
    }

    /* Make the snake run when it should run, and lose parts*/
    private float t1;
    private float t2;
    void SnakeRun() {
        if (bodyParts.Count > 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                t2 = Time.realtimeSinceStartup;
                if (t2 - t1 < 0.2)
                {
                    isRunning = true;
                    snakeWalkSpeed = snakeRunSpeed;
                }
                t1 = t2;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isRunning = false;
                snakeWalkSpeed = 3.5f;
			
            }
        }
        else {
            isRunning = false;         
			snakeWalkSpeed = 3.5f;
        }
        if (isRunning == true)
        {
            StartCoroutine("LosingBodyParts");
        }
        
    }
    IEnumerator LosingBodyParts() {
        yield return new WaitForSeconds(0.8f);  // Every 0.8 second lose one body part
        StopCoroutine("LosingBodyParts");
        int lastIndex = bodyParts.Count - 1;
        Transform lastBodyPart = bodyParts[lastIndex].transform;
        bodyParts.RemoveAt(lastIndex);
        Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], lastBodyPart.position, Quaternion.identity);
        Destroy(lastBodyPart.gameObject);
        curAmountOfFood++;
        foodCounter--;
		length--;
        SnakeScaleChange();
    }
    /* If snake is running, then glowing*/
    void SnakeGlowing(bool isRunning) {
        foreach (Transform part in bodyParts) {
            part.Find("Glowing").gameObject.SetActive(isRunning);
        }
    }

    /* Choose the way of snake moving*/
    void ChooseControlMethod(int id)
    {
        switch (id) {
            case 1: MouseControlSnake();
                break;
            case 2: StickControl();
                break;
            case 3: DirectionControl();
                break;
        }
    }
    /* Sanke moves toward finger*/
    private Vector3 pointInWorld, mousePosition, direction;
    private float radius = 20.0f;
    void MouseControlSnake()
    {
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Store the first obj touched by ray
        Physics.Raycast(ray, out hit, 50.0f); // The third parameter is the max distance
        mousePosition = new Vector3(hit.point.x, hit.point.y, 0);
        direction = Vector3.Slerp(direction, mousePosition - transform.position, Time.deltaTime * 1);
        direction.z = 0;
        pointInWorld = direction.normalized * radius + transform.position;
        transform.LookAt(pointInWorld);
    }
    /* Use virtual joystick to control the direction of snake*/
    void StickControl() { }
    /* Slide finger to control the direction of snake*/
    void DirectionControl() { }

    void SnakeMoveAdjust()
    {
        Vector3 temp = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
        gameObject.transform.position = temp;
    }

    /* Choose the skin of snake*/
    public Material blue, red, orange;
    void ColorSnake(int id) {
        switch (id)
        {
            case 1: BlueAndWhite(); break;
            case 2: RedAndWhite(); break;
            case 3: OrangeAndWhite();break;
        }
    }
    void BlueAndWhite()
    {
        for (int i = 0; i < bodyParts.Count; i++) {
            if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = blue;
            }
        }
    }
    void RedAndWhite()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = red;
            }
        }
    }
    void OrangeAndWhite()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = orange;
            }
        }
    }

	// added by Yue Chen
	public void ShowAd()
	{
//		if (Advertisement.IsReady ()) {
//			Advertisement.Show ();
//		} else {
//			Advertisement.Initialize ("1165124");
//		}


		if (!Advertisement.IsReady())
		{
		Debug.Log("Ads not ready for default placement");
		return;
		}

		Advertisement.Show();

	}

	public void SetScore(int curScore){
		int bestScore;
		PlayerPrefs.SetInt ("FinalScore",curScore );
		bestScore = PlayerPrefs.GetInt ("BestScore", 0);
		if (bestScore < curScore) {
			PlayerPrefs.SetInt ("BestScore", curScore);
		}
	}


}
