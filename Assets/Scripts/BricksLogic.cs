using System;
using UnityEditor;
using UnityEngine;

public class BricksLogic : MonoBehaviour
{
    private string Name="No Block Chosen";
    private string Location= "No Location Chosen";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string BrickSet(string name, string location)
    {
        
        switch (location)
        {
            case "Underground":
                Location = location;
                break;
            case "Overworld":
                Location = location;
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
            case "Smash Blocks":
                Name = name;
                break;
            case "Safe Blocks":
                Name = name;
                break;
            default:
                Name = "No Block Chosen";
                break;
        }

        return Name+", "+Location;
    }

}

