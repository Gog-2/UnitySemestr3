using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    private const int BaseHealth = 100;
    private int _health = BaseHealth;

    [SerializeField]private Rigidbody _rigidbody;
    private const float Speed = 5;
    
    private InputSystem_Actions _movement;

    private void Awake()
    {
        _movement = new InputSystem_Actions();
    }

    private void FixedUpdate()
    {
        Move(_movement.Player.Move.ReadValue<Vector2>());
    }

    private void Move(Vector2 direction)
    {
        _rigidbody.linearVelocity = new Vector3(direction.x,0f,direction.y) * Speed;
    }
    
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        _health -= damage;
        Debug.Log($"Got damaged {damage}, Health: {_health}");
    }
    private void OnEnable()
    {
        _movement.Enable();
    }

    private void OnDisable()
    {
        _movement.Disable();
    }
}
