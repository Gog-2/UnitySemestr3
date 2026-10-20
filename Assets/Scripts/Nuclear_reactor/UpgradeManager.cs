using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;

namespace Nuclear_reactor
{
    [AlchemySerialize] 
    public partial class UpgradeManager : Singltone<UpgradeManager>
    {
        [AlchemySerializeField, NonSerialized] private Dictionary<UpgradeType, int> _basePrice;
        [AlchemySerializeField, NonSerialized] private Dictionary<UnitType, int> _basePriceUnit;
        private int _buferPrice;
        
        public int GetPrice(UpgradeType type)
        {
            return _basePrice[type] * (int)Mathf.Pow(
                Inventory.Instance.Upgrades[type] + 1, 1.5f);
        }

        public int GetPrice(UnitType type)
        {
            return _basePriceUnit[type];
        }

        public bool TrySpendToUpgrade(UpgradeType type)
        {
            _buferPrice = GetPrice(type);

            if (WalletManager.Instance.TrySpend(_buferPrice))
            {
                Inventory.Instance.Upgrade(type, 1);
                return true;
            }
            return false;
        }
        public bool TrySpendToUpgrade(UnitType type)
        {
            _buferPrice = GetPrice(type);

            if (!Inventory.Instance.Unitiles[type] && WalletManager.Instance.TrySpend(_buferPrice))
            {
                Inventory.Instance.Unitile(type, true);
                return true;
            }
            return false;
        }
    }
}