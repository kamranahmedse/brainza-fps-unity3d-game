#pragma strict


var health : float = 100;
//var show = false;

function Start() {
}

function Update() 
{
	//	eHealth.health = this.health;
		if(health <= 0)
		{
			Destroy(gameObject);
			eHealth.show = false;
			scoreUpdater.score += 10;
		}
}
/*
function healthDown(damage : int)
{
	Debug.Log(damage);
}
*/