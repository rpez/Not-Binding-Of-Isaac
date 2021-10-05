using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// maybe if there is a lot of copy-paste code, can make this interface
// into an abstract class with default implementations
public interface MonsterController
{
    void DamageMonster(float amount);

    void MoveTowardsPlayer(Vector3 playerPos);

    void SetRigidbodyVelocity(Vector2 velocity);

    void OnTriggerStay2D(Collider2D collision);

    void PlayAnimation(string animation);
}
