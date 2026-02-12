using NUnit.Framework;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class SEManager : MonoBehaviour
{

   static AudioSource audioSource;

    [SerializeField]public List<AudioClip> SEs;

    [SerializeField]static public List<AudioClip> clipList;

    public enum SE
    {
        click = 0,
        Move,
        Count,
        Start,
        Break,
        Big,
        Small,
        put,
        Damage,
        Rap,
        Goal,
        Erectlo,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
        audioSource = GetComponent<AudioSource>();

        clipList = SEs;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// SEçƒê∂
    /// </summary>
    /// <param name="type">SEÇÃéÌóﬁ</param>
    static public void PlaySE(SE type)
    {
        audioSource.PlayOneShot(clipList[(int)type]);
    }
}
