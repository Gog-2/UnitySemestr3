using System;
using Nuclear_reactor;
using UnityEngine;

public class WalletManager : Singltone<WalletManager>
{
    [SerializeField] private int _walletBalance;
    public event Action<int> OnValueChanged;

    public void AddCurrency(int value)
    {
        
        _walletBalance += value;
        
        OnValueChanged?.Invoke(_walletBalance);
        
    }

    public bool TrySpend(int price)
    {
        if (_walletBalance >= price)
        {
            
            _walletBalance -= price;
            
            OnValueChanged?.Invoke(_walletBalance);
            
            return true;
            
        }

        return false;

    }
}
