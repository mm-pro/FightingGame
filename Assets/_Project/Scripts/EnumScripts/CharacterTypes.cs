using System;
using UnityEngine;

[Serializable]
public enum CharacterTypeEnum { TobiDark, PitchBlack, Enma, Abaddon, Nocturne };

public class CharacterTypes : MonoBehaviour
{
    [SerializeField] private CharacterTypeEnum _characterTypeEnum = default;

    public CharacterTypeEnum CharacterTypeEnum { get { return _characterTypeEnum; } private set { } }
}
