using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectivesCatalogSO", menuName = "Objectives/ObjectivesCatalogSO")]
public class ObjectivesCatalogSO : ScriptableObject
{
    [SerializeField] private List<ObjectiveSO> objectives;
    public List<ObjectiveSO> Objectives { get => objectives; }
}
