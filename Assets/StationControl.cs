using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationControl : MonoBehaviour {

	public List<BezierSpline> splines;

	private float[] positions;
	private float[] forwardColliderExitPosition;
	private float[] backwardColliderExitPosition;

	// Use this for initialization
	void Start () {

		positions = new float[splines.Count];
		Vector3 stationPosition = transform.position;
		float pointSpacing = .001f;
		float radius = this.GetComponent<CapsuleCollider> ().radius;
		forwardColliderExitPosition = new float[splines.Count];
		backwardColliderExitPosition = new float[splines.Count];

		float enterColliderDistance = 0f;
		float exitColliderDistance = 0f;
		Vector3 enterColliderPosition = new Vector3();
		Vector3 exitColliderPosition = new Vector3();

		for (int i=0; i<splines.Count; i++) {
			BezierSpline spline = splines [i];
			float distance = Vector3.Distance (stationPosition, spline.GetPoint(0f));
			bool enterCollider = false;

			for(float ps = 0; ps < 1f; ps = ps + pointSpacing) {
				Vector3 position = spline.GetPoint(ps);
				float d = Vector3.Distance (stationPosition, position);

				if (d < distance) {
					distance = d;
					positions[i] = ps;
				}

				if (d <= radius && !enterCollider) {
					backwardColliderExitPosition[i] = ps;
					enterColliderDistance = d;
					enterCollider = true;
					enterColliderPosition = position;
				}
				if (d <= radius) {
					forwardColliderExitPosition[i] = ps;
					exitColliderDistance = d;
					exitColliderPosition = position;
				}
			}
		}

		if (transform.name == "Station7") {
			for (int i = 0; i < splines.Count; i++) {
				Debug.Log ("CENTER: " + positions [i] + ", on " + splines[i].name);
				Debug.Log ("backward collide: " + backwardColliderExitPosition [i]);
				Debug.Log ("forward collide: " + forwardColliderExitPosition [i]);
			}
		}

	}

	public float getPosition(BezierSpline spline) {

		for (int i=0; i<splines.Count; i++) {
			BezierSpline bezier = splines [i];
			if (spline == bezier) {
				return positions [i];
			}
		}
		return 0f;
	}

	public bool shouldStop(BezierSpline spline) {

		foreach (BezierSpline bezier in splines) {
			if (bezier == spline) {
				return true;
			}
		}
		return false;
	}

	public float getColliderExitPosition(BezierSpline spline, bool goingForward) {

		for (int i=0; i<splines.Count; i++) {
			BezierSpline bezier = splines [i];
			if (spline == bezier) {
				if (goingForward) {
					return forwardColliderExitPosition [i];
				} else {
					return backwardColliderExitPosition [i];
				}
			}
		}
		return 0f;
	}

}
