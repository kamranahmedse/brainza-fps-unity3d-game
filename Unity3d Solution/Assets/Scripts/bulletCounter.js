#pragma strict

static var bullets : int;
function Start () {

}

function Update () {
	guiText.text = "" + "Bullets: " + bullets.ToString() + "/100";
}