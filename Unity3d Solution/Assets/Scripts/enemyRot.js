#pragma strict
var speed : int = 10;   	      	 // the speed of the object.
var sightRange : int = 5;		     // How fat can it see.
//var underRange : boolean = false;   // will be used in the enemies gun to fire at player, if the player is in range.


// Use this for initialization
function Start () {
}

// Update is called once per frame
function Update () 
{
	var controller = GetComponent(CharacterController);
	var velocity = speed;
	
	var fwd = transform.TransformDirection(Vector3.forward);   // not actually moving the object in forward direction, it just shows that 
	var hit : RaycastHit;   // Object contatining the information of the object that is hit.
	
	if(Physics.Raycast(transform.position, fwd, hit, 2.5))      // to check if there is something ahead. If there is one then rotate otherwise keep on moving.
	{
		transform.Rotate(0,90,0);
		if(Physics.Raycast(transform.position, fwd, hit, 2.5))      // to check if there is something ahead. If there is one then rotate otherwise keep on moving.
		{
			transform.Rotate(0,90,0);
			if(Physics.Raycast(transform.position, fwd, hit, 2.5))      // to check if there is something ahead. If there is one then rotate otherwise keep on moving.
			{
				transform.Rotate(0,90,0);
				if(Physics.Raycast(transform.position, fwd, hit, 2.5))      // to check if there is something ahead. If there is one then rotate otherwise keep on moving.
				{
						transform.Rotate(0,90,0);
				}
			}
		}
	}  
	controller.Move(fwd * velocity * Time.deltaTime);               // move the player ahead with the help of its direction i.e.fwd with velocity and in a smoother way
}

