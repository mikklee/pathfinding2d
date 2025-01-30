
//(c)Michael Thomas Lee

using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class will find the GameObject named "Player" and track its coordinates.
/// It will then output this through the Unity UI
/// </summary>
public class PositionText : MonoBehaviour
{

    private GameObject _trackedObject;
    private Transform _objectTransform;
    private Text _textHolder;

	// Use this for initialization
	public void Start ()
	{
        _trackedObject = GameObject.Find("Player");
        _objectTransform = _trackedObject.GetComponent<Transform>();
	    _textHolder = GetComponent<Text>();
	}

	// Update is called once per frame
	public void Update ()
	{
	    var x = Math.Round(_objectTransform.position.x);
	    var y = Math.Round(_objectTransform.position.y);
	    _textHolder.text =
            string.Format("X:{0} Y:{1}", x, y);
	}
}
