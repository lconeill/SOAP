﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Soomla.Store;

public class SplashInit : MonoBehaviour 
{
    //public GameObject image_ref;
    //private Animator image_anim;
    //private Animation fadeColorAnimationClip;

    // Don't destroy this gameobject
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

	// Use this for initialization
	void Start () 
	{
		//image_anim = image_ref.GetComponent<Animator>();
		//fadeColorAnimationClip = image_ref.GetComponent<Animation>();
		//fadeColorAnimationClip.Play ("FadeToColor");

        SoomlaStore.Initialize(new SOAPStoreAssets());
		
		Invoke("LoadDelayed", 1.0f);
	}


    //Load the selected scene, by scene index number in build settings
	public void LoadDelayed()
	{
        Application.LoadLevel(1); // Load the classic start screen
	}
}
