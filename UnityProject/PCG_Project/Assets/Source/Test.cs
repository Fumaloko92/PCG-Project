using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CastleGenerator cg = new CastleGenerator(new Vector2(200, 200), 50);
        StartCoroutine(cg.runEvolution());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
