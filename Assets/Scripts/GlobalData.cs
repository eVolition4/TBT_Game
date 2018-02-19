﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

    public static GlobalData instance;

    public int playerID;
    public int gameID;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
