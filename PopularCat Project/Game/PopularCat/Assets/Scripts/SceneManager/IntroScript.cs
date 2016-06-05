using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroScript : MonoBehaviour {

    #region Public Fields
    public Sprite[] sprites;
    public float[] screenDuration = new float[9];
    public string[] dialogCollection = new string[8];
    #endregion

    #region Private Fields
    SpriteRenderer render;
    Text dialog;
    Renderer human;
    GameObject[] cats;
    Renderer bubble;
    Renderer thumb;

    int counter = 0;
    #endregion

    public Text message { get { return GameObject.Find("SceneText").GetComponent<Text>(); } }

    // Use this for initialization
    void Start ()
    {
        render = GetComponent<SpriteRenderer>();
        human = GameObject.Find("Human").GetComponent<Renderer>();
        cats = GameObject.FindGameObjectsWithTag("Character");
        bubble = GameObject.Find("SpeechBubble").GetComponent<Renderer>();
        thumb = GameObject.Find("Thumb").GetComponent<Renderer>();

        render.sprite = sprites[0];

        dialogCollection[0] = "Meet Piper.";
        dialogCollection[1] = "Piper only loves two things...";
        dialogCollection[2] = "his human...";
        dialogCollection[3] = "...and dancing.";
        dialogCollection[4] = "But piper has competition at home.";
        dialogCollection[5] = "His human barely notices him.";
        dialogCollection[6] = "But Piper has a plan to win his human back.";
        dialogCollection[7] = "A plan to become...";

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
            counter++;

            yield return new WaitForSeconds(screenDuration[counter]);
            StartCoroutine("PlayScene");
        }
    }

    void LoadSlide()
    {
        switch (counter)
        {
            case 0:
                message.text = dialogCollection[counter];
                break;
            case 1:
                message.text = dialogCollection[counter];
                break;
            case 2:
                message.text = dialogCollection[counter];
                human.enabled = true;
                break;
            case 3:
                message.text = dialogCollection[counter];
                render.sprite = sprites[1];
                break;
            case 4:
                message.text = dialogCollection[counter];
                foreach (GameObject cat in cats)
                {
                    Renderer thisCat = cat.GetComponent<Renderer>();
                    thisCat.enabled = true;
                }
                break;
            case 5:
                message.text = dialogCollection[counter];
                break;
            case 6:
                message.text = dialogCollection[counter];
                human.enabled = false;
                foreach (GameObject cat in cats)
                {
                    Renderer thisCat = cat.GetComponent<Renderer>();
                    thisCat.enabled = false;
                }
                render.sprite = sprites[2];
                GetComponent<Transform>().position = new Vector3(0, 5.5f, 0);
                break;
            case 7:
                message.text = dialogCollection[counter];
                render.sprite = sprites[3];
                GetComponent<Transform>().position = new Vector3(0, 2.9f, 0);
                bubble.enabled = true;
                thumb.enabled = true;
                break;
            case 8:
                //message.text = dialogCollection[counter];
                break;
        }
    }
}