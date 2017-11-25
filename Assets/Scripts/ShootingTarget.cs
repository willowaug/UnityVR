﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VRStandardAssets.Utils;
using VRStandardAssets.Common;

public class ShootingTarget : MonoBehaviour
{
    public int score = 1;
    public float destroyTimeOutDuration = 2f;
    public float timeOutDuration = 2f;
    public event Action<ShootingTarget> OnRemove;
    private Transform cameraTransform;
    public AudioSource audioSource;
    private VRInteractiveItem vrInteractiveItem;
    private Renderer mRenderer;
    private Collider mCollider;
    public AudioClip destroyClip;
    public GameObject destroyPrefab;
    public AudioClip spawnClip;
    public AudioClip missedClip;

    private bool isEnding;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
        vrInteractiveItem = GetComponent<VRInteractiveItem>();
        mRenderer = GetComponent<Renderer>();
        mCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        vrInteractiveItem.OnDown += HandleDown;
    }

    private void OnDisable()
    {
        vrInteractiveItem.OnDown -= HandleDown;
    }

    private void OnDestroy()
    {
        OnRemove = null;
    }
    private void HandleDown()
    {
        StartCoroutine(OnHit());
    }

    private IEnumerator OnHit()
    {
        if (isEnding)
            yield break;
        isEnding = true;
        mRenderer.enabled = false;
        mCollider.enabled = false;
        audioSource.clip = destroyClip;
        audioSource.Play();
        SessionData.AddScore(score);
        GameObject destroyedTarget = Instantiate<GameObject>(destroyPrefab, transform.position, transform.rotation);
        Destroy(destroyedTarget, destroyTimeOutDuration);//幾秒後會死掉
        yield return new WaitForSeconds(destroyClip.length);
        if (OnRemove != null)
            OnRemove(this);
    }

    public void Restart()
    {
        mRenderer.enabled = true;
        mCollider.enabled = true;
        isEnding = false;
        audioSource.clip = spawnClip;
        audioSource.Play();
        transform.LookAt(cameraTransform.position);
        StartCoroutine(MissTarget());

    }

    private IEnumerator MissTarget()
    {
        yield return new WaitForSeconds(timeOutDuration);
        if (isEnding)
            yield break;
        isEnding = true;
        mRenderer.enabled = false;
        mCollider.enabled = false;
        audioSource.clip = missedClip;
        yield return new WaitForSeconds(missedClip.length);
        if (OnRemove != null)
            OnRemove(this);
    }
}

