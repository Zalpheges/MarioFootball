using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharaAudio
{
    public string Name;

    public AudioClip[] Shoot;

    public AudioClip[] Pass;

    public AudioClip[] ThrowItem;

    public AudioClip[] Celebrate;

    public AudioClip[] Electrocuted;

    public PlayerSpecs PlayerLinkedTo;
}
