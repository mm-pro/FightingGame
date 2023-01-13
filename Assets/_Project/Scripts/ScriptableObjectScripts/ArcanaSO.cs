using UnityEngine;

[CreateAssetMenu(fileName = "Arcana", menuName = "Scriptable Objects/Arcana", order = 2)]
public class ArcanaSO : AttackSO
{
    [Header("Arcana")]
    public bool airOk;
    public bool reversal;
    public float projectileSpeed;
    public bool projectileDestroyOnHit;
}
