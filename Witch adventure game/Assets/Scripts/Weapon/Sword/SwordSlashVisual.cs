using UnityEngine;
using System;

public class SwordSlashVisual : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash(Attack);

    [SerializeField] private Sword sword;

    private const string Attack = "Attack";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        sword.OnSwordSwing += Sword_OnSwordSwing;
    }

    private void Sword_OnSwordSwing(object sender, EventArgs e)
    {
        animator.SetTrigger(AttackHash);
    }

    private void OnDestroy()
    {
        sword.OnSwordSwing -= Sword_OnSwordSwing;
    }
}
