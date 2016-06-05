using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroScript : MonoBehaviour {

    #region Public Fields
    public Sprite[] sprites;
    public float[] screenDuration;
    public AudioClip[] angrySounds;
    public AudioClip[] happySounds;
    #endregion

    #region Private Fields
    Camera main;
    SpriteRenderer piper;
    Text dialog;
    Renderer background;
    Renderer human;
    GameObject[] cats;
    Transform bubbleObj;
    Renderer bubble;
    Renderer poop;
    Renderer thumb;
    Renderer logo;
    AudioSource meow;

    int counter = 0;
    #endregion

    public Text message { get { return GameObject.Find("SceneText").GetComponent<Text>(); } }

    // Use this for initialization
    void Start ()
    {
        main = GameObject.Find("Main Camera").GetComponentInChildren<Camera>();
        piper = GetComponent<SpriteRenderer>();
        background = GameObject.Find("Background").GetComponent<Renderer>();
        human = GameObject.Find("Human").GetComponent<Renderer>();
        cats = GameObject.FindGameObjectsWithTag("Character");
        bubble = GameObject.Find("SpeechBubble").GetComponent<Renderer>();
        bubbleObj = GameObject.Find("SpeechBubble").GetComponent<Transform>();
        poop = GameObject.Find("Poop").GetComponent<Renderer>();
        thumb = GameObject.Find("Thumb").GetComponent<Renderer>();
        logo = GameObject.Find("Logo").GetComponent<Renderer>();
        meow = GetComponent<AudioSource>();

        piper.sprite = sprites[0];

        StartCoroutine("PlayScene");
    }
	
	// Update is called once per frame
	void Update ()
    {
        //// Allow player to quit to main menu
        //if (Input.GetKey(KeyCode.Escape))
        //{
        //    SceneManager.LoadScene("Level0");
        //    FirstGameManager.GameState.SetCurrentLevel("Level0");
        //}
    }

    IEnumerator PlayScene()
    {
        LoadSlide();

        if (counter < screenDuration.Length - 1)
        {
            yield return new WaitForSeconds(screenDuration[counter]);

            counter++;
            StartCoroutine("PlayScene");
        }
    }

    void LoadSlide()
    {
        switch (counter)
        {
            case 0:

                break;
            case 1:
                piper.enabled = true;
                background.enabled = true;
                break;
            case 2:
                HappyMeow();
                message.text = "Meet Piper.";
                break;
            case 3:
                message.text = "Piper only loves two things...";
                break;
            case 4:
                message.text = "his human...";
                human.enabled = true;
                break;
            case 5:
                HappyMeow();
                message.text = "...and dancing.";
                piper.sprite = sprites[1];
                break;
            case 6:
                message.text = "But piper has competition at home.";
                foreach (GameObject cat in cats)
                {
                    Renderer thisCat = cat.GetComponent<Renderer>();
                    thisCat.enabled = true;
                    StartCoroutine("DelayedMeow");
                }
                break;
            case 7:
                AngryMeow();
                message.text = "His human barely notices him.";
                bubble.enabled = true;
                poop.enabled = true;
                break;
            case 8:
                message.text = "But Piper has a plan to win his human back.";
                human.enabled = false;
                bubble.enabled = false;
                poop.enabled = false;
                foreach (GameObject cat in cats)
                {
                    Renderer thisCat = cat.GetComponent<Renderer>();
                    thisCat.enabled = false;
                }
                piper.sprite = sprites[2];
                GetComponent<Transform>().position = new Vector3(0, 5.5f, 0);
                break;
            case 9:
                message.text = "A plan to become...";
                piper.sprite = sprites[3];
                GetComponent<Transform>().position = new Vector3(0, 2.9f, 0);
                bubbleObj.localPosition = new Vector3(-6, 14, 0);
                bubbleObj.localScale = new Vector3(5, 7, 1);
                bubble.enabled = true;
                thumb.enabled = true;
                break;
            case 10:
                background.enabled = false;
                message.enabled = false;
                piper.enabled = false;
                bubble.enabled = false;
                thumb.enabled = false;
                logo.enabled = true;
                Camera.main.backgroundColor = new Color(0.682f, 0.369f, 0.369f);
                break;
        }
    }

    IEnumerator DelayedMeow()
    {
        yield return new WaitForSeconds(Random.Range(0, 5.5f));
        HappyMeow();
    }

    void HappyMeow()
    {
        meow.pitch = Random.Range(0.9f, 1.1f);
        meow.PlayOneShot(happySounds.RandomItem());
    }

    void AngryMeow()
    {
        meow.pitch = Random.Range(0.9f, 1.1f);
        meow.PlayOneShot(angrySounds.RandomItem());
    }
}