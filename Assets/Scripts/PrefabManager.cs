using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject[] captains;
    public GameObject[] mates;
    public GameObject goalKeeper;
    public GameObject Ball;

    private void Awake()
    {
        instance = this;
    }

}
