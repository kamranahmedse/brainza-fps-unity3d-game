#pragma strict
static var score : int;
function Start () {

}

function Update () 
{
	guiText.text = "Score  : " + score.ToString();
}