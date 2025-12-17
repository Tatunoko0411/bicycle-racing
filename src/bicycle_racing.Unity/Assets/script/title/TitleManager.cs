using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveMtching(int StageId)
    {
        GameManager.StageId = StageId;

        Initiate.Fade("MatcingScene", Color.black, 1.5f);
    }
}
