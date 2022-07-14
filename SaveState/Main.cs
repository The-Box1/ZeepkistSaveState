using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace SaveState;

[BepInAutoPlugin("dev.thebox1.zeepkistsavestate")]
[BepInProcess("Zeepkist.exe")]
public partial class Main : BaseUnityPlugin
{
    public new static BepInEx.Logging.ManualLogSource Logger;

    private Harmony Harmony { get; } = new (Id);

    public static CarState state = null;

    public void Awake()
    {
        Logger = base.Logger;
        Harmony.PatchAll();
        Logger.LogMessage("Loaded SaveState");
    }
}

public class CarState
{
    public Vector3 CarPos;
    public Quaternion CarRot;
    public Vector3 CarVel;
    public Vector3 CarAngVel;
}