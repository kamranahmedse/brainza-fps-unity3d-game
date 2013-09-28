#pragma strict


var gun : Transform;
var delay : float = 0.1;					

var nextPosX : float = 0.3;					// targeted position
var nextPosY : float = -0.4;				// --
var nextPosZ : float = 0.7;					// --
var nextField : float = 60.0;			

var dampX : float = 0.3;					// velocity
var dampY : float = 0.3;					// --
var dampZ : float = 0.3;					// --
var dampF : float = 0.3;					

public var zAdd : float = 0.0;

function Start () 
{

}

function Update () 
{
	var newPosX = Mathf.SmoothDamp(gun.localPosition.x, nextPosX, dampX, delay);		// will contain the values for the smoothly damping positions
	var newPosY = Mathf.SmoothDamp(gun.localPosition.y, nextPosY, dampY, delay);		// ------
	var newPosZ = Mathf.SmoothDamp(gun.localPosition.z, nextPosZ, dampZ, delay);		//------
	var newF = Mathf.SmoothDamp(Camera.main.fieldOfView, nextField, dampF, delay);
	
	gun.localPosition.x = newPosX;				// taking the gun back to its default position
	gun.localPosition.y = newPosY;				//--
	gun.localPosition.z = newPosZ - zAdd;		//--
	
	Camera.main.fieldOfView = newF;				// taking the camera back to default position
	
	if(Input.GetButton("Fire2"))				// if the right mouse button is clicked
	{
		nextPosX = 0.0;							// take the gun to the center of screen
		nextPosY = -0.2;						// change the height
		nextField = 40.0;						// zoom in to the camera
	}
	else
	{
		nextPosX = 0.3;							// other wise take every thing back to default.
		nextPosY = -0.4;
		nextField = 60;
	}
}