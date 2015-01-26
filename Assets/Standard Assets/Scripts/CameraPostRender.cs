using UnityEngine;
using System.Collections;
//using UnityEngine.Classes.GL;
public class CameraPostRender : MonoBehaviour {

	// Use this for initialization
	void Start () {
		print ("wtf, started");
	}
	
	// Update is called once per frame
	void Update () {
		print ("wtf, upd");
	}

	void OnPostRender()
	{
		GL.IssuePluginEvent (0);
		print ("wtf, gl call");
	}
}
