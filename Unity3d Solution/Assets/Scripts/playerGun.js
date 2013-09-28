#pragma strict

// var  sparks : Transform; // i.e. Transform sparks; in C++. Created the variable sparks of type transform because it is going to be a game object.
							// Was used because it was just a prefab that was being used
var sparks : GameObject;	// Now we will be using a gameobject i.e. a part of the scence it self and will be shifting its location, whenever fire button will be clicked.
							// Will be more efficient as there will be just the object shifting and no instantiating again and again.
var ammo : int = 100;     // i.e. int ammo = 100; in C++. This will be showing the number of bullets currently available
var fireRate : float = 0.2;
var recall : float = 1;
var shotCount : int = 0;

var gunShot : AudioClip;

private var nextFire : float = 0.0; // private will ensure that it will not exist in the unity it would be only the part of script.

var damage : float = 10;              // i.e. how much damage will be made by this gun.

function Start () {

}

function Update ()		// A function that is called every frame of the game.
 {
 	bulletCounter.bullets = ammo;
 				
	if(Input.GetButton("Fire1") && ammo > 0)		//Input.GetButton Class contains all the input.
	{      		
		Screen.lockCursor = true;
		if(Time.time > nextFire)    //will avoid the firing to occur every frame.
		{ 	
			//Screen.lockCursor = true;	
			nextFire = Time.time + fireRate;    // For how much time we want to delay it.
			fire();
			audio.PlayOneShot(gunShot);
			// making the effect of gun moving back
			transform.parent.GetComponent(playerCam).zAdd = recall/10;
			ammo--;
		}
		else
			transform.parent.GetComponent(playerCam).zAdd = 0;
	}
	else if(Input.GetKey(KeyCode.R))
	{
		transform.parent.GetComponent(playerCam).zAdd = 0;
		ammo = 100;
	}
	else
	{
		shotCount = 0;
		transform.parent.GetComponent(playerCam).zAdd = 0;
	}
}

function fire()
{
//  Just chosen, so that the shooter may shoot at an infinite range.
	//var range = Mathf.Infinity;					
	
	var mousePos = Vector3(Input.mousePosition.x + Random.Range(-shotCount, shotCount), Input.mousePosition.y + Random.Range(-shotCount, shotCount), Input.mousePosition.z);

//	var ray = Camera.main.ScreenPointToRay(Input.mousePosition);     //to throw something/ray at the current position of mouse.		
	var ray = Camera.main.ScreenPointToRay(mousePos);

// to avoid the larger spread of sparks.
	if(shotCount < 5)				
		shotCount++;
	else
		shotCount = 0;

// will contain the information about the object which is hit.
	var hit : RaycastHit;					
	var range = Mathf.Infinity;
	
	// will check if the ray can hit the object i.e. whether it is in its range.
	if(Physics.Raycast(ray , hit, range))          
	{
	//  will create an object of bullet each time fire button is clicked.
//		Instantiate(sparks, hit.point, Quaternion.identity);   
		 	
		sparks.transform.position = hit.point;
		
		sparks.particleEmitter.Emit();
		sparks.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		sparks.particleEmitter.emit = false;
		
		
		if(hit.transform.gameObject.tag == "enemy")
		{
			eHealth.show = true;
			hit.transform.GetComponent(enemyHealth).health -= damage;   
			scoreUpdater.score += 3;
		}
	}
}


@script RequireComponent(AudioSource);