using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Objects/Attack", order = 2)]
public class AttackSO : ScriptableObject
{
    [Header("Main")]
    public int damage;
    public AttackTypeEnum attackTypeEnum;
    public int hitStun;
    public int blockStun;
    public Vector2 travelDistance;
    public Vector2 travelDirection;
    public Vector2 knockbackForce;
    public Vector2 knockbackDirection;
    public int knockbackDuration;

    [Header("Properties")]
    public bool hasSuperArmor;
    public bool isAirAttack;
    public bool isProjectile;
    public bool isArcana;
    public bool jumpCancelable;
    public bool causesKnockdown;
    [Header("Sounds")]
    public string attackSound;
    public string impactSound;
    [Header("Visuals")]
    [Range(0, 150)]
    public int hitstop;
    public GameObject hitEffect;
    public Vector2 hitEffectPosition;
    public float hitEffectRotation;
    public GameObject hurtEffect;
    [HideInInspector] public Vector2 hurtEffectPosition;
    public float hurtEffectRotation;
    public CameraShakerSO cameraShaker;
    [Header("Framedata")]
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;
    public int hitAdv { get { return hitStun - recoveryFrames; } private set { } }
    public int blockAdv { get { return blockStun - recoveryFrames; } private set { } }
    [Header("Information")]
    public string moveName;
    [TextArea(5, 7)]
    public string moveDescription;
    public VideoClip moveVideo;
}

public class ResultAttack
{
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;
    public AttackTypeEnum attackTypeEnum;
    public int damage;
    public int comboDamage;
}