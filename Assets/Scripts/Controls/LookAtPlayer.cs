using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

	private Vector3 point;
	public Transform target;
	private float angle = 45;
	private float xValue;
	private float zValue;
	public float rotateSpeed;
	public float cameraDistance;
	public float cameraHeight;

	// Use this for initialization
	void Start () {
		point = target.transform.position;
		transform.LookAt (point);
		xValue = cameraDistance * Mathf.Cos (angle);
		zValue = cameraDistance * Mathf.Sin (angle);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			angle -= rotateSpeed * Time.deltaTime;
			if (angle < 0) angle += 360;
			xValue = cameraDistance * Mathf.Cos (angle);
			zValue = cameraDistance * Mathf.Sin (angle);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			angle += rotateSpeed * Time.deltaTime;
			if (angle > 360) angle -= 360;
			xValue = cameraDistance * Mathf.Cos (angle);
			zValue = cameraDistance * Mathf.Sin (angle);
		}
		point.x += xValue;
		point.y += cameraHeight;
		point.z += zValue;
		transform.position = point;
		point = target.transform.position;
		transform.LookAt (point);
	}
}
