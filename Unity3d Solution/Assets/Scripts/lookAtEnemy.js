#pragma strict

var hit : RaycastHit;

function Start () {

}

function Update () {
	if (Physics.Raycast (transform.position, transform.forward, hit, Mathf.Infinity)) 
	{
		if(hit.collider.tag == "enemy")
		{
			var life = hit.collider.gameObject.GetComponent(enemyHealth).health;
			eHealth.show = true;
			eHealth.health = life;
		}        
    }

}