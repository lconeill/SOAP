﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


//
// Logic for having falling obstacles be triggered individually 
//

public class BallTrigger : MonoBehaviour {

    public GameObject[] ball_trigger_go;    // Array of ball trigger game objects
    public GameObject avatar;               // Avatar game object
    public GameObject ball_reset_go;        // Reset trigger game object

    private BoxCollider2D ball_reset_trigger;       // Box collider component of reset trigger game object
    private Vector2 force_push_left = new Vector2(-1.0f, 0.0f);     // Vector representing leftwards force
    private Vector2 force_push_right = new Vector2(1.0f, 0.0f);     // Vector representing rightwards force
    private Vector2 force_push_up = new Vector2(0.0f, 1.0f);        // Vector representing upwards force
    private BoxCollider2D[] trigger_box_collider;       // Array of box collider components of trigger game objects
    private List<GameObject[]> ball_elements = new List<GameObject[]>(); // Each array contains ball game objects corresponding to its trigger parent
    private List<Vector3[]> ball_original_positions = new List<Vector3[]>(); // List of arrays of original ball positions


    // Use this for initialization
	void Start () 
	{
        ball_reset_trigger = ball_reset_go.GetComponent<BoxCollider2D>();
        trigger_box_collider = new BoxCollider2D[ball_trigger_go.Length];

        for (int i = 0; i < ball_trigger_go.Length; i++)
        {
            trigger_box_collider[i] = ball_trigger_go[i].GetComponent<BoxCollider2D>();
        }

        for (int j = 0; j < ball_trigger_go.Length; j++)
        {
            ball_elements.Add(getBallChildren(ball_trigger_go[j]));
            ball_original_positions.Add(getBallPosition(ball_trigger_go[j]));
        }
	}
	

	// Check if avatar collider is touching any of the triggers and apply force to its children
	void Update () 
	{

        for (int i = 0; i < trigger_box_collider.Length; i++)
        {
            if (trigger_box_collider[i].IsTouching(avatar.GetComponent<CircleCollider2D>()))
            {
                int force = getForce(ball_trigger_go[i].name);
                string force_tag = trigger_box_collider[i].tag;
                dropBalls(ball_elements[i], force, force_tag);
            }
        }

        if (ball_reset_trigger.IsTouching(avatar.GetComponent<CircleCollider2D>()))
        {
            resetBallPositions();
        }
    }


    // Return an array of all children under the trigger GameObject
    private GameObject[] getBallChildren(GameObject trigger)
    {
        int ball_count = trigger.transform.childCount;
        GameObject[] balls = new GameObject[ball_count];
        
        int counter = 0;

        foreach (Transform child in trigger.transform)
        {
            balls[counter] = child.gameObject;
            counter++;
        }

        return balls;
    }


    // Return an array of ball original positions
    private Vector3[] getBallPosition(GameObject trigger)
    {
        int ball_count = trigger.transform.childCount;
        Vector3[] ball_position = new Vector3[ball_count];

        int counter = 0;

        foreach (Transform child in trigger.transform)
        {
            ball_position[counter] = child.transform.position;
            counter++;
        }

        return ball_position;
    }


    // Extract the force magnitude from the trigger name
    private int getForce(string trigger_name)
    {
        Match match = Regex.Match(trigger_name, @"_fx[0-9]+", RegexOptions.IgnoreCase);
        int force = 0;

        if (match.Success)
        {
            string key = match.Groups[0].Value;
            string force_str = key.Split(new string[] {"fx"}, StringSplitOptions.None)[1];
            force = Convert.ToInt32(force_str);
        }

        return force;
    }


    // Apply force to the balls under the trigger group according to force magnitude and tag 
    private void dropBalls(GameObject[] ball_array, int force, string force_tag)
    {
        foreach (GameObject ball in ball_array)
        {
            ball.GetComponent<Rigidbody2D>().isKinematic = false;

            switch (force_tag)
            {
                case "force_push_right":
                    ball.GetComponent<Rigidbody2D>().AddForce(force_push_right * force);
                    break;

                case "force_push_left":
                    ball.GetComponent<Rigidbody2D>().AddForce(force_push_left * force);
                    break;

                case "force_push_up":
                    ball.GetComponent<Rigidbody2D>().AddForce(force_push_up * force);
                    break;
            }
        }
    }


    // Reset the ball positions to their original values
    private void resetBallPositions()
    {
        int array_count = 0;
        int ball_count = 0;

        foreach (GameObject[] go in ball_elements)
        {
            foreach (GameObject ball in go)
            {
                ball.transform.position = ball_original_positions[array_count][ball_count];
                ball.GetComponent<Rigidbody2D>().isKinematic = true;
                ball_count++;
            }

            array_count++;
            ball_count = 0;
        }
    }
}

