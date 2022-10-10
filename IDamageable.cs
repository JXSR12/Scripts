using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Die();
    void ReceiveDamage(int dmg);
    void UpdateHealth(int newHp);
    void ResetHealth();
}
