using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager _instance;

    private void Awake()
    {
        if( _instance == null )
        {
            _instance = this;
            DontDestroyOnLoad( this );
        }
        else
            Destroy( this );
    }
}
