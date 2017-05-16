using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KaiGeX;
using KaiGeX.Core;
using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using KaiGeX.Entities.Variables;
using KaiGeX.Requests;
using KaiGeX.Logging;


public class GameGUI : MonoBehaviour {
	
	//----------------------------------------------------------
	// Setup variables
	//----------------------------------------------------------
	private GameManager gameManager;
	
	//----------------------------------------------------------
	// Unity callbacks
	//----------------------------------------------------------
	void Start() {
		gameManager = this.gameObject.GetComponent<GameManager>();
	}
	
	void OnGUI() {
		// We basically just draw some buttons to change color and model of our player
		GUILayout.BeginArea(new Rect(0, 0, 150, 400));
		GUILayout.BeginVertical();
		
		GUILayout.Label("Select your model");
		
		if (GUILayout.Button("Cube")) {
			gameManager.ChangePlayerModel(0);
		}
		
		if (GUILayout.Button("Sphere")) {
			gameManager.ChangePlayerModel(1);
		}

		if (GUILayout.Button("Capsule")) {
			gameManager.ChangePlayerModel(2);
		}

		GUILayout.Label("Select your color");
		
		if (GUILayout.Button("Blue")) {
			gameManager.ChangePlayerMaterial(0);
		}
		
		if (GUILayout.Button("Green")) {
			gameManager.ChangePlayerMaterial(1);
		}
		
		if (GUILayout.Button("Red")) {
			gameManager.ChangePlayerMaterial(2);
		}
		
		if (GUILayout.Button("Yellow")) {
			gameManager.ChangePlayerMaterial(3);
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}