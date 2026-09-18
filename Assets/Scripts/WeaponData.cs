using UnityEngine;
public enum WeaponType { Melee, Ranged }

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float attackCooldown;
    public float attackRange;
    public WeaponType weaponType;
    public GameObject bulletPrefab;
    public bool hasAmmo;
    public int maxAmmo;
    public float ammoRegenTime;
}