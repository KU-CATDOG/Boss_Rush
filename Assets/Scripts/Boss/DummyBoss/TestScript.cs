﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Boss boss;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            boss.GetDamaged(1);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            boss.Stun(5);
        }
    }
}