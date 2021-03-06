﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Player _player;

    public float PlayerHealth = 100;
    public float PlayerDamage = 25;
    public GameObject Ammunition;
    public float MovementSpeed = 20;
    public float LookSpeed = 10;
    public float BulletSpeed = 20;

    private Transform _bulletspawn;
    private Animator _animator;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _player = ScriptableObject.CreateInstance<Player>();
        _bulletspawn = GetComponentInChildren<PlayerBulletSpawnBehaviour>().Spawn;
        _player.Health = PlayerHealth;
        _player.Damage = PlayerDamage;
        _player.Alive = true;
    }

    Vector3 LookAround()
    {
        var _hori = Input.GetAxis("HorizontalRightJoy");
        var _vert = Input.GetAxis("VerticalRightJoy");

        //_animator.SetFloat("AimMovement", Mathf.Abs(_hori));
        //_animator.SetFloat("AimMovement", Mathf.Abs(_vert));

        _animator.SetFloat("AimMovement", Mathf.Abs(_hori) + Mathf.Abs(_vert));

        Debug.Log(_hori);

        Vector3 _direction = new Vector3(_hori, 0, _vert);

        return _direction.normalized;
    }

    Vector3 MoveAround()
    {
        //var h = Input.GetAxis("HorizontalLeftJoy");
        //var v = Input.GetAxis("VerticalLeftJoy");

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        //_animator.SetFloat("WalkMovement", Mathf.Abs(h));
        //_animator.SetFloat("WalkMovement", Mathf.Abs(v));

        _animator.SetFloat("WalkMovement", Mathf.Abs(h) + Mathf.Abs(v));

        Vector3 _direction = new Vector3(h, 0, v);

        return _direction.normalized;
    }

    public float shotCooldown = 1f;
    void Shoot()
    {
        StartCoroutine(ShotCooldown(shotCooldown));
    }

    bool CheckIfAlive()
    {
        if(_player.Alive)
        {
            return true;
        }
        return false;
    }


    //RESEARCH THIS
    IEnumerator ShotCooldown(float timer)
    {
        if (shooting)
            yield return null;
        shooting = true;
        var countdown = timer;
        var _bullet = Instantiate(Ammunition, _bulletspawn.position, _bulletspawn.localRotation);
        _bullet.GetComponent<Rigidbody>().velocity += _bulletspawn.forward * BulletSpeed;
        Destroy(_bullet, 2.0f);
        while (countdown >= 0)
        {
            countdown -= Time.deltaTime;
            yield return null;
        }
        shooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        //_bulletspawn = GetComponentInChildren<PlayerBulletSpawnBehaviour>().Spawn;
        //SETUP MOVEMENT
        //SETUP CAMERA FOR PLAYER
        //var h = Input.GetAxis("Horizontal");
        //var v = Input.GetAxis("Vertical");

        //var hSpin = Input.GetAxis("HorizontalArrow");

        //transform.position += new Vector3(h, 0, v);
        //transform.Rotate(new Vector3(0, hSpin * 5, 0) * Time.deltaTime * _lookspeed);
    }

    bool canshoot, shooting;

    void FixedUpdate()
    {
        var _rightTrigger = Input.GetAxis("JoyFire");
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        //RESEARCH THIS
        canshoot = _rightTrigger >= .5f;        
        if(canshoot && !shooting)
        {      
            Shoot();
        }

        if (MoveAround() != Vector3.zero) //CHECK IF THE PLAYER IS MOVING
        {
            if(LookAround() == Vector3.zero) //CHECK IF THE PLAYER IS AIMING
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(MoveAround()), Time.deltaTime * LookSpeed);
            }
        }

        if (LookAround() != Vector3.zero) // CHECKING IF THE ARROW INPUTS ARE ZERO, WILL LOCK PLAYER ROTATION ON RELEASE
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(LookAround()), Time.deltaTime * LookSpeed);
        }

        transform.localPosition += MoveAround() * MovementSpeed * Time.deltaTime;

        //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(LookAround()), Time.deltaTime * LookSpeed);
    }
}
