using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Hero : MonoBehaviour
{
    [SerializeField] protected int _step;
    [SerializeField] protected int _life;
    [SerializeField] protected Sound[] _sound;

    private SoundManager _soundManager;
    private ParticleSystem _particleSystem;
    private bool _isAttack;
    private bool _isDamage;
    protected Animator _animator;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Setup();
        _isAttack = false;
        _isDamage = false;
    }
    protected virtual void Initialize()
    {
        _soundManager = FindObjectOfType<SoundManager>();
        _particleSystem = FindObjectOfType<ParticleSystem>();
    }

    protected virtual void Setup()
    {
       // _soundManager.sounds = _sound;
        PlayerPrefs.SetInt("lifes", _life);
        PlayerPrefs.SetInt("steps", _step);
    }
    public virtual void Attack()
    {
        _isAttack = !_isAttack;
        _animator.SetBool("attack", _isAttack);
    }
    public virtual void Die()
    {
        _animator.SetBool("death", true);
    }
    public virtual void Hurt()
    {
        _isDamage = !_isDamage;
        _animator.SetBool("damage", _isDamage);
    }
    public virtual void Walk(bool isMoving)
    {
        _animator.SetBool("move", isMoving);
        if (isMoving ) {_particleSystem.Play(); }
        else {_particleSystem.Stop(); }
    }
    
}


