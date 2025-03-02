using System;
using UnityEditor.UIElements;
using UnityEngine;

public class BricksLogic : MonoBehaviour
{

    [SerializeField] private bool Invisible = false;
    [SerializeField] private bool Lucky = false;
    [SerializeField] private bool Smash = false;
    [SerializeField] private bool isRandom = true;
    [SerializeField] private string location;
    [SerializeField] private GameObject[] RandomSeed;
    [SerializeField] private GameObject GaurenteeObject;
    [SerializeField] private GameObject CheckMush;
    [SerializeField] private GameObject FireReplace;
    [SerializeField] private GameObject BrokenBrick;
    [SerializeField] private int Hp=1;
    private  string MariosmallTag = "Small Mario";
    private int i = 0;

    [SerializeField] private Animator BrickAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BrickAnim = GetComponent<Animator>();
        BrickAnim.SetBool("Lucky",Lucky);
        if (Invisible)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled=false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SpewObject(bool other)
    {
        GameObject Placed;
        if (isRandom&&RandomSeed.Length>0)
        {
            GameObject TempOb = RandomSeed[UnityEngine.Random.Range(0, RandomSeed.Length)];
            if (TempOb == CheckMush&& other)
            {
                TempOb = FireReplace;
            }
            Placed=Instantiate(TempOb);
            Placed.transform.position = gameObject.transform.position;
        }
        else if(GaurenteeObject!=null)
        {
            if (GaurenteeObject == CheckMush && other)
            {
                Placed=Instantiate(FireReplace);
            }
            else
            {
                Placed=Instantiate(GaurenteeObject);
            }
            Placed.transform.position = gameObject.transform.position;
        }
    }

    private void Break()
    {
        GameObject Placed = Instantiate(BrokenBrick);
        Placed.transform.position = gameObject.transform.position;
        Destroy(gameObject);
        AudioManager.Instance.PlaySFX("BrickBreak");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled=true;
        if (Hp > 0)
        {
            if(!Smash)
                Hp -= 1;
            BrickAnim.Play("Brick Bounce", 0);
            SpewObject(!other.CompareTag(MariosmallTag));
            AudioManager.Instance.PlaySFX("Brickbump");
        }
        if(Hp==0&&!Smash)
        {
            Lucky = false;
            BrickAnim.SetBool("Lucky", Lucky);
            BrickAnim.Play("Hp", 1);
        }
        else if(Smash&&!other.CompareTag(MariosmallTag))
        {
            Break();
        }
    }
}

