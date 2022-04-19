using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharaAudio
{
    public string name;

    public AudioClip[] shoot;

    public AudioClip[] pass;

    public AudioClip[] throwItem;

    public AudioClip[] celebrate;

    public AudioClip[] electrocuted;

    public PlayerSpecs playerLinkedTo;
}
