using Entitas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    private Systems _systems;
	// Use this for initialization
	void Start () {
        _systems = CreateSystems();
        _systems.Initialize();
    }

    private Systems CreateSystems()
    {
        return new Feature("Game")
            .Add(new AR_InitSystem())
            ;
    }

    // Update is called once per frame
    //void Update () {

    //}
}
