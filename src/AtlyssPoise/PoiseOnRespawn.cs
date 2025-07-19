using HarmonyLib;
using Mirror;

namespace AtlyssPoise;

[HarmonyPatch(typeof(StatusEntity), "Replenish_All")]
public class PoiseOnRespawn
{
    static void Prefix(StatusEntity __instance)
    {
        var nb = __instance.GetComponent<NetworkBehaviour>();
        if (!__instance._isPlayer || !nb.isLocalPlayer || nb == null) return;
        
        var poiseManager = __instance.GetComponent<PoiseManager>();
        if (poiseManager != null)
        {
            poiseManager.ResetPoise();
            Plugin.Log.LogInfo($"[Poise] Reset on respawn: {poiseManager.currentPoise}/{poiseManager.maxPoise}");
        }
    }
}