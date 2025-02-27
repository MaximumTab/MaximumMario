using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BricksLogic : MonoBehaviour
{
    private string Name="No Block Chosen";
    private string Location= "No Location Chosen";
    [SerializeField] private Sprite[] DisplayBricks;

    [SerializeField] private bool Invisible=false;

    [SerializeField] private Animator BrickAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BrickAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string BrickSet(string name, string location)
    {
        int i = 0;
        switch (location)
        {
            case "Underground":
                Location = location;
                i = 15;
                break;
            case "Overworld":
                Location = location;
                i = 0;
                break;
            default:
                Location = "No Location Chosen";
                break;
        }
        switch (name)
        {
            case "Lucky Block":
                Name = name;
                break;
            case "Smash Block":
                Name = name;
                i += 10;
                break;
            case "Safe Block":
                Name = name;
                i += 5;
                break;
            default:
                Name = "No Block Chosen";
                break;
        }
        Spritebrick(i);
        return Name+", "+Location;
    }

    void Spritebrick(int i)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = DisplayBricks[i];
    }

    void OnTriggerEnter()
    {
        
    }
}

