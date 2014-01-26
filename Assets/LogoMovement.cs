using UnityEngine;
using System.Collections;

public class LogoMovement : MonoBehaviour {

	Vector3 targetPos = new Vector3(0.001349114f, -0.6726874f, 0.5574791f);
	Vector3 startPos = new Vector3(0.001349114f, -0.6726874f, -25.5574791f);
	int frames = 0;
	int curFrame = 0;

	// Use this for initialization
	void Start () {
		transform.position = startPos;
		for (int i = 0; i < MoveCamera.frames.Length; ++i) {
			frames += MoveCamera.frames[i];
		}
		Debug.Log ("frames: " + frames);
	}
	
	// Update is called once per frame
	void Update () {
		if (curFrame >= frames) {
			return;
		}
		float time = curFrame / (float)frames;
		Vector3 pos = startPos + (targetPos - startPos) * time * time;
		transform.position = pos;
		++curFrame;
	}
}
