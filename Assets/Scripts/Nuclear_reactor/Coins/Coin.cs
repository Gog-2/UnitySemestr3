using Nuclear_reactor;
using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
    [SerializeField] private int _ammout;
    private IObjectPool<Coin> _pool;
    
    public void SetPool(IObjectPool<Coin> pool)
    {
        _pool = pool;
    }
    private void OnTriggerEnter(Collider other)
    {
        Despawn();
        WalletManager.Instance.AddCurrency(
            Inventory.Instance.Upgrades[UpgradeType.CoinValue] + 1);
    }
    public void ReUse(Vector3 position)
    {
        transform.position = position;
    }
   
    private void Despawn()
    {
        _pool.Release(this);
    }
}
