using System;
using TMPro;
using UnityEngine;

namespace Nuclear_reactor.Upgrader
{
    public class BuyUnit : MonoBehaviour
    {
        [SerializeField] private UnitType _type;
        [SerializeField] private TMP_Text _info;

        private void Start()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _info.text = $"Type: {_type}, " +
                         $"Active: {Inventory.Instance.Unitiles[_type]} ," +
                         $"Price: {UpgradeManager.Instance.GetPrice(_type)}";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<FirstPersonController>(out _))
            {
                UpgradeManager.Instance.TrySpendToUpgrade(_type);
                UpdateText();
            }
        }
    }
}