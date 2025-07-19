using HarmonyLib;
using Mirror;

namespace AtlyssPoise;

[HarmonyPatch(typeof(StatusEntity), "Take_Damage")]
public class PoisePatch
{
    static void Prefix(StatusEntity __instance, ref DamageStruct _dmgStruct)
    {
        if (__instance == null || _dmgStruct._damageValue <= 0) return;

        var nb = __instance.GetComponent<NetworkBehaviour>();
        if (!__instance._isPlayer || !nb.isLocalPlayer || nb == null) return;

        var poiseManager = __instance.GetComponent<PoiseManager>();
        if (poiseManager == null) return;

        var player = __instance._isPlayer;

        // Check for block / parry
        if (player._pCombat && (player._pCombat._isBlocking || player._pCombat._parryChanceBuffer > 0))
        {
            Plugin.Log.LogInfo("[Poise] Blocking or parrying - no poise damage");
            return;
        }

        // Apply poise damage
        poiseManager.ApplyPoiseDamage(_dmgStruct._damageValue);
        Plugin.Log.LogInfo($"[Poise] after damage: {poiseManager.currentPoise}/{poiseManager.maxPoise}");

        if (!poiseManager.isBroken)
        {
            // Suppress knockback
            _dmgStruct._appliedForce = 0f;
            Plugin.Log.LogInfo("[Poise] Suppressing knockback (poise intact).");
        }
        else
        {
            Plugin.Log.LogInfo("[Poise] Poise broken! Knockback allowed");
        }

        /* Another idea: (for heavy attacks to bypass poise)
        if (_dmgStruct._damageWeight == DamageWeight.Heavy)
        {
            Plugin.Log.LogInfo("[Poise] Heavy attacks bypass poise!");
        }
        else
        {
            _dmgStruct._appliedForce = 0f;
            Plugin.Log.LogInfo("[Poise] Suppressing knockback (poise intact).");
        }*/
    }
}