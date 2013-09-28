#pragma strict

var target : Transform;
var finished = true;

function Start () {
	target.light.enabled = false;
	
	guiText.text = "Level 1 : The Raid\n\nKill all the defaulters that have taken control on this part of your brain!!\n You have to FIND them, CHASE them and KILL them!\n\nHint: The enemies are much afraid of you and will never try to cross your way. \nYou can easily kill them if you can get them stuck in some place.";
	
	yield WaitForSeconds(7);
	guiText.text = "";
	target.light.enabled = true;		
}

function Update () {	
	if(GameObject.FindGameObjectsWithTag("enemy").Length == 0)
	{
		target.light.enabled = false;	
		guiText.text = "Congratulations!\nYou have successfully killed all the enemies and have \ntaken control over this part of brain.Lets proceed to the next level!";
	}
}