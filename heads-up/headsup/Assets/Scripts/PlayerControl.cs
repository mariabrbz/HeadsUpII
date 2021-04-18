using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    public float speed = 15;
    public float turnSpeed = 50;
    public GameObject leftBlinker;
    public GameObject rightBlinker;
    public TextMesh speedText;
    public GameObject stopMistake;
    public GameObject blinkerMistake;
    public GameObject speedingMistake;
    public GameObject priorityMistake;
    public GameObject noturnMistake;
    public GameObject Congrats;
    public GameObject[] questions;
    
    private float horizontal;
    private float vertical;
    private bool accelerate = true;
    private Color breakColor;
    private bool left = false;
    private bool right = false;
    private Animator leftAnimator;
    private Animator rightAnimator;
    private bool stopped = false;
    private bool speeding = false;
    private bool turn = false;
    private string correct;
    private GameObject question;
    private Transform resultText;
    private Transform playButton;
    private List<Transform> results = new List<Transform>();
    private List<Transform> answersChildren = new List<Transform>();
    private Dictionary<string, GameObject> mistakes = new Dictionary<string, GameObject>();
    private int n = 3;
    private float mistakeSeconds = 0;
    private int nicejob = 0;

    private void Start()
    {
        breakColor = leftBlinker.GetComponent<MeshRenderer>().materials[0].color;
        leftAnimator = leftBlinker.GetComponent<Animator>();
        rightAnimator = rightBlinker.GetComponent<Animator>();

        stopMistake.SetActive(false);
        blinkerMistake.SetActive(false);
        priorityMistake.SetActive(false);
        Congrats.SetActive(false);
        speedingMistake.SetActive(false);
        noturnMistake.SetActive(false);
        mistakes.Add("stop", stopMistake);
        mistakes.Add("blinker", blinkerMistake);
        mistakes.Add("priority", priorityMistake);
        mistakes.Add("speeding", speedingMistake);
        mistakes.Add("noturn", noturnMistake);
        foreach (GameObject q in questions)
        {
            q.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (nicejob >= 5)
        {
            Win();
        }
        mistakeSeconds += Time.deltaTime;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (vertical != 0)
        {
            speedText.text = speed.ToString();
        }
        else
        {
            speedText.text = " 0";
        }

        if (horizontal != 0)
        {
            if (horizontal < -0.9f)
            {
                if (!leftAnimator.GetBool("left"))
                {
                    if (mistakeSeconds >= 2)
                    {
                        Mistake("blinker");
                        mistakeSeconds = 0;
                    }
                }
            }

            if (horizontal > 0.9f)
            {
                if (!rightAnimator.GetBool("right"))
                {
                    if (mistakeSeconds >= 2)
                    {
                        Mistake("blinker");
                        mistakeSeconds = 0;
                    }
                }
            }
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime * vertical);
        transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * horizontal);

        if (Input.GetKey("space"))
        {
            if (accelerate)
            {
                speed += 1;
            }
            else
            {
                if (speed > 15) { speed -= 1; }
            }
        }

        if (Input.GetKeyUp("space"))
        {
            accelerate = !accelerate;
        }

        if (Input.GetKeyDown("z"))
        {
            leftAnimator.SetBool("left", !left);
            if (left)
            {
                leftBlinker.GetComponent<MeshRenderer>().materials[0].color = breakColor;
            }
            left = !left;
        }

        if (Input.GetKeyDown("x"))
        {
            rightAnimator.SetBool("right", !right);
            if (right)
            {
                rightBlinker.GetComponent<MeshRenderer>().materials[0].color = breakColor;
            }
            right = !right;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "StopSign")
        {
            if (vertical == 0)
            {
                stopped = true;
            }
        }

        if (other.tag == "Priority")
        {
            if (speed > 15 && vertical != 0)
            {
                speeding = true;
            }
        }

        if (other.tag == "SpeedLimit")
        {
            if (speed > 25 && vertical != 0)
            {
                speeding = true;
            }
        }

        if (other.tag == "NoLeft")
        {
            if (horizontal < -0.9f)
            {
                turn = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "StopSign")
        {
            if (!stopped)
            {
                if (mistakeSeconds >= 2)
                {
                    Mistake("stop");
                    mistakeSeconds = 0;
                }
                stopped = false;
            }
            else
            {
                Congrats.SetActive(true);
                StartCoroutine(Dissapear(Congrats));
            }
        }

        if (other.tag == "Priority")
        {
            if (speeding)
            {
                if (mistakeSeconds >= 2)
                {
                    Mistake("priority");
                    mistakeSeconds = 0;
                }
                speeding = false;
            }
            else
            {
                Congrats.SetActive(true);
                StartCoroutine(Dissapear(Congrats));
            }
        }

        if (other.tag == "SpeedLimit")
        {
            if (speeding)
            {
                if (mistakeSeconds >= 2)
                {
                    Mistake("speeding");
                    mistakeSeconds = 0;
                }
                speeding = false;
            }
            else
            {
                Congrats.SetActive(true);
                StartCoroutine(Dissapear(Congrats));
            }
        }

        if (other.tag == "NoLeft")
        {
            if (turn)
            {
                if (mistakeSeconds >= 2)
                {
                    Mistake("noturn");
                    mistakeSeconds = 0;
                }
                turn = false;
            }
            else
            {
                Congrats.SetActive(true);
                StartCoroutine(Dissapear(Congrats));
            }
        }
    }

    void Mistake(string type)
    {
        if (Congrats.activeSelf) {
            Congrats.SetActive(false);
        }
        Time.timeScale = 0;
        mistakes[type].SetActive(true);
        Button getQuestion = mistakes[type].transform.GetChild(3).GetComponent<Button>();
        getQuestion.onClick.AddListener(() => LoadQuestion(mistakes[type]));
    }

    IEnumerator Dissapear(GameObject go)
    {
        yield return new WaitForSeconds(2);
        nicejob++;
        go.SetActive(false);
    }

    void LoadQuestion(GameObject prev)
    {
        prev.SetActive(false);
        if (prev.name == "BlinkerButton")
        {
            Destroy(prev);
        }
        if (n < questions.Length)
        {
            question = questions[n];
            n++;
        }
        else
        {
            return;
        }
        question.SetActive(true);
        resultText = question.transform.Find("ResultText");
        playButton = null;


        foreach (Transform element in question.transform)
        {
            if (element.gameObject.GetComponent<Button>())
            {
                playButton = element;
                element.GetChild(0).gameObject.GetComponent<Text>().text = "Continue after answering the question";
                element.gameObject.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
            }
        }

        Transform answers = question.transform.Find("Answers");
        resultText.gameObject.SetActive(false);
        foreach (Transform child in answers.transform)
        {
            answersChildren.Add(child);
            foreach (Transform grandchild in child.transform)
            {
                if (grandchild.tag == "Result")
                {
                    results.Add(grandchild);
                    grandchild.gameObject.SetActive(false);
                    if (grandchild.GetComponent<Image>().sprite.name == "check") 
                    {
                        correct = grandchild.parent.name;
                    }
                }
            }
        }

        foreach (Transform child in answers.transform)
        {

            Image img = child.GetComponent<Image>();
            if (child.name == correct)
            {
                playButton.gameObject.SetActive(true);
                child.GetComponent<Button>().onClick.AddListener(() => RightAnswer(img));
            }

            else
            {
                playButton.gameObject.SetActive(true);
                child.GetComponent<Button>().onClick.AddListener(() => WrongAnswer(img));
            }

        }

        if (n >= 5)
        {
            StartCoroutine(LoseGame());
        }
    }

    void RightAnswer(Image i)
    {
        playButton.gameObject.GetComponent<Button>().onClick.AddListener(() => Resume());
        foreach (Transform answerChild in answersChildren)
        {
            answerChild.GetComponent<Button>().enabled = false;
        }
        foreach (Transform child in results)
        {
            child.gameObject.SetActive(true);
        }

        playButton.GetChild(0).gameObject.GetComponent<Text>().text = "Back to the game";
        playButton.gameObject.GetComponent<Image>().color = new Color(0, 0.86f, 0.36f);

        i.color = new Color(0, 0.86f, 0.36f);
        resultText.gameObject.GetComponent<Text>().text = "You are right! Great job!";
        resultText.gameObject.SetActive(true);
    }

    void WrongAnswer(Image i)
    {
        playButton.gameObject.GetComponent<Button>().onClick.AddListener(() => Resume());
        foreach (Transform answerChild in answersChildren)
        {
            answerChild.GetComponent<Button>().enabled = false;
        }
        foreach (Transform child in results)
        {
            child.gameObject.SetActive(true);
        }

        playButton.GetChild(0).gameObject.GetComponent<Text>().text = "Back to the game";
        playButton.gameObject.GetComponent<Image>().color = new Color(0, 0.86f, 0.36f);

        i.color = new Color(1, 0, 0);
        resultText.gameObject.GetComponent<Text>().text = "Now you know! You'll do better next time!";
        resultText.gameObject.SetActive(true);
    }

    void Resume()
    {
        Time.timeScale = 1;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
        question.SetActive(false);
    }

    IEnumerator LoseGame()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("GameFinish");
    }

    void Win()
    {
        SceneManager.LoadScene("GameWon");
    }
}
