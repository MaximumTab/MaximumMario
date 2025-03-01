using System;
using UnityEngine;

public class BricksLogic : MonoBehaviour
{
    private string Name = "No Block Chosen";
    private string Location = "No Location Chosen";

    [SerializeField] private bool Invisible = false;
    [SerializeField] private bool Lucky = false;
    [SerializeField] private bool Smash = false;
    [SerializeField] private bool isRandom = true;
    [SerializeField] private string location;
    [SerializeField] private GameObject[] RandomSeed;
    [SerializeField] private GameObject GaurenteeObject;
    [SerializeField] private int Hp=1;
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

    private void SpewObject()
    {
        if (isRandom)
        {
            Instantiate(RandomSeed[UnityEngine.Random.Range(0,RandomSeed.Length)]);
        }
        else
        {
            Instantiate(GaurenteeObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled=true;
        if (Hp > 0)
        {
            if(!Smash)
                Hp -= 1;
            BrickAnim.Play("Brick Bounce", 0);
        }
        if(Hp==0&&!Smash)
        {
            Lucky = false;
            BrickAnim.SetBool("Lucky",Lucky);
            BrickAnim.Play("Hp",1);
        }
        else if(Smash)
        {
            
        }
    }
}

