using HarmonyLib;
using UnityEngine;

namespace SaveState;

[HarmonyPatch(typeof(GameMaster), "Update")]
public class GameMasterUpdatePatch
{
    static void Postfix(GameMaster __instance)
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (__instance.manager.currentZeepLevel != null && __instance.manager.currentZeepLevel.isTestLevel)
            {
                SetupCar car = __instance.carSetups[0];
                Main.state = new CarState
                {
                    CarPos = car.transform.position,
                    CarRot = car.transform.rotation,
                    CarVel = car.cc.rb.velocity,
                    CarAngVel = car.cc.rb.angularVelocity
                };

                __instance.manager.messenger.Log("Saved State", 5f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Main.state != null)
            {
                Main.state = null;
                __instance.manager.messenger.Log("Removed Save State", 5f);
            }
        }
    }
}

[HarmonyPatch(typeof(SetupGame), "SpawnPlayers")]
public class SetupGameSpawnPlayersPatch
{
    static bool Prefix(SetupGame __instance)
    {
        if (__instance.manager.currentZeepLevel == null || !__instance.manager.currentZeepLevel.isTestLevel || Main.state == null) return true;


        SetupCar setupCar3 = Object.Instantiate(__instance.soapboxPrefab);

        setupCar3.transform.position = Main.state.CarPos;
        setupCar3.transform.rotation = Main.state.CarRot;
        setupCar3.DoCarSetup(1, __instance.manager.rect1P);

        __instance.gameMaster.PlayersReady.Add(setupCar3.GetComponent<ReadyToReset>());
        __instance.gameMaster.PlayersReady[0].GiveMaster(__instance.gameMaster, 0);
        __instance.gameMaster.PlayersReady[0].screenPointer = Object.Instantiate(__instance.playerScreenPrefab, __instance.manager.playersScreens);
        __instance.DoProperScreenPointerSize(0, __instance.gameMaster.PlayersReady[0].screenPointer);
        __instance.gameMaster.PlayersReady[0].WakeScreenPointer();
        __instance.gameMaster.previousChampionshipPosition.Add(-1);
        __instance.gameMaster.playerResults.Add(new WinCompare.Result(0, 0f, 0));
        __instance.gameMaster.carSetups.Add(setupCar3);
        setupCar3.cc.GetInput().allowPauseGame = true;
        setupCar3.cc.GetInput().allowQuickReset = false;
        setupCar3.cc.GetInput().allowControl = true;
        setupCar3.cc.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        setupCar3.cc.rb.isKinematic = true;

        __instance.manager.RegisterCamera(setupCar3.theCamera, 1);

        return false;
    }
}

[HarmonyPatch(typeof(GameMaster), "ReleaseTheZeepkists")]
public class SetupGameReleaseTheZeepkistsPatch
{
    static void Postfix(GameMaster __instance)
    {
        if (__instance.manager.currentZeepLevel == null || !__instance.manager.currentZeepLevel.isTestLevel || Main.state == null) return;

        __instance.carSetups[0].cc.rb.velocity = Main.state.CarVel;
        __instance.carSetups[0].cc.rb.angularVelocity = Main.state.CarAngVel;
    }
}

[HarmonyPatch(typeof(LEV_SaveLoad), "AreYouSure")]
public class LEV_SaveLoadAreYouSurePatch
{
    static void Postfix(LEV_SaveLoad __instance)
    {
        if (!__instance.isSaving && Main.state != null)
        {
            Main.state = null;
            __instance.central.manager.messenger.Log("Removed Save State", 5f);
        }
    }
}