using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_WeaponType))]
public class SO_WeaponTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SO_WeaponType weapon = (SO_WeaponType)target;

        serializedObject.Update();

        EditorGUILayout.LabelField("General Info", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponCategory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoCategory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRate"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackCategory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isSemiAutomatic"));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Ammo Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasAmmo"));

        if (weapon.HasAmmo)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("magSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("reloadTime"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Melee Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isMelee"));

        if (weapon.IsMelee)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeReach"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeHitRadius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeHitForce"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Ballistics Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasBallistics"));
        if (weapon.HasBallistics)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("impactRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spreadStandardDeviation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("damageOverDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pelletCount"));
        }

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Recoil Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("recoilType"));
        if (weapon.RecoilType == SO_WeaponType.RecoilTypes.SpreadPerShot)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recoilPerShotMin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recoilPerShotMax"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recoilRecoverySpeed"));
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
