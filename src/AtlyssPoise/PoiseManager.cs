using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace AtlyssPoise;

public class PoiseManager : MonoBehaviour
{
    // Poise values and isBroken check
    public float maxPoise;
    public float currentPoise;
    public bool isBroken = false;

    // Values for Poise regen
    private float _timeSinceDamaged = 0f;
    private float _regenRate;
    private float _regenDelay = 10f;
    private float _regenPercentage = 0.1f;
    
    // Log
    private float _logTimer = 0f;
    private float _logInterval = 1f;
    private PoiseBarBuilder _poiseBarBuilder;
    
    private Coroutine _recoveryCoroutine;

    public void Initialize(float poise, PoiseBarBuilder poiseBarBuilder)
    {
        maxPoise = poise;
        currentPoise = maxPoise;
        _regenRate = maxPoise * _regenPercentage;
        _poiseBarBuilder = poiseBarBuilder;
    }
    
    public void ApplyPoiseDamage(float damage)
    {
        if (isBroken) return;

        currentPoise -= damage;
        _timeSinceDamaged = 0f;
        
        _poiseBarBuilder?.ShowTemporarily();
        
        if (currentPoise <= 0)
        {
            currentPoise = 0;
            isBroken = true;
            
            // Delayed poise half-reset:
            if (_recoveryCoroutine != null)
                StopCoroutine(_recoveryCoroutine);
            _recoveryCoroutine = StartCoroutine(DelayedHalfReset());
        }
    }

    public void ResetPoise()
    {
        currentPoise = maxPoise;
        isBroken = false;
    }

    public void HalfResetPoise()
    {
        currentPoise = maxPoise / 2;
        isBroken = false;

        if (_recoveryCoroutine != null)
        {
            StopCoroutine(_recoveryCoroutine);
            _recoveryCoroutine = null;
        }
    }

    private void Update()
    {
        if (currentPoise >= maxPoise || isBroken) return;

        _timeSinceDamaged += Time.deltaTime;
        if (_timeSinceDamaged >= _regenDelay)
        {
            currentPoise += _regenRate * Time.deltaTime;
            if (currentPoise > maxPoise)
            {
                currentPoise = maxPoise;
            }
        }
        
        _poiseBarBuilder?.SetPoisePercent(currentPoise / maxPoise);
        
        _logTimer += Time.deltaTime;
        if (_logTimer >= _logInterval && currentPoise < maxPoise)
        {
            Plugin.Log.LogInfo($"[Poise Regen] {currentPoise:F1}/{maxPoise}");
            _logTimer = 0f;
        }
    }
    
    private IEnumerator DelayedHalfReset()
    {
        yield return new WaitForSeconds(0.5f);

        HalfResetPoise();
        Plugin.Log.LogInfo("[Poise] Poise half reset in 0.5 seconds...");
    }
    
}