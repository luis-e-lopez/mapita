using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class PlayerBehaviour : MonoBehaviour {

	private SplineWalker3 splineWalker3;
	private SplineWalker2 splineWalker2;
	private SplineWalker3[] walkers3;
	private SplineWalker2[] walkers2;
	private List<SplineWalker2> trainsOnStation = new List<SplineWalker2> ();
	private bool onBoard = false;
	private string[] stops; 
	private int stopIndex = 0;
	private string[][] steps;
	private int step = 0;
	private string[] actions;
	private string[] ifConditions;
	private string[] stations;
	private string[] lines;
	private string[] directions;
	private string[] trains;
	private int pausedCount;
	private bool paused = false;
	private bool suspendedExecution = false;

	private int count;
	private string currentLocation;

	// Use this for initialization
	void Start () {
		splineWalker3 = null;
		splineWalker2 = null;
		walkers3 = FindObjectsOfType(typeof(SplineWalker3)) as SplineWalker3[];
		walkers2 = FindObjectsOfType(typeof(SplineWalker2)) as SplineWalker2[];
		stops = new string[] { "Station6", "Station3", "Station1", "Station7" };

		//actions
		actions = new string[] { "HOP-ON", "HOP-OFF", "GO TO", "IF" };

		//if conditions
		ifConditions = new string[] { "station", "line", "train", "train direction" };

		//stations
		stations = new string[] { "Station1", "Station2", "Station3", "Station4", "Station5", "Station6", "Station7", "Station8" };

		//lines
		lines = new string[] { "Line Orange", "Line Red", "Line Yellow", "Line Green" };

		//directions
		directions = new string[] { "Lower Manhattan", "Upper Manhattan", "Queens" };

		//trians
		trains = new string[] { "Local", "Express" };

		initPosition ();
	}
	
	// Update is called once per frame
	void Update () {

		if (GlobalProperties.isPaused) {
			return;
		}

		if (count < steps.Length) {
			if (trainsOnStation.Count > 0) {
				SplineWalker2 train = trainsOnStation [0];
				if (train.doorsAreOpened ()) {
					executeCommand (steps [count], train);
					StartCoroutine (stopTime (.5f));
				} else {
					trainsOnStation.RemoveAt (0);
				}
			}
		}
	}

	private void initPosition (){
		count = -1;
		this.transform.position = new Vector3 (6.54f, 1.06f, -0.1f);
		currentLocation = stations[5];
	}

	public void initExecution() {
		count = 0;
	}

	IEnumerator stopTime(float seconds)
	{
		//print(Time.time);
		//Time.timeScale = 0;
		GlobalProperties.isPaused = true;
		yield return new WaitForSecondsRealtime(seconds);
		GlobalProperties.isPaused = false;
		//Time.timeScale = 1;
		//print(Time.time);
	}

	private void executeCommand(string[] command, SplineWalker2 walker) {

		if (command [0] == actions [0]) { // HOP-ON
			//Debug.Log ("Line " + count + ": " + command [0]);
			hopOn (walker);
		} else if (command [0] == actions [1]) { // HOP-OFF
			//Debug.Log ("Line " + count + ": " + command [0]);
			hopOff ();
			trainsOnStation.RemoveAt(0);
		} else if (command [0] == actions [2]) { // GO TO
			//Debug.Log ("Line " + count + ": " + command [0] + " " + command [1]);
			goTo (int.Parse (command [1]) - 1);
			//suspendExecution ();
			trainsOnStation.RemoveAt(0);
			return;
		} else if (command [0] == actions [3]) { // IF
			//Debug.Log ("Line " + count + ": " + command [0] + " " + command [1] + " " + command [2] + " go to " + command[3]);
			if (executeIf (command [1], command [2], walker)) {
				goTo (int.Parse(command[3]) - 1);
				return;
			}
		}
		count++;
	}

	private bool goTo(int lineNumber) {
		if (lineNumber < steps.Length) {
			count = lineNumber;
			return true;
		}
		return false;
	}

	private bool executeIf(string ifCondition, string value, SplineWalker2 walker) {

		if (walker == null) {
			return false;
		}

		if (ifCondition == ifConditions [0]) { // station is
			if (walker.stationName () == value) {
				return true;
			}
		} else if (ifCondition == ifConditions [1]) {  // line is
			if (walker.getLine() == value) {
				return true;
			}
		} else if (ifCondition == ifConditions [2]) { // train is
			if (walker.gameObject.tag == value) {
				return true;
			}
		} else if (ifCondition == ifConditions [3]) { // train direction is
			if (walker.getDirection() == value) {
				return true;
			}
		}
		return false;
	}

	private bool hopOn(string stationName, string line, string direction, string train) {
		if (!onBoard) {
			foreach (SplineWalker3 walker in walkers3) {
				if (walker.getLine() == line && walker.getDirection() == direction && walker.doorsAreOpened () && walker.stationName () == stationName && walker.gameObject.tag == train) {
					hopOn (walker);
					Debug.Log ("HOP-ON " + stationName + " SUCCESS in " + walker.gameObject.name + " WITH DIRECTION TO " + walker.getDirection() + " IN LINE " + walker.getLine() + " TRAIN TYPE " + train);
					return true;
				}
			}
			foreach (SplineWalker2 walker in walkers2) {
				if (walker.getLine() == line && walker.getDirection() == direction && walker.doorsAreOpened () && walker.stationName () == stationName  && walker.gameObject.tag == train) {
					hopOn (walker);
					Debug.Log ("HOP-ON " + stationName + " SUCCESS in " + walker.gameObject.name + " WITH DIRECTION TO " + walker.getDirection() + " IN LINE " + walker.getLine() + " TRAIN TYPE " + train);
					return true;
				}
			}
		}
		return false;
	}

	private bool hopOn(string stationName, string line, string direction) {
		if (!onBoard) {
			foreach (SplineWalker3 walker in walkers3) {
				if (walker.getLine() == line && walker.getDirection() == direction && walker.doorsAreOpened () && walker.stationName () == stationName) {
					hopOn (walker);
					Debug.Log ("HOP-ON " + stationName + " SUCCESS in " + walker.gameObject.name + " WITH DIRECTION TO " + walker.getDirection() + " IN LINE " + walker.getLine());
					return true;
				}
			}
			foreach (SplineWalker2 walker in walkers2) {
				if (walker.getLine() == line && walker.getDirection() == direction && walker.doorsAreOpened () && walker.stationName () == stationName) {
					hopOn (walker);
					Debug.Log ("HOP-ON " + stationName + " SUCCESS in " + walker.gameObject.name + " WITH DIRECTION TO " + walker.getDirection() + " IN LINE " + walker.getLine());
					return true;
				}
			}
		}
		return false;
	}

	private bool hopOn(string stationName) {
		if (!onBoard) {
			//Debug.Log ("HOP-ON " + stationName);
			foreach (SplineWalker3 walker in walkers3) {
				if (walker.doorsAreOpened () && walker.stationName () == stationName) {
					hopOn (walker);
					Debug.Log ("HOP-ON " + stationName + " SUCCESS in " + walker.gameObject.name + " WITH DIRECTION TO " + walker.getDirection() + " IN LINE " + walker.getLine());
					return true;
				}
			}
			foreach (SplineWalker2 walker in walkers2) {
				if (walker.doorsAreOpened () && walker.stationName () == stationName) {
					hopOn (walker);
					Debug.Log ("HOP-ON " + stationName + " SUCCESS in " + walker.gameObject.name + " WITH DIRECTION TO " + walker.getDirection() + " IN LINE " + walker.getLine());
					return true;
				}
			}
		}
		return false;
	}

	private bool hopOn(SplineWalker3 walker) {
		walker.addPassenger (this);
		onBoard = true;
		splineWalker3 = walker;
		//suspendExecution ();
		removeAllTrains();
		return true;
	}

	private bool hopOn(SplineWalker2 walker) {
		walker.addPassenger (this);
		onBoard = true;
		splineWalker2 = walker;
		//suspendExecution ();
		removeAllTrains();
		return true;
	}

	private bool hopOff(string stationName) {
		if (splineWalker2 != null && splineWalker2.stationName() == stationName) {
			if (splineWalker2.doorsAreOpened ()) {
				splineWalker2.removePassenger ();
				Debug.Log ("HOP-OFF " + stationName + " SUCCESS in " + splineWalker2.gameObject.name + " WITH DIRECTION TO " + splineWalker2.getDirection() + " IN LINE " + splineWalker2.getLine());
				splineWalker2 = null;
				onBoard = false;
				return true;
			}
		} else if (splineWalker3 != null && splineWalker3.stationName() == stationName) {
			if (splineWalker3.doorsAreOpened ()) {
				splineWalker3.removePassenger ();
				Debug.Log ("HOP-OFF " + stationName + " SUCCESS in " + splineWalker3.gameObject.name + " WITH DIRECTION TO " + splineWalker3.getDirection() + " IN LINE " + splineWalker3.getLine());
				splineWalker3 = null;
				onBoard = false;
				return true;
			}
		}
		return false;
	}

	private bool hopOff() {
		splineWalker2.removePassenger ();
		checkForOtherTrainsInStation (splineWalker2);
		splineWalker2 = null;
		onBoard = false;
		return true;
	}

	private void checkForOtherTrainsInStation(SplineWalker2 currentTrain) {
		foreach (SplineWalker2 train in walkers2) {
			if (train != currentTrain && train.stationName() == currentLocation && train.doorsAreOpened()) {
				trainsOnStation.Add (train);	
			}
		}
	}

	public void removeAllTrains() {
		trainsOnStation.Clear ();
	}

	public void suspendExecution() {
		suspendedExecution = true;
	}

	public void resumeExecution() {
		suspendedExecution = false;
	}

	public void setCurrentLocation(string location) {
		currentLocation = location;
	}

	public string getCurrentLocation() {
		return currentLocation;
	}

	public void addTrain(SplineWalker2 train) {
		if (splineWalker2 == null || splineWalker2 == train) {
			trainsOnStation.Add (train);
		}
	}

	public void setCommands(string[][] commands) {
		steps = commands;
	}

	public void restart() {
		initPosition ();
	}

	public int getCurrentLine() {
		return count;
	}

}
