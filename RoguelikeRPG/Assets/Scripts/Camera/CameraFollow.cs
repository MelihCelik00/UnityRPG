using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followHeight = 7f;
    public float followDistance = 0f;
    public float followHeightSpeed = 0.9f;

    private Transform _player;

    private float _targetHeight;
    private float _currentHeight;
    private float _currentRotation;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        _targetHeight = _player.position.y + followHeight;

        _currentRotation = transform.eulerAngles.y;
        _currentHeight = Mathf.Lerp(transform.position.y, _targetHeight, followHeightSpeed * Time.deltaTime);
        Quaternion euler = Quaternion.Euler(0f, _currentRotation, 0f);

        Vector3 targetPos = _player.position - (euler * Vector3.forward) * followDistance;

        targetPos.y = _currentHeight;

        transform.position = targetPos;
        transform.LookAt(_player);
    }
}
