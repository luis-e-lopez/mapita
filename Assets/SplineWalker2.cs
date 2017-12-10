using UnityEngine;
using AssemblyCSharp;

public class SplineWalker2 : MonoBehaviour {

	public BezierSpline spline;
	public float speed = 3f;
	public bool lookForward;
	public float rotationSpeed = 100f;
	public SplineWalkerMode mode;
	public float stopTime = 1f;
	public string forwardDirection = "";
	public string backwardDirection = "";
	public bool isSprite = false;

	private float progress;
	private bool goingForward = true;
	private Vector3 position;
	private float constantSpeed;
	private float accumulatedStopTime;
	private int collidersCount = 0;
	private Collider col;
	private float lastDistance = 0f;	
	private bool openedDoors = false;
	private string currentStation = null;
	private PlayerBehaviour passenger = null;
	private PlayerBehaviour player = null;

	private float acceleration;
	private float accelerationTime = 1.5f;
	private float currentAccelerationTime;
	private float stationProgress;
	private float startProgress;

	void Start () {
		position = spline.GetPoint(progress);
		transform.localPosition = position;
		constantSpeed = speed / spline.GetVelocity (progress).magnitude;

		player = (PlayerBehaviour)FindObjectOfType(typeof(PlayerBehaviour));
	}
	// Update is called once per frame
	void FixedUpdate () {

		if (GlobalProperties.isPaused) {
			return;
		}

		if (constantSpeed == 0) {
			accumulatedStopTime += Time.fixedDeltaTime;
			if (accumulatedStopTime < stopTime) {
				return;
			}
			closeDoors ();
			currentStation = null;
			recalculateAcceleration ();
			constantSpeed = 1f;
		}

		if (goingForward) {
			if (collidersCount == 1) {

				if (progress >= stationProgress) {
					accelerate (goingForward);
				} else {
					decelerate (goingForward);
				}

			} else {
				Vector3 velocity = spline.GetVelocity (progress); 
				constantSpeed = speed / velocity.magnitude;
				progress += Time.fixedDeltaTime * constantSpeed;
			}

			if (progress > 1f) {
				if (mode == SplineWalkerMode.Once) {
					progress = 1f;
				}
				else if (mode == SplineWalkerMode.Loop) {
					progress -= 1f;
				}
				else {
					progress = 2f - progress;
					goingForward = false;
				}
			}
		}
		else {
			if (collidersCount == 1) {

				if (progress > stationProgress) {
					decelerate (goingForward);
				} else {
					accelerate (goingForward);
				}

			} else {
				Vector3 velocity = spline.GetVelocity (progress); 
				constantSpeed = speed / velocity.magnitude;
				progress -= Time.fixedDeltaTime * constantSpeed;
			}
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}
		}

		position = spline.GetPoint(progress);
		transform.localPosition = position;
		if (lookForward) {
			transform.LookAt (position + spline.GetDirection(progress));
			if (isSprite) {
				transform.Rotate (-90f, 0f, 0f);
				transform.Rotate (0f, -90f, 0f);
			}
		}

		if (passenger != null) {
			//passenger.transform.position = position;
			passenger.gameObject.transform.position = new Vector3 (position.x, position.y, position.z);
		}
	}

	void OnTriggerEnter(Collider col) {

		if (collidersCount == 0) {
			if ((this.gameObject.tag == "Local" && (col.tag == "Local Station" || col.tag == "Local and Express Station")) ||
			   (this.gameObject.tag == "Express" && col.tag == "Local and Express Station")) {

				if (transform.name == "Train Red 2") {
					//Debug.Log ("Collided on " + col.name);
				}
				StationControl stationControl = col.transform.GetComponent<StationControl> ();
				if (stationControl.shouldStop (spline)) {
					stationProgress = stationControl.getPosition (spline);
					startProgress = progress;

					float velocity = ((stationProgress - startProgress) * 2f) / accelerationTime;
					velocity = (velocity < 0) ? velocity * -1f : velocity;
					acceleration = velocity / accelerationTime;
					currentAccelerationTime = accelerationTime;

					this.col = col;
					collidersCount = 1;

					if (transform.name == "Train Red") {
						//Debug.Log ("Distance " + (stationProgress - startProgress));
					}
				}

				/*if (collidersCount == 0) {
				this.col = col;
				collidersCount++;
			} else if (collidersCount == 1) {
				lastDistance = Vector3.Distance (col.transform.position, transform.position);
				currentStation = col.gameObject.name;
				collidersCount++;
			} else {
				collidersCount = 0;
			}*/

			} 
		}
	}

	/*void OnTriggerExit(Collider col) {

		if ((this.gameObject.tag == "Local" && (col.tag == "Local Station" || col.tag == "Local and Express Station")) ||
			(this.gameObject.tag == "Express" && col.tag == "Local and Express Station")) {

			StationControl stationControl = col.transform.GetComponent<StationControl> ();
			if (stationControl.shouldStop (spline)) {
				collidersCount = 0;
				//GlobalProperties.isPaused = true;
				//Debug.Log("Exit Collider!");
			}
		}
	}*/

	private void recalculateAcceleration() {

		StationControl stationControl = col.transform.GetComponent<StationControl> ();
		float d = stationControl.getColliderExitPosition (spline, goingForward);
		float velocity = ((stationProgress - d) * 2f) / accelerationTime;
		velocity = (velocity < 0) ? velocity * -1f : velocity;
		acceleration = velocity / accelerationTime;
	}

	private void accelerate(bool goingForward) {

		currentAccelerationTime += Time.fixedDeltaTime;
		if (currentAccelerationTime >= accelerationTime) {
			currentAccelerationTime = accelerationTime;
			collidersCount = 0;
			if (transform.name == "Train Red") {
				//Debug.Log ("Out of " + col.name + ", time: " + currentAccelerationTime + ", vel: " + (acceleration * currentAccelerationTime) + ", d: " + (acceleration * currentAccelerationTime / 2f) * currentAccelerationTime);
				//GlobalProperties.isPaused = true;
			}
		}

		float velocity = acceleration * currentAccelerationTime;
		float d = (velocity / 2f) * currentAccelerationTime;

		if (goingForward) {
			progress = stationProgress + d;
		} else {
			progress = stationProgress - d;
		}
	}

	private void decelerate(bool goingForward) {

		currentAccelerationTime -= Time.fixedDeltaTime;
		float velocity = acceleration * currentAccelerationTime;
		float d = (velocity / 2f) * currentAccelerationTime;

		if (goingForward) {
			progress = stationProgress - d;
		} else {
			progress = stationProgress + d;
		}

		if (currentAccelerationTime <= 0f || progress == stationProgress) {
			currentAccelerationTime = 0f;
			accumulatedStopTime = 0f;
			constantSpeed = 0f;
			if (transform.name == "Train Red 2") {
				//Debug.Log ("stopping on " + col.name);

			}
		}
	}

	private void openDoors() {
		openedDoors = true;
		/*if (passenger != null) {
			passenger.resumeExecution ();
		}*/
		if (currentStation == player.getCurrentLocation ()) {
			player.addTrain (this);
		}
	}

	private void closeDoors() {
		openedDoors = false;
		/*if (passenger != null) {
			passenger.suspendExecution ();
		}*/
	}

	public bool doorsAreOpened() {
		return openedDoors;
	}

	public string stationName() {
		return currentStation;
	}

	public void addPassenger(PlayerBehaviour player) {
		//Debug.Log ("Added passenger " + go.name);
		passenger = player;
		passenger.gameObject.transform.position = new Vector3 (position.x, position.y, position.z);
	}

	public void removePassenger() {
		//Debug.Log ("REMOVE passenger " + passenger.name);
		passenger = null;
	}

	public string getDirection() {
		return (goingForward) ? forwardDirection : backwardDirection;
	}

	public string getLine() {
		return spline.gameObject.tag;
	}

	private void setPassengerLocation() {
		if (passenger != null && currentStation != null) {
			passenger.setCurrentLocation (currentStation);
		}
	}

	public void restart (){
		progress = 0;
		goingForward = true;
		position = spline.GetPoint2(progress);
		transform.localPosition = position;
		constantSpeed = speed;
		collidersCount = 0;
		lastDistance = 0f;
		openedDoors = false;
		currentStation = null;
		passenger = null;
	}
}
