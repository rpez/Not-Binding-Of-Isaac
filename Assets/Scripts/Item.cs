using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Item : MonoBehaviour
{
    public AnimatorController m_mutationAnimator;

    public float m_damageModifier = 1f;
    public float m_speedModifier = 1f;
    public float m_attackSpeedModifier = 1f;
    public float m_healthModifier = 1f;
}
