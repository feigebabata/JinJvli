using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JinJvLi.Lobby;
// using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class FGBBT : MonoBehaviour
{
    public AudioSamples audioSamples;
    List<float> samples=new List<float>();
    public AudioSource audioSource;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KeyDown()
    {
        samples.Add(audioSource.time);
    }

    public void End()
    {
        audioSamples.Samples = samples.ToArray();
    }

    public void Play()
    {
        samples.Clear();
        audioSource.time = 0;
        audioSource.Play();
    }
    
    // [MenuItem("FGBBT/Run")]
    // public static void Run()
    // {
        
        
    // }
}
