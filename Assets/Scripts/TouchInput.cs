﻿using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {

    private int screen_width;           // Width of the mobile device
    private Vector2 touch_position;     // The pixel position of the players touch
    private Vector3 mouse_position;     // For testing in unity editor
    public AvatarController avatar_controller_script;   // Reference to the AvatarController script
    private float time_of_last_tap = 0;     // The time when the last tap occured
    private float minimum_tap_time = 0.3f;  // Anything under this number is considered a double tap
    public bool tap_valid = true;           // Is the tap a double tap; when a double tap occurs this is false
    private bool side_touched;
    private bool pre_side_touched;

    private Touch initial_touch = new Touch();
    private float swipe_distance = 0;
    private bool has_swiped = false;

    public static string control_type;
    public const string swipe_control = "swipe";
    public const string classic_control = "classic";
    public const string arrow_control = "arrow";

	// Use this for initialization
	void Start () 
    {
        // Get screen width
        screen_width = Screen.width;

        // Get the control type stored in the player prefs (set in UI Manager)
        control_type = getControlType();
	}
	

	// Update is called once per frame
	void Update () 
    {
        //if (Input.touchCount == 1)
        //{
        //    sideTouched();
        //}

        // Activate swipe control if not in pause menu
        if (control_type == TouchInput.swipe_control && !UIManager.is_paused)
        {
            swipe_controls();
        }

        else if (control_type == TouchInput.classic_control)
        {
            editorSideTouched();
        }

        else if (control_type == TouchInput.arrow_control)
        {
            // Do nothing input handled by button presses
        }

        //sideTouched();
        //editorSideTouched();
	}

    
    // Get the control type
    public string getControlType()
    {
        return PlayerPrefs.GetString("ControlType");
    }


    // Determine whether user clicked on right or left side of the screen
    public void sideTouched()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began && !BlockRaycast.IsPointerOverUIObject())
        {
            // Left = true right = false
            touch_position = Input.GetTouch(0).position;
            side_touched = touch_position.x <= screen_width/2;
            tap_valid = Time.time > time_of_last_tap + minimum_tap_time || side_touched != pre_side_touched;
            avatar_controller_script.setTapValid(tap_valid);

            // Check if the player tapped too fast
            //tap_valid = Time.time > time_of_last_tap + minimum_tap_time;
            //avatar_controller_script.setTapValid(tap_valid);
            //touch_position = Input.GetTouch(0).position;

            // Left = turn ccw, Right = turn cw
            //if (touch_position.x <= screen_width / 2)
            if (side_touched)
            {
                avatar_controller_script.rotate_avatar(Mathf.PI / 2);
            }

            else
            {
                avatar_controller_script.rotate_avatar(-Mathf.PI / 2);
            }

            time_of_last_tap = Time.time;
            pre_side_touched = side_touched;
        }
    }


     //For testing in the editor
    public void editorSideTouched()
    {
        if (Input.GetMouseButtonDown(0) && !BlockRaycast.IsPointerOverUIObject())
        {
            mouse_position = Input.mousePosition;
            side_touched = mouse_position.x <= screen_width/2;
            tap_valid = Time.time > time_of_last_tap + minimum_tap_time || side_touched != pre_side_touched;
            avatar_controller_script.setTapValid(tap_valid);

            //// Check if the player tapped too fast
            //tap_valid = Time.time > time_of_last_tap + minimum_tap_time;
            //avatar_controller_script.setTapValid(tap_valid);
            //mouse_position = Input.mousePosition;

            //if (mouse_position.x <= screen_width / 2)
            if (side_touched)
            {
                avatar_controller_script.rotate_avatar(Mathf.PI / 2);
            }

            else
            {
                avatar_controller_script.rotate_avatar(-Mathf.PI / 2);
            }

            time_of_last_tap = Time.time;
            pre_side_touched = side_touched;
        }
    }


    // Logic for arrow control mechanism
    public void arrowControls(string direction)
    {
        avatar_controller_script.simpleRotateAvatar(direction);
    }


    // Logic for swipe control mechanism
	public void swipe_controls()
	{
		foreach(Touch touch in Input.touches)
        {

        	if (touch.phase == TouchPhase.Began)
        	{
				initial_touch = touch;
			}

			else if (touch.phase == TouchPhase.Moved && !has_swiped)
			{
				float x_delta = initial_touch.position.x - touch.position.x;
				float y_delta = initial_touch.position.y - touch.position.y;
				bool swiped_sideways = Mathf.Abs(x_delta) > Mathf.Abs(y_delta);
				swipe_distance = Mathf.Sqrt((x_delta * x_delta) + (y_delta * y_delta));

				if (swipe_distance > 10f)
				{
					// Swipe left
					if (swiped_sideways && x_delta > 0)
					{

						if (initial_touch.position.x <= (Screen.width - Screen.width * 0.05))
						{
                            avatar_controller_script.simpleRotateAvatar("left");
						}
					}

					// Swipe Right
					else if (swiped_sideways && x_delta < 0)
					{
						if (initial_touch.position.x >= Screen.width * 0.05)
						{
                            avatar_controller_script.simpleRotateAvatar("right");
						}
					}

					// Swipe Down
					else if (!swiped_sideways && y_delta > 0)
					{
                        avatar_controller_script.simpleRotateAvatar("down");
					}

					// Swipe Up
					else if (!swiped_sideways && y_delta < 0)
					{
                        avatar_controller_script.simpleRotateAvatar("up");
					}

					has_swiped = true;
				}
			}

			else if (touch.phase == TouchPhase.Ended)
			{
				initial_touch = new Touch();
				has_swiped = false;
			}
        }
	}
}


