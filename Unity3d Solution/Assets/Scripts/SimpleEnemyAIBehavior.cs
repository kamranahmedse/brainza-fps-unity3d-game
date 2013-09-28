using UnityEngine;

using System.Collections;

 

public class SimpleEnemyAIBehavior : MonoBehaviour {

    

    //Inspector initiated variables. Defaults are set for ease of use.

    public bool on = true; //Is the AI active? this can be used to place pre-set enemies in you scene.

    public bool canFly = false; //Flying alters float behavior to ignore gravity. The enemy will fly up or down only to sustain floatHeight level.

    public float floatHeight = 0.0f; //If it can fly/hover, you need to let the AI know how high off the ground it should be.

    public bool runAway = false; //Is it the goal of this AI to keep it's distance? If so, it needs to have runaway active.

    public bool runTo = false; //Opposite to runaway, within a certain distance, the enemy will run toward the target.

    public float runDistance = 25.0f; //If the enemy should keep its distance, or charge in, at what point should they begin to run?

    public float runBufferDistance = 50.0f; //Smooth AI buffer. How far apart does AI/Target need to be before the run reason is ended.

    public int walkSpeed = 10; //Standard movement speed.

    public int runSpeed = 15; //Movement speed if it needs to run.

    public int randomSpeed = 10; //Movement speed if the AI is moving in random directions.

    public float rotationSpeed = 20.0f; //Rotation during movement modifier. If AI starts spinning at random, increase this value. (First check to make sure it's not due to visual radius limitations)

    public float visualRadius = 100.0f; //How close does the player need to be to be seen by the enemy? Set to 0 to remove this limitation.

    public float moveableRadius = 200.0f; //If the player is too far away, the AI will auto-matically shut down. Set to 0 to remove this limitation.

    public float attackRange = 10.0f; //How close does the enemy need to be in order to attack?

    public float attackTime = 0.50f; //How frequent or fast an enemy can attack (cool down time).

    public bool useWaypoints = false; //If true, the AI will make use of the waypoints assigned to it until over-ridden by another functionality.

    public bool reversePatrol = true; //if true, patrol units will walk forward and backward along their patrol.

    public Transform[] waypoints; //define a set path for them to follow.

    public bool pauseAtWaypoints = false; //if true, patrol units will pause momentarily at each waypoint as they reach them.

    public float pauseMin = 1.0f; //If pauseAtWaypoints is true, the unit will pause momentarily for minmum of this time.

    public float pauseMax = 3.0f; //If pauseAtWaypoints is true, the unit will pause momentarily formaximum of this time.

    public float huntingTimer = 5.0f; //Search for player timer in seconds. Minimum of 0.1

    public bool estimateElevation = false; //This implements a pause between raycasts for heights and guestimates the need to move up/down in height based on the previous raycast.

    public float estRayTimer = 1.0f; //The amount of time in seconds between raycasts for gravity and elevation checks.

    public bool requireTarget = true; //Waypoint ONLY functionality (still can fly and hover).

    public Transform target; //The target, or whatever the AI is looking for.

    

    

    //private script handled variables

    private bool initialGo = false; //AI cannot function until it is initialized.

    private bool go = true; //An on/off override variable

    private Vector3 lastVisTargetPos; //Monitor target position if we lose sight of target. provides semi-intelligent AI.

    CharacterController characterController; //CC used for enemy movement and etc.

    private bool playerHasBeenSeen = false; //An enhancement to how the AI functions prior to visibly seeing the target. Brings AI to life when target is close, but not visible.

    private bool enemyCanAttack = false; //Used to determine if the enemy is within range to attack, regardless of moving or not.

    private bool enemyIsAttacking = false; //An attack interuption method.

    private bool executeBufferState = false; //Smooth AI buffer for runAway AI. Also used as a speed control variable.

    private bool walkInRandomDirection = false; //Speed control variable.

    private float lastShotFired; //Used in conjuction with attackTime to monitor attack durations.

    private float lostPlayerTimer; //Used for hunting down the player.

    private bool targetIsOutOfSight; //Player tracking overload prevention. Makes sure we do not call the same coroutines over and over.

    private Vector3 randomDirection; //Random movement behaviour setting.

    private float randomDirectionTimer; //Random movement behaviour tracking.

    private float gravity = 20.0f; //force of gravity pulling the enemy down.

    private float antigravity = 2.0f; //force at which floating/flying enemies repel

    private float estHeight = 0.0f; //floating/flying creatures using estimated elevation use this to estimate height necessities and gravity impacts.

    private float estGravityTimer = 0.0f; //floating/flying creatures using estimated elevation will use this to actually monitor time values.

    private int estCheckDirection = 0; //used to determine if AI is falling or not when estimating elevation.

    private bool wpCountdown = false; //used to determine if we're moving forward or backward through the waypoints.

    private bool monitorRunTo = false; //when AI is set to runTo, they will charge in, and then not charge again to after far enough away.

    private int wpPatrol = 0; //determines what waypoint we are heading toward.

    private bool pauseWpControl; //makes sure unit pauses appropriately.

    private bool smoothAttackRangeBuffer = false; //for runAway AI to not be so messed up by their visual radius and attack range.

    

    

 

    //---Starting/Initializing functions---//

    void Start() {

        StartCoroutine(Initialize()); //co-routine is used incase you need to interupt initiialization until something else is done.

    }

    

    IEnumerator Initialize() {

        if ((estimateElevation) && (floatHeight > 0.0f)) {

            estGravityTimer = Time.time;

        }

        characterController = gameObject.GetComponent<CharacterController>();

        initialGo = true;

        yield return null;

    }

    

    

    

    //---Main Functionality---//

    void Update () {
		/*
		float range = 2.0f;
		Vector3 fwd = Vector3.forward;
		if(Physics.Raycast(transform.position,  fwd, range))
		{
			transform.Rotate (0,90,0);
			if(Physics.Raycast(transform.position,  fwd, range))
			{
				transform.Rotate (0,90,0);
				if(Physics.Raycast(transform.position,  fwd, range))
				{
					characterController.Move (fwd * 20 * Time.deltaTime);
				}
				
			}
			else
			{
				characterController.Move (fwd * 20 * Time.deltaTime);
			}
		}*/
		

        if (!on || !initialGo) {

            return;

        } else {

            AIFunctionality();

        }

    }

    

    void AIFunctionality() {

        if ((!target) && (requireTarget)) {

            return; //if no target was set and we require one, AI will not function.

        }

        

        //Functionality Updates

        lastVisTargetPos = target.position; //Target tracking method for semi-intelligent AI

        Vector3 moveToward = lastVisTargetPos - transform.position; //Used to face the AI in the direction of the target

        Vector3 moveAway = transform.position - lastVisTargetPos; //Used to face the AI away from the target when running away

        float distance = Vector3.Distance(transform.position, target.position);

        

        if (go) {

            MonitorGravity();

        }

        

        if (!requireTarget) {

            //waypoint only functionality

            Patrol();

        } else if (TargetIsInSight ()) {

            if (!go) { //useWaypoints is false and the player has exceeded moveableRadius, shutdown AI until player is near.

                return;

            }

            

            if ((distance > attackRange) && (!runAway) && (!runTo)) {

                enemyCanAttack = false; //the target is too far away to attack

                MoveTowards (moveToward); //move closer

            } else if ((smoothAttackRangeBuffer) && (distance > attackRange+5.0f)) {

                smoothAttackRangeBuffer = false;

                WalkNewPath();

            }else if ((runAway || runTo) && (distance > runDistance) && (!executeBufferState)) {

                //move in random directions.

                if (monitorRunTo) {

                    monitorRunTo = false;

                }

                if (runAway) {

                    WalkNewPath ();

                } else {

                    MoveTowards (moveToward);

                }

            } else if ((runAway || runTo) && (distance < runDistance) && (!executeBufferState)) { //make sure they do not get too close to the target

                //AHH! RUN AWAY!...  or possibly charge :D

                enemyCanAttack = false; //can't attack, we're running!

                if (!monitorRunTo) {

                    executeBufferState = true; //smooth buffer is now active!

                }

                walkInRandomDirection = false; //obviously we're no longer moving at random.

                if (runAway) {

                    MoveTowards (moveAway); //move away

                } else {

                    MoveTowards (moveToward); //move toward

                }

            } else if (executeBufferState && ((runAway) && (distance < runBufferDistance)) || ((runTo) && (distance > runBufferDistance))) {

                //continue to run!

                if (runAway) {

                    MoveTowards (moveAway); //move away

                } else {

                    MoveTowards (moveToward); //move toward

                }

            } else if ((executeBufferState) && (((runAway) && (distance > runBufferDistance)) || ((runTo) && (distance < runBufferDistance)))) {

                monitorRunTo = true; //make sure that when we have made it to our buffer distance (close to user) we stop the charge until far enough away.

                executeBufferState = false; //go back to normal activity

            } 

 

            //start attacking if close enough

            if ((distance < attackRange) || ((!runAway && !runTo) && (distance < runDistance))) {

                if (runAway) {

                    smoothAttackRangeBuffer = true;

                }

                if (Time.time > lastShotFired + attackTime) {

                    StartCoroutine(Attack());

                }

            } 

            

        } else if ((playerHasBeenSeen) && (!targetIsOutOfSight) && (go)) {

            lostPlayerTimer = Time.time + huntingTimer;

            StartCoroutine(HuntDownTarget(lastVisTargetPos));

        } else if (useWaypoints) {

            Patrol();

        } else if (((!playerHasBeenSeen) && (go)) && ((moveableRadius == 0) || (distance < moveableRadius))){

            //the idea here is that the enemy has not yet seen the player, but the player is fairly close while still not visible by the enemy

            //it will move in a random direction continuously altering its direction every 2 seconds until it does see the player.

            WalkNewPath();

        }

    }

    

    //attack stuff...

    IEnumerator Attack() {

        enemyCanAttack = true;

        

        if (!enemyIsAttacking) {

            enemyIsAttacking = true;

            while (enemyCanAttack) {

                lastShotFired = Time.time;

                //implement attack variables here

                yield return new WaitForSeconds(attackTime);

            }

        }

    }

    

    

    

    //----Helper Functions---//

    //verify enemy can see the target

    bool TargetIsInSight () {

        //determine if the enemy should be doing anything other than standing still

        if ((moveableRadius > 0) && (Vector3.Distance(transform.position, target.position) > moveableRadius)) {

            go = false;

        } else {

            go = true;

        }

        

        //then lets make sure the target is within the vision radius we allowed our enemy

        //remember, 0 radius means to ignore this check

        if ((visualRadius > 0) && (Vector3.Distance(transform.position, target.position) > visualRadius)) {

            return false;

        }

        

        //Now check to make sure nothing is blocking the line of sight

        RaycastHit sight;

        if (Physics.Linecast(transform.position, target.position, out sight)) {

            if (!playerHasBeenSeen && sight.transform == target) {

                playerHasBeenSeen = true;

            }

            return sight.transform == target;

        } else {

            return false;

        }

    }

    

    //target tracking

    IEnumerator HuntDownTarget (Vector3 position) {

        //if this function is called, the enemy has lost sight of the target and must track him down!

        //assuming AI is not too intelligent, they will only move toward his last position, and hope they see him

        //this can be fixed later to update the lastVisTargetPos every couple of seconds to leave some kind of trail

        targetIsOutOfSight = true;

        while (targetIsOutOfSight) {

            Vector3 moveToward = position - transform.position;

            MoveTowards (moveToward);

            

            //check if we found the target yet

            if (TargetIsInSight ()) {

                targetIsOutOfSight = false;

                break;

            }

            

            //check to see if we should give up our search

            if (Time.time > lostPlayerTimer) {

                targetIsOutOfSight = false;

                playerHasBeenSeen = false;

                break;

            }

            yield return null;

        }

    }

    

    void Patrol () {

        if (pauseWpControl) {

            return;

        }

        Vector3 destination = CurrentPath();

        Vector3 moveToward = destination - transform.position;

        float distance = Vector3.Distance(transform.position, destination);

        MoveTowards (moveToward);

        if (distance <= 1.5f+floatHeight) {// || (distance < floatHeight+1.5f)) {

            if (pauseAtWaypoints) {

                if (!pauseWpControl) {

                    pauseWpControl = true;

                    StartCoroutine(WaypointPause());

                }

            } else {

                NewPath();

            }

        }

    }

    

    IEnumerator WaypointPause () {

        yield return new WaitForSeconds(Random.Range(pauseMin, pauseMax));

        NewPath();

        pauseWpControl = false;

    }

    

    Vector3 CurrentPath () {

        return waypoints[wpPatrol].position;

    }

    

    void NewPath () {

        if (!wpCountdown) {

            wpPatrol++;

            if (wpPatrol >= waypoints.GetLength(0)) {

                if (reversePatrol) {

                    wpCountdown = true;

                    wpPatrol -= 2;

                } else {

                    wpPatrol = 0;

                }

            }

        } else if (reversePatrol) {

            wpPatrol--;

            if (wpPatrol < 0) {

                wpCountdown = false;

                wpPatrol = 1;

            }

        }

    }

    

    //random movement behaviour

    void WalkNewPath () {
		
	    RaycastHit hit;
		float range = Mathf.Infinity;

        if (!walkInRandomDirection) {

            walkInRandomDirection = true;

            if (!playerHasBeenSeen) {

                randomDirection = new Vector3(Random.Range(-0.15f,0.15f),0,Random.Range(-0.15f,0.15f));

            } else {

                randomDirection = new Vector3(Random.Range(-0.5f,0.5f),0,Random.Range(-0.5f,0.5f));

            }

            randomDirectionTimer = Time.time;

        } else if (walkInRandomDirection) {

            MoveTowards (randomDirection);

        }

        

        if ((Time.time - randomDirectionTimer) > 2) {

            //choose a new random direction after 2 seconds

            walkInRandomDirection = false;

        }

    }

    

    //standard movement behaviour

    void MoveTowards (Vector3 direction) {

        direction.y = 0;

        int speed = walkSpeed;

        

        if (walkInRandomDirection) {

            speed = randomSpeed;

        }

        

        if (executeBufferState) {

            speed = runSpeed;

        }

        

        //rotate toward or away from the target

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

 

        //slow down when we are not facing the target

        Vector3 forward = transform.TransformDirection(Vector3.forward);

        float speedModifier = Vector3.Dot(forward, direction.normalized);

        speedModifier = Mathf.Clamp01(speedModifier);

            

        //actually move toward or away from the target

        direction = forward * speed * speedModifier;

        if ((!canFly) && (floatHeight <= 0.0f)) {

            direction.y -= gravity;

        }

        characterController.Move(direction * Time.deltaTime); 

    }

    

    //continuous gravity checks

    void MonitorGravity () {

        Vector3 direction = new Vector3(0, 0, 0);

        

        if ((!canFly) && (floatHeight > 0.0f)) {

            //we need to make sure our enemy is floating.. using evil raycasts! bwahahahah!

            if ((estimateElevation) && (estRayTimer > 0.0f)) {

                if (Time.time > estGravityTimer) {

                    RaycastHit floatCheck;

                    if (Physics.Raycast(transform.position, -Vector3.up, out floatCheck)) {

                        if (floatCheck.distance < floatHeight-0.5f) {

                            estCheckDirection = 1;

                            estHeight = floatHeight - floatCheck.distance;

                        } else if (floatCheck.distance > floatHeight+0.5f) {

                            estCheckDirection = 2;

                            estHeight = floatCheck.distance - floatHeight;

                        } else {

                            estCheckDirection = 3;

                        }

                    } else {

                        estCheckDirection = 2;

                        estHeight = floatHeight*2;

                    }

                    estGravityTimer = Time.time + estRayTimer;

                }

            

                switch(estCheckDirection) {

                    case 1:

                        direction.y += antigravity;

                        estHeight -= direction.y * Time.deltaTime;

                        break;

                    case 2:

                        direction.y -= gravity;

                        estHeight -= direction.y * Time.deltaTime;

                        break;

                    default:

                        //do nothing

                        break;

                }

                

            } else {

                RaycastHit floatCheck;

                if (Physics.Raycast(transform.position, -Vector3.up, out floatCheck, floatHeight+1.0f)) {

                    if (floatCheck.distance < floatHeight) {

                        direction.y += antigravity;

                    }

                } else {

                    direction.y -= gravity;

                }

            }

        } else {

            //bird like creature! Again with the evil raycasts! :p

            if ((estimateElevation) && (estRayTimer > 0.0f)) {

                if (Time.time > estGravityTimer) {

                    RaycastHit floatCheck;

                    if (Physics.Raycast(transform.position, -Vector3.up, out floatCheck)) {

                        if (floatCheck.distance < floatHeight-0.5f) {

                            estCheckDirection = 1;

                            estHeight = floatHeight - floatCheck.distance;

                        } else if (floatCheck.distance > floatHeight+0.5f) {

                            estCheckDirection = 2;

                            estHeight = floatCheck.distance - floatHeight;

                        } else {

                            estCheckDirection = 3;

                        }

                    }

                    estGravityTimer = Time.time + estRayTimer;

                }

                    

                switch(estCheckDirection) {

                    case 1:

                        direction.y += antigravity;

                        estHeight -= direction.y * Time.deltaTime;

                        break;

                    case 2:

                        direction.y -= antigravity;

                        estHeight -= direction.y * Time.deltaTime;

                        break;

                    default:

                        //do nothing

                        break;

                }

                    

            } else {

                RaycastHit floatCheck;

                if (Physics.Raycast(transform.position, -Vector3.up, out floatCheck)) {

                    if (floatCheck.distance < floatHeight-0.5f) {

                        direction.y += antigravity;

                    } else if (floatCheck.distance > floatHeight+0.5f) {

                        direction.y -= antigravity;

                    }

                }

            }

        }

        

        if ((!estimateElevation) || ((estimateElevation) && (estHeight >= 0.0f))) {

            characterController.Move(direction * Time.deltaTime);

        }

    }

    

}