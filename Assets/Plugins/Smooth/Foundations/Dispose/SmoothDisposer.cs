using UnityEngine;
using System;
using Smooth.Dispose;

public class SmoothDisposer : MonoBehaviour {

	private static SmoothDisposer _instance;

	private void Awake() {
		if (_instance) {
			Debug.LogWarning("Only one " + GetType().Name + " should exist at a time, instantiated by the " + typeof(DisposalQueue).Name + " class.");
			Destroy(this);
		} else {
			_instance = this;
			DontDestroyOnLoad(this);
		}
	}
	
	private void LateUpdate() {
		DisposalQueue.Pulse();
	}
}