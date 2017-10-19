using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotAction : MonoBehaviour {

    public List<Transform> robotBody = new List<Transform>();   // Records the location information of body parts of the snake

    public Transform addRobotBody;   // Called in OnCollisionEnter(), it is the thing added behind the snake after eating fo

    public int SkinId;
    //private bool isRunning; // Called in SnakeRun()
    private float robotWalkSpeed = 3.5f; // Called in SnakeMove()
    public Vector3 curSize = Vector3.one;  //Called in SizeUp()
    private float bodyPartSmoothTime = 0.2f; //Called in OnCollisionEnter(), the same value as in SnakeBodyActions.cs
    private float currentRotation; //Called in RobotRandomMove()
    private float rotationSensitivity = 50.0f; //Called in RobotRandomMove()

    public GameObject[] foodGenerateTarget;     // Store the objects of food points
  

    // Use this for initialization
    void Start () {
        InitiateBodies();
    }
	
	// Update is called once per frame
	void Update () {
       ColorRobot(SkinId);
    }

    void FixedUpdate()
    {
        RobotMove();
        SetBodySizeAndSmoothTime();
        RobotRandomMove();
    }

    // Initinate two body parts for each robot
    void InitiateBodies()
    {
        Vector3 currentPos;
        if (robotBody.Count == 0)
        {
            currentPos = transform.position;
        }
        else
        {
            currentPos = robotBody[robotBody.Count - 1].position;
        }
        Transform newPart1 = Instantiate(addRobotBody, currentPos, Quaternion.identity) as Transform;
        newPart1.parent = GameObject.Find("RobotBodies").transform;
        newPart1.name = this.name + "body";
        robotBody.Add(newPart1);
        Transform newPart2 = Instantiate(addRobotBody, currentPos, Quaternion.identity) as Transform;
        newPart2.parent = GameObject.Find("RobotBodies").transform;
        newPart2.name = this.name + "body";
        robotBody.Add(newPart2);
    }

    /* When the head encounters an object, figure out what to do*/
    void OnCollisionEnter(Collision obj)
    {
        if (obj.transform.tag == "Food")
        {
            Destroy(obj.gameObject);
         // The contents in 'if' shouldn't be exectued in logic as we always have several body parts
            Vector3 currentPos;
            if (robotBody.Count == 0)
            {
                currentPos = transform.position;
            }
            else
            {
                currentPos = robotBody[robotBody.Count - 1].position;
                Transform newPart = Instantiate(addRobotBody, currentPos, Quaternion.identity) as Transform;
                newPart.parent = GameObject.Find("RobotBodies").transform;
                newPart.name = this.name + "body";
                robotBody.Add(newPart);
            }
        }  
        else if (obj.transform.tag == "Boundary")
        {
            while (robotBody.Count > 0)
            {
                int lastIndex = robotBody.Count - 1;
                Transform lastBodyPart = robotBody[lastIndex].transform;
                robotBody.RemoveAt(lastIndex);
                Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)],
                    lastBodyPart.position, Quaternion.identity);
                Destroy(lastBodyPart.gameObject);
            }
            GameObject head = GameObject.Find(this.name);
            Destroy(head);
        }
        else if ((obj.transform.tag == "Snake") || (obj.transform.tag == "Player") || (obj.transform.tag == "Robot"))
        {
            while (robotBody.Count > 0)
            {
                int lastIndex = robotBody.Count - 1;
                Transform lastBodyPart = robotBody[lastIndex].transform;
                robotBody.RemoveAt(lastIndex);
                Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)],
                    lastBodyPart.position, Quaternion.identity);
                Destroy(lastBodyPart.gameObject);
            }
            GameObject head = GameObject.Find(this.name);
            Destroy(head);
        }
        else if ((obj.transform.tag == "Body"))
        {
            bool isMyself = false;
            Transform myself = obj.gameObject.transform;
            foreach (Transform part in robotBody)
            {
                if (part.Equals(myself))
                    isMyself = true;
            }
            if (isMyself == false)
            {
                while (robotBody.Count > 0)
                {
                    int lastIndex = robotBody.Count - 1;
                    Transform lastBodyPart = robotBody[lastIndex].transform;
                    robotBody.RemoveAt(lastIndex);
                    Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)],
                        lastBodyPart.position, Quaternion.identity);
                    Destroy(lastBodyPart.gameObject);
                }
                GameObject head = GameObject.Find(this.name);
                Destroy(head);
            }
        }
    }
    
    /* Make the snake head move forward all the time*/
    void RobotMove()
    {
        transform.position += transform.up * robotWalkSpeed * Time.deltaTime;
    }

    /* Set the size and smooth time of snake body parts every frame*/
    void SetBodySizeAndSmoothTime()
    {
        transform.localScale = curSize;
        foreach (Transform part in robotBody)
        {
            part.localScale = curSize;
            part.GetComponent<RobotBodyAction>().smoothTime = bodyPartSmoothTime;
        }
    }
   
    /* The robot snake moves randomly*/
    void RobotRandomMove()
    {
        int rotateInterval = Random.Range(3, 5);
        StartCoroutine("RunSnakeRandomRotate", rotateInterval);
        //int runningInterval = Random.Range(3, 5);
        //StartCoroutine("RunSnakeRandomRun", rotateInterval);
    }

    // Robot rotate ramdomly while moving
    IEnumerator RunSnakeRandomRotate(float interval)
    {
        yield return new WaitForSeconds(interval);
        StopCoroutine("RunSnakeRandomRotate");

        float deltaTime = Random.Range(1, 5);
        int dir = Random.Range(0, 2);
        if (dir == 0)
        {
            currentRotation += rotationSensitivity * deltaTime;
        }
        if (dir == 1)
        {
            currentRotation -= rotationSensitivity * deltaTime;
        }

        transform.rotation = Quaternion.Euler(
            new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }


    /* Choose the skin of snake*/
    public Material blue, red, orange;
    void ColorRobot(int id)
    {
        switch (id)
        {
            case 1: BlueAndWhite(); break;
            case 2: RedAndWhite(); break;
            case 3: OrangeAndWhite(); break;
        }
    }
    void BlueAndWhite()
    {
        for (int i = 0; i < robotBody.Count; i++)
        {
            if (i % 2 == 0)
            {
                robotBody[i].GetComponent<Renderer>().material = blue;
            }
        }
    }
    void RedAndWhite()
    {
        for (int i = 0; i < robotBody.Count; i++)
        {
            if (i % 2 == 0)
            {
                robotBody[i].GetComponent<Renderer>().material = red;
            }
        }
    }
    void OrangeAndWhite()
    {
        for (int i = 0; i < robotBody.Count; i++)
        {
            if (i % 2 == 0)
            {
                robotBody[i].GetComponent<Renderer>().material = orange;
            }
        }
    }
}
