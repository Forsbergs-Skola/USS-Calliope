    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;



    [System.Serializable]
    public class InventoryData : IRuntimeData
    // all the save-worthy inventory information
    {
        public bool IsSandbox { get; private set; }

        /////////////////
        // Data Fields //
        /////////////////

        private List<string> _weaponItemIDs;
        private Dictionary<string, int> _consumableItemIDsAndQuantities;
        private List<string> _questItemIDs;

        // WEAPONS
        public void SetWeaponsList(List<string> ids)
        {
            _weaponItemIDs = new List<string>(ids);
        }
        public List<string> GetWeaponItemIDs()
        {
            return new List<string>(_weaponItemIDs);
        }
        public void AddWeaponItem(string weaponItemID)
        {
            if (_weaponItemIDs.Contains(weaponItemID)) return;
            _weaponItemIDs.Add(weaponItemID);
            DataTools.HandleOnDataChanged(this);
        }
        public void RemoveWeaponItem(string weaponItemID)
        {
            if (!_weaponItemIDs.Contains(weaponItemID)) return;
            _weaponItemIDs.Remove(weaponItemID);
            DataTools.HandleOnDataChanged(this);
        }

        // QUEST ITEMS
        public void SetQuestItemsList(List<string> ids)
        {
            _questItemIDs = new List<string>(ids);
        }
        public List<string> GetQuestItemIDs()
        {
            return new List<string>(_questItemIDs);
        }
        public void AddQuestItem(string questItemID)
        {
            if (_questItemIDs.Contains(questItemID)) return;
            _questItemIDs.Add(questItemID);
            DataTools.HandleOnDataChanged(this);
        }
        public void RemoveQuestItem(string questItemID)
        {
            if (!_questItemIDs.Contains(questItemID)) return;
            _questItemIDs.Remove(questItemID);
            DataTools.HandleOnDataChanged(this);
        }

    // CONSUMABLES
        public void SetConsumablesDict(Dictionary<string, int> dict)
        {
            _consumableItemIDsAndQuantities = new Dictionary<string, int>(dict);
        }
        public Dictionary<string, int> GetConsumableIDsAndQuantities()
        {
            return new Dictionary<string, int>(_consumableItemIDsAndQuantities);
        }
        public void DepleteConsumable(string consumableID, int depleteAmount)
        {
            if (!_consumableItemIDsAndQuantities.Keys.ToList<string>().Contains(consumableID)) return;

            depleteAmount = Mathf.Abs(depleteAmount);
            if (depleteAmount <= 0) return;
            if (_consumableItemIDsAndQuantities[consumableID] <= 0) return;

            int newValue = _consumableItemIDsAndQuantities[consumableID] - depleteAmount;
            newValue = Mathf.Max(0, newValue); // don't go below zero
            _consumableItemIDsAndQuantities[consumableID] = newValue;
            DataTools.HandleOnDataChanged(this);
        }
        public void ReplenishConsumable(string consumableID, int replenishAmount)
        {
            if (!_consumableItemIDsAndQuantities.Keys.ToList<string>().Contains(consumableID)) return;

            replenishAmount = Mathf.Abs(replenishAmount);
            if (replenishAmount <= 0) return;

            int newValue = _consumableItemIDsAndQuantities[consumableID] + replenishAmount;
            _consumableItemIDsAndQuantities[consumableID] = newValue;
            DataTools.HandleOnDataChanged(this);
        }
        public void AddNewConsumable(string consumableID, int amount)
        {
            if (_consumableItemIDsAndQuantities.Keys.ToList<string>().Contains(consumableID)) return;
            _consumableItemIDsAndQuantities[consumableID] = amount;
            DataTools.HandleOnDataChanged(this);

        }

        //////////////////
        // Constructors //
        //////////////////
        public InventoryData()
        {
            IsSandbox = false;
            _weaponItemIDs = new List<string>();
            _questItemIDs = new List<string>();
            _consumableItemIDsAndQuantities = new Dictionary<string, int>();
        }
        public InventoryData(bool isSandbox)
        {
            IsSandbox = isSandbox;
            _weaponItemIDs = new List<string>();
            _questItemIDs = new List<string>();
            _consumableItemIDsAndQuantities = new Dictionary<string, int>();
        }
        public InventoryData(InventoryData inData)
        {
            IsSandbox = false;
            _weaponItemIDs = inData.GetWeaponItemIDs();
            _questItemIDs = inData.GetQuestItemIDs();
            _consumableItemIDsAndQuantities = inData.GetConsumableIDsAndQuantities();
        }
        public bool GetIsSandbox() { return IsSandbox; }

    }
