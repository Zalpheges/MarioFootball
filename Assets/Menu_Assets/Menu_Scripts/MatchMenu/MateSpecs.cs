using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MateSpecs
{
    public enum MateType
    {
        Toad,
        Koopa
    }

    public MateType mateType;
    public PlayerSpecs mateSpec;
}
