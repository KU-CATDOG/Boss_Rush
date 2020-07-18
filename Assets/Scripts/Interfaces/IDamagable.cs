﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    int Health { get; }
    void GetDamaged(int damage);
}
