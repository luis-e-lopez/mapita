using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SplineDecorator2 : MonoBehaviour {

	public BezierSpline spline;

	public float pointSpacing = .01f;

	void Start() {
		
	}

	private void Awake () {

		LineRenderer line = GetComponent<LineRenderer> ();
		int count = 0;

		for(float ps = 0; ps < 1f; ps = ps + pointSpacing) {
			Vector3 position = spline.GetPoint(ps);

			count++;
			line.positionCount = count;
			line.SetPosition (line.positionCount - 1, position);

		} 
	}
}