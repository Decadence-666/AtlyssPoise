using HarmonyLib;
using Mirror;
using UnityEngine;

namespace AtlyssPoise;

[HarmonyPatch(typeof(StatusEntity), "Start")]
public class LocalPlayerInitPatch
{
    private static float _poiseMulti = 5f;
    static void Postfix(StatusEntity __instance)
    {
        if (__instance == null) return;

        var nb = __instance.GetComponent<NetworkBehaviour>();
        if (!__instance._isPlayer || nb == null || !nb.isLocalPlayer) return;

        if (__instance.GetComponent<PoiseManager>() != null) return;

        Plugin.Log.LogInfo("[Poise] Patching from Start() — init coroutine starting.");
        
        GameObject ui = new GameObject("PoiseBarUI");
        var builder = ui.AddComponent<PoiseBarBuilder>();
        __instance.StartCoroutine(WaitForDefenseAndInit(__instance, __instance._isPlayer, builder));
    }

    private static System.Collections.IEnumerator WaitForDefenseAndInit(
        StatusEntity __instance, Player player, PoiseBarBuilder barBuilder)
    {
        float poise = 0f;
        float defense = __instance._pStats._statStruct._defense;

        int attempts = 0;
        while (attempts < 400)
        {
            if (__instance._pStats != null)
            {
                defense = __instance._pStats._statStruct._defense;
                
                if (defense > 0f)
                {
                    poise = defense * _poiseMulti;
                    break;
                }
                if (attempts % 50 == 0)
                    Plugin.Log.LogInfo($"[Poise] Attempt {attempts}: Defense = {defense}");
            }
            else
            {
                Plugin.Log.LogInfo($"[Poise] Attempt {attempts}: _pStats is null.");
            }

            attempts++;
            yield return new WaitForSeconds(0.05f);
        }
        Plugin.Log.LogInfo($"[Poise] Defense = {defense}");

        if (poise == 0f)
        {
            Plugin.Log.LogWarning("[Poise] Defense never initialized — using fallback.");
        }

        var poiseManager = __instance.gameObject.AddComponent<PoiseManager>();
        poiseManager.Initialize(poise, barBuilder);
        barBuilder.SetPoisePercent(1f);

        Plugin.Log.LogInfo($"[Poise] Initialized after wait: {poiseManager.currentPoise}/{poiseManager.maxPoise}");
    }
}