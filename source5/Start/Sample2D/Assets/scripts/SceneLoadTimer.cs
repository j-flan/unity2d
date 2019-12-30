using UnityEngine;
using System.Collections;

public class SceneLoadTimer : MonoBehaviour 
{
	public int SceneID = 0;
	public float TimeDelay = 5f;

	// Use this for initialization
	void Start () 
	{
		Invoke ("LoadScene", TimeDelay);
	}
	
	void LoadScene()
	{
		Application.LoadLevel(SceneID);
	}
}
