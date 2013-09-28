#pragma strict

var speed = 10;

private var moveDirection = Vector3.zero;
private var velocity = speed;
private var runSpeed = speed * 2;
private var grounded = false;

function Start () {

}

function Update () 
{
	if(grounded)
	{
	//  taking the input and adding it into the vector moveDirection
		moveDirection = Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));   
	//  telling to move the face towards the direction got as input.	
		moveDirection = transform.TransformDirection(moveDirection);		
	//  Telling to add the force with the direction
		moveDirection *= velocity;
		
		if(Input.GetButton("Jump"))
		{
			// move the player upward
			moveDirection.y = speed;
		}
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))		// make the player run if shift is pressed
			velocity = runSpeed;
		else												// otherwise make the player run at normal spee
			velocity = speed;
	}
	// gravity pull
	moveDirection.y -= 20.0 * Time.deltaTime;
	
	var controller = GetComponent(CharacterController);
	var flags = controller.Move(moveDirection * Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow)!= 0;
}