using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationControl : MonoBehaviour {

	public BezierSpline spline;

	private float distance;
	public float progress;
	// Use this for initialization
	void Start () {

		Vector3 stationPosition = transform.position;
		distance = Vector3.Distance (stationPosition, spline.GetPoint(0f));
		float pointSpacing = .001f;

		for(float ps = 0; ps < 1f; ps = ps + pointSpacing) {
			Vector3 position = spline.GetPoint(ps);
			float d = Vector3.Distance (stationPosition, position);

			if (d < distance) {
				distance = d;
				progress = ps;
			}
		}
	}

}
