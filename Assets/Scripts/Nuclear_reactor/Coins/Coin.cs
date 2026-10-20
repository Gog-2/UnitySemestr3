using DG.Tweening;
using Nuclear_reactor;
using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
    [SerializeField] private int _ammout;
    
    private bool _magnetToPlayer = false;

    private Vector3 _playerPos;
    
    private IObjectPool<Coin> _pool;
    
    public void SetPool(IObjectPool<Coin> pool)
    {
        _pool = pool;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out _))
        {
            Despawn();
            WalletManager.Instance.AddCurrency(
                Inventory.Instance.Upgrades[UpgradeType.CoinValue] + 1);
        }
    }

    private void FixedUpdate()
    {
        if (_magnetToPlayer)
        {
            transform.DOMove(_playerPos, 1f);        
        }
    }

    public void IsMoveToPlayer(bool moveStatus, Transform direction)
    {
        _magnetToPlayer = moveStatus;
        _playerPos = direction.position;
    }
    public void ReUse(Vector3 position)
    {
        _magnetToPlayer = false;
        transform.position = position;
    }
   
    private void Despawn()
    {
        _pool.Release(this);
    }
}
