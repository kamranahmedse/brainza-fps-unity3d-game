#pragma strict

function Start () {

}

function Update () {
	// to count the number of objects:
	var enemyCount : int = GameObject.FindGameObjectsWithTag("enemy").Length;
	guiText.text = "Enemies: " + enemyCount + "/3";
}
