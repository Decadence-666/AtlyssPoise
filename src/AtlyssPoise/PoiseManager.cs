using UnityEngine;
using UnityEngine.Serialization;

namespace AtlyssPoise;

public class PoiseManager : MonoBehaviour
{
    public float maxPoise;
    public float currentPoise;
    public bool isBroken = false;

    private float _timeSinceDamaged = 0f;
    private float _regenRate;
    private float _regenDelay = 10f;
    private float regenPercentage = 0.1f;
    
    private float logTimer = 0f;
    private float logInterval = 1f;
    private Player _player;
    private PoiseBarBuilder _poiseBarBuilder;

    public void Initialize(float poise, Player player, PoiseBarBuilder poiseBarBuilder)
    {
        maxPoise = poise;
        currentPoise = maxPoise;
        _regenRate = maxPoise * regenPercentage;
        _player = player;
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
        
        logTimer += Time.deltaTime;
        if (logTimer >= logInterval && currentPoise < maxPoise)
        {
            Plugin.Log.LogInfo($"[Poise Regen] {currentPoise:F1}/{maxPoise}");
            logTimer = 0f;
        }
    }
    
}