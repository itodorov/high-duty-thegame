using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

	double[] positions = {1.233889, 1.233889, 0.57, 0.57, -.15, -.15};
	public static int[] frames = {12, 48, 72, 48, 72, 48, 72, 48};
	double[] delta = {0, -0.65, 0, -.65, 0, -.65, 0, -.91};
	int curPos = 0;
	int curFrame = 0;
	double activeStartPos;
	bool started = false;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (-0.003729045f, 2.338328f, -3.07846f);
		activeStartPos = 2.338328;
	}
	
	// Update is called once per frame
	void Update () {
		if (!started) {
			if(Input.GetKey(KeyCode.Space))
				started = true;
			return;
				}
		//Debug.Log("curPos: " + curPos);
		//Debug.Log("curFrame: " + curFrame);
		if (curPos >= delta.Length) {
			if(Input.GetKey(KeyCode.Space))
			{
				Debug.Log("space");
				Application.LoadLevel("main_scene");
			}
			return;
		}
		double time = curFrame / (double)frames[curPos];
		//Debug.Log (time);
		double deltaY = delta [curPos] * (1 - time) * (1 - time);
		deltaY = delta [curPos] - deltaY;
		// double deltaY = (positions[curPos + 1] - positions[curPos]) / frames[curPos];
		// double deltaY = delta [curPos] / frames [curPos];
		//Debug.Log (activeStartPos);
		transform.position = new Vector3 ((float)transform.position.x, (float)(activeStartPos + deltaY), (float)transform.position.z);
		++curFrame;
		if (curFrame == frames [curPos]) {
			++curPos;
			curFrame = 0;
			activeStartPos += deltaY;
		}
//		transform.position = new Vector3 ((float)transform.position.x, (float)(transform.position.y + deltaY), (float)transform.position.z);
//		transform.position.y += deltaY;
	}
}
