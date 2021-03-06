using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// maybe if there is a lot of copy-paste code, can make this interface
// into an abstract class with default implementations
public interface MonsterController
{
    void DamageMonster(float amount);

    void MoveTo(Vector3 position);

    void SetRigidbodyVelocity(Vector2 velocity);

    void OnCollisionStay2D(Collision2D collision);

    void PlayAnimation(string animation);

    public bool IsDead();

    public void Activate();
}
