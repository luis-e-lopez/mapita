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

	void Start () {
		position = spline.GetPoint2(progress);
		transform.localPosition = position;
		constantSpeed = speed;

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
		}

		if (goingForward) {
			progress += Time.fixedDeltaTime * constantSpeed;
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
			progress -= Time.fixedDeltaTime * constantSpeed;
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}
		}

		position = spline.GetPoint2(progress);
		position.Set (position.x, position.y, position.z - .05f);
		Vector3 velocity = spline.GetVelocity (progress);

		if (collidersCount == 1 || collidersCount == 3) {
			//Debug.Log ("Distance: " + Vector3.Distance (col.transform.position, transform.position));
			constantSpeed = speed * (Vector3.Distance (col.transform.position, position) / velocity.magnitude);
		} else if (collidersCount == 2) {
			constantSpeed = speed * (.1f / velocity.magnitude);

			float newDistance = Vector3.Distance (col.transform.position, position);
			//Debug.Log ("Distance: " + newDistance);
			if (lastDistance < newDistance) {
				constantSpeed = 0;
				accumulatedStopTime = 0;
				collidersCount++;
				//Debug.Log ("STOPS");
				//Debug.Log ("STOPS AT STATION: " + currentStation);
				setPassengerLocation();
				openDoors();
			}
			lastDistance = newDistance;
		} else {
			constantSpeed = speed * (1f / velocity.magnitude);
		}

		transform.localPosition = position;
		if (lookForward) {
			transform.LookAt (position + spline.GetDirection(progress));
		}
		/*
		Debug.Log ("Dir: " + position.magnitude);
		transform.Translate (Vector2.up * speed * Time.fixedDeltaTime, Space.World);
		transform.Rotate (Vector3.forward * spline.GetDirection2(progress).magnitude * Time.fixedDeltaTime);
		*/
		if (passenger != null) {
			//passenger.transform.position = position;
			passenger.gameObject.transform.position = new Vector3 (position.x, position.y, position.z - .1f);
		}
	}

	void OnTriggerEnter(Collider col) {

		if ((this.gameObject.tag == "Local" && (col.tag == "Local Station" || col.tag == "Local and Express Station")) ||
			(this.gameObject.tag == "Express" && col.tag == "Local and Express Station")) {
			
			if (collidersCount == 0) {
				// start decreasing speed
				//Debug.Log ("ENTERS");
				//Debug.Log ("Starts decreasing speed. Position is " + transform.position + ", Col Position is " + col.transform.position);
				//Debug.Log ("Distance: " + Vector3.Distance (col.transform.position, transform.position));
				this.col = col;
				collidersCount++;
			} else if (collidersCount == 1) {
				//Debug.Log ("ENTERS STATION");
				lastDistance = Vector3.Distance (col.transform.position, transform.position);
				currentStation = col.gameObject.name;
				//constantSpeed = 0;
				//accumulatedStopTime = 0;
				collidersCount++;
			} else {
				// do nothing
				collidersCount = 0;
			}

		} /*else {
			constantSpeed = 0;
			accumulatedStopTime = 0;
		}*/
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
		passenger.gameObject.transform.position = new Vector3 (position.x, position.y, position.z - .1f);
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
