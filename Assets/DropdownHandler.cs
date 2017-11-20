using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour {

	private Dropdown dropdown1;
	private Dropdown dropdown2;

	private List<string> conditions = new List<string> () { "station", "line", "train", "train direction" };
	private List<string> stations = new List<string> () { "Station1", "Station2", "Station3", "Station4", "Station5", "Station6", "Station7", "Station8" };
	private List<string> lines = new List<string> () { "Line Orange", "Line Red", "Line Yellow", "Line Green" };
	private List<string> trains = new List<string> () { "Local", "Express" };
	private List<string> directions = new List<string> () { "Lower Manhattan", "Upper Manhattan", "Queens" };

	// Use this for initialization
	void Start () {

		int childCount = this.transform.childCount;
		for (int i = 0; i < childCount; i++) {
			Dropdown dropdown = this.transform.GetChild (i).GetComponent<Dropdown> ();
			if (dropdown != null) {
				if (dropdown1 == null) {
					dropdown1 = dropdown;
					//Debug.Log ("Dropdown 1 found");
				} else {
					dropdown2 = dropdown;
					//Debug.Log ("Dropdown 2 found");
					break;
				}
			}
		}

		if (dropdown1 != null) {
			dropdown1.ClearOptions ();
			dropdown1.AddOptions (conditions);
			dropdown1.onValueChanged.AddListener(delegate {
				selectCondition(dropdown1);
			});
		}
	}

	private void selectCondition(Dropdown dropdown) {
		Debug.Log ("Selected option: " + conditions[dropdown.value]);
		if (dropdown2 != null) {
			dropdown2.ClearOptions ();
			switch (dropdown.value) {
			case 0:
				dropdown2.AddOptions (stations);
				break;
			case 1:
				dropdown2.AddOptions (lines);
				break;
			case 2:
				dropdown2.AddOptions (trains);
				break;
			case 3:
				dropdown2.AddOptions (directions);
				break;
			default:
				dropdown2.AddOptions (stations);
				break;
			}
			dropdown2.interactable = true;
		}
	}
	
	// Update is called once per frame
	void Update () {


	}
}
