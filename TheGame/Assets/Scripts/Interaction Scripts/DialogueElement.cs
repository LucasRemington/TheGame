﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueElement
{

    public enum Characters {DialogBox};
    public Characters Character;
    public Sprite CharacterPic;
    public string DialogueText;
    public string[] ChoiceText;
    public string CharacterName;
    public GUIStyle DialogueTextStyle;
    public float TextPlayBackSpeed;
    public AudioClip PlayBackSoundFile;
    public Vector3 textBoxLocation;
    public bool isAboveSelf;
    public bool isAboveCharacter;

}
