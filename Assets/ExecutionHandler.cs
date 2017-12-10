using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class ExecutionHandler : MonoBehaviour {

	public Button playButton;
	public Button restartButton;
	public Button pauseButton;
	public Image pointer;

	private PlayerBehaviour player;

	private List<string> actions = new List<string> () { "HOP-ON", "HOP-OFF", "GO TO", "IF" };
	private List<string> conditions = new List<string> () { "station", "line", "train", "train direction" };
	private List<string> stations = new List<string> () { "Station1", "Station2", "Station3", "Station4", "Station5", "Station6", "Station7", "Station8" };
	private List<string> lines = new List<string> () { "Line Orange", "Line Red", "Line Yellow", "Line Green" };
	private List<string> trains = new List<string> () { "Local", "Express" };
	private List<string> directions = new List<string> () { "Lower Manhattan", "Upper Manhattan", "Queens" };

	private float lineOffset = 54f;
	private float initialLineOffset = -28f;
	private int lastLineNumber = 0;

	private bool done = false;
	private bool secondDone = false;

	// Use this for initialization
	void Start () {

		playButton.onClick.AddListener (delegate {
			play();
		});

		restartButton.onClick.AddListener (delegate {
			restart();
		});

		pauseButton.onClick.AddListener (delegate {
			pause();
		});

		player = FindObjectOfType(typeof(PlayerBehaviour)) as PlayerBehaviour;
		init ();
	}

	void Update() {

		int lineNumber = player.getCurrentLine ();
		if (lineNumber != lastLineNumber) {
			pointer.transform.localPosition = new Vector3(pointer.transform.localPosition.x, initialLineOffset - (lineOffset * (float)lineNumber));
			lastLineNumber = lineNumber;
		}
	}

	private void init() {
		pointer.transform.localPosition = new Vector3(pointer.transform.localPosition.x, .0f);
	}

	private void play () {

		int childCount = this.transform.childCount;
		string[][] steps = new string[childCount][];
		for (int i = 0; i < childCount; i++) {
			Transform command = this.transform.GetChild (i);

			if (command.tag == actions [0]) { // HOP-ON
				steps [i] = new string[] { actions [0] };
			} else if (command.tag == actions [1]) { // HOP-OFF
				steps [i] = new string[] { actions [1] };
			} else if (command.tag == actions [2]) { // GO TO
				string lineNumber = getLineNumber(command);
				steps [i] = new string[] { actions [2], lineNumber };
			} else if (command.tag == actions [3]) { // IF
				string condition = getCondition(command);
				string value = getValue (command);
				string lineNumber = getLineNumber(command);
				steps [i] = new string[] { actions [3], condition, value, lineNumber };
			}
		}

		for (int i = 0; i < steps.Length; i++) {
			string command = "";
			string[] step = steps [i];
			for (int j = 0; j < step.Length; j++) {
				command += " " + step [j];
			}
			Debug.Log (command);
		}

		player.setCommands (steps);
		player.initExecution ();
		GlobalProperties.isPaused = false;
	}

	private void restart() {

		GlobalProperties.isPaused = true;
		SplineWalker2[] allTrains = FindObjectsOfType(typeof(SplineWalker2)) as SplineWalker2[];

		foreach (SplineWalker2 train in allTrains) {
			train.restart ();
		}

		SplineDecorator4[] lines = FindObjectsOfType(typeof(SplineDecorator4)) as SplineDecorator4[];
		foreach (SplineDecorator4 line in lines) {
			line.restart ();
		}

		player.restart ();
		init ();
	}

	private void pause(){
		GlobalProperties.isPaused = true;
	}

	private string getLineNumber(Transform command) {

		for (int i = 0; i < command.childCount; i++) {
			GameObject child = command.GetChild (i).gameObject;
			InputField input = child.GetComponent<InputField> ();
			if (input != null) {
				if (input.text == "") {
					return "0";
				} else {
					return input.text;
				}
			}
		}
		return null;
	}

	private string getCondition(Transform command) {

		for (int i = 0; i < command.childCount; i++) {
			GameObject child = command.GetChild (i).gameObject;
			Dropdown dropdown = child.GetComponent<Dropdown> ();
			if (dropdown != null) {
				return dropdown.options[dropdown.value].text;
			}
		}
		return conditions[0];
	}

	private string getValue(Transform command) {

		for (int i = 0; i < command.childCount; i++) {
			GameObject child = command.GetChild (i).gameObject;
			Dropdown dropdown = child.GetComponent<Dropdown> ();
			if (dropdown != null && child.name == "Value") {
				return dropdown.options[dropdown.value].text;
			}
		}
		return "";
	}
}
