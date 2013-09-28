#pragma strict
static var health : float = 100;
static var show = false;

function Start () {

}

function Update () {
	guiText.enabled = false;
	if(show)
	{	guiText.enabled = true;
		guiText.text = "Enemy Health: " + health + "/100";
	}
}