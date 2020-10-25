using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaCosmetic : MonoBehaviour
{
    [Header("plane")]
    public string planeName;
    public GameObject plane;
    public Transform planePos;

    [Header("character")]
    public string characterName;
    public GameObject character;
    public Transform characterPos;
    
    private void Start()
    {
        ChangePlane(planeName);
        ChangeCharacter(characterName);
    }

    public void ChangePlane(string name)
    {
        GameObject nPlane = CollectionCosmetic.instance.GetPlane(name);

        if(nPlane != null)
        {
            Destroy(plane);
            GameObject newPlane = Instantiate(nPlane, planePos);
            plane = newPlane;
        }
    }
    public void ChangeCharacter(string name)
    {
        GameObject nCharacter = CollectionCosmetic.instance.GetCharacter(name);

        if (nCharacter != null)
        {
            Destroy(character);
            GameObject newCharacter = Instantiate(nCharacter, characterPos);
            character = newCharacter;
        }
    }
}
