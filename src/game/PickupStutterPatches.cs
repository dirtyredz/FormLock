using System;
using Chicken.Utilities;
using HarmonyLib;

namespace FormLock
{
    /// <summary>
    /// WIP companion feature: removes the movement stutter you get when picking up a loose item
    /// while in form. It only ever engages while <see cref="FormProtection.TryGetProtectedForm"/>
    /// is true, so it never touches vanilla pickups.
    ///
    /// The stutter has two interacting root causes:
    ///
    /// 1. BasePlayerState.OnActivate adds "BasePlayerState" to input.InputBlocker, which
    ///    fires PlayerInput.HandleInputBlockerChanged → mover.StopMove(). StopMove zeros
    ///    the RVO velocity so the character snaps to a halt even if the state only lasts
    ///    two frames.
    ///
    /// 2. The input blocker remains until OnDeactivate fires, so even after the halt
    ///    ProcessInput() is skipped for the rest of that frame, adding another perceived
    ///    hitch as the player re-accelerates.
    ///
    /// Fix:
    ///   • Prefix on CharacterMover.StopMove: return false (skip) while the player is in
    ///     a form-pickup combo, so the velocity is never zeroed.
    ///   • Postfix on PlayerPickupState.OnActivate: immediately remove the "BasePlayerState"
    ///     input-blocker key that base.OnActivate just added; ProcessInput() runs on the
    ///     very next Update() as if the state never existed.
    ///
    /// The timer-zeroing patches (OnActivate prefix / OnDeactivate postfix) are kept so the
    /// coroutine still exits in two frames rather than the default 0.46 s, eliminating the
    /// HarvestTrigger-animation pop as a belt-and-suspenders measure.
    ///
    /// Scope — pickup only, deliberately. The halt is triggered by BasePlayerState.OnActivate's
    /// `if (!isInputAllowed) input.InputBlocker.Add("BasePlayerState")`. PlayerPickupState leaves
    /// isInputAllowed at its default false, so the blocker is added mid-approach and StopMove zeroes
    /// the velocity. PlayerHarvestState overrides `isInputAllowed => true`, so that Add never runs on
    /// activation and there is no walk-through halt to suppress. (Harvest can still StopMove when a
    /// harvest animation begins, via its own "PlayerHarvestState" blocker key, but that's a deliberate
    /// stationary action, not the reported stutter.) So these patches gate on PlayerPickupState only;
    /// mirroring them onto harvest would suppress a halt harvest does not have.
    /// </summary>
    internal static class PickupStutterPatches
    {
        // Saved PlayerPickupState timing fields, restored in OnDeactivate.
        // Negative sentinel means no save is in flight.
        private static float _savedPickupDuration = -1f;
        private static float _savedPickupDurationAfter = -1f;

        [HarmonyPatch(typeof(CharacterMover), nameof(CharacterMover.StopMove))]
        [HarmonyPrefix]
        private static bool CharacterMover_StopMove_Prefix()
        {
            if (!FormLockPlugin.Enabled.Value || !FormLockPlugin.ApplyToPickup.Value) return true;
            try
            {
                if (!MonoBehaviourSingleton<PlayerView>.Exists) return true;
                var pv = MonoBehaviourSingleton<PlayerView>.Instance;
                if (!(pv.StateMachine.CurrentState is PlayerPickupState)) return true;
                if (!FormProtection.TryGetProtectedForm(out _)) return true;
                if (FormLockPlugin.VerboseLogging.Value)
                    FormLockPlugin.Log.LogInfo("Suppressed CharacterMover.StopMove during form pickup.");
                return false; // keep velocity intact
            }
            catch (Exception e)
            {
                FormLockPlugin.Log.LogError($"CharacterMover StopMove prefix failed: {e}");
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerPickupState), "OnActivate")]
        [HarmonyPrefix]
        private static void PlayerPickupState_OnActivate_Prefix(PlayerPickupState __instance)
        {
            if (!FormLockPlugin.Enabled.Value || !FormLockPlugin.ApplyToPickup.Value) return;
            try
            {
                if (!FormProtection.TryGetProtectedForm(out _)) return;

                var t = Traverse.Create(__instance);
                _savedPickupDurationAfter = t.Field("pickupDurationAfter").GetValue<float>();
                _savedPickupDuration = t.Field("pickupDuration").GetValue<float>();
                t.Field("pickupDurationAfter").SetValue(0f);
                t.Field("pickupDuration").SetValue(0f);

                if (FormLockPlugin.VerboseLogging.Value)
                    FormLockPlugin.Log.LogInfo(
                        $"Collapsed pickup waits ({_savedPickupDurationAfter:F2}s + " +
                        $"{_savedPickupDuration - _savedPickupDurationAfter:F2}s) for form pickup.");
            }
            catch (Exception e)
            {
                FormLockPlugin.Log.LogError($"PlayerPickupState OnActivate prefix failed: {e}");
            }
        }

        [HarmonyPatch(typeof(PlayerPickupState), "OnActivate")]
        [HarmonyPostfix]
        private static void PlayerPickupState_OnActivate_Postfix()
        {
            if (!FormLockPlugin.Enabled.Value || !FormLockPlugin.ApplyToPickup.Value) return;
            try
            {
                if (!FormProtection.TryGetProtectedForm(out _)) return;
                if (!MonoBehaviourSingleton<PlayerView>.Exists) return;

                // Remove the "BasePlayerState" key that base.OnActivate() just added so that
                // PlayerInput.ProcessInput() is not skipped on the next Update().
                var pv = MonoBehaviourSingleton<PlayerView>.Instance;
                var input = Traverse.Create(pv).Property("Input").GetValue<PlayerInput>();
                input?.InputBlocker.Remove("BasePlayerState");

                if (FormLockPlugin.VerboseLogging.Value)
                    FormLockPlugin.Log.LogInfo("Restored input blocker state after form pickup OnActivate.");
            }
            catch (Exception e)
            {
                FormLockPlugin.Log.LogError($"PlayerPickupState OnActivate postfix failed: {e}");
            }
        }

        [HarmonyPatch(typeof(PlayerPickupState), "OnDeactivate")]
        [HarmonyPostfix]
        private static void PlayerPickupState_OnDeactivate_Postfix(PlayerPickupState __instance)
        {
            if (_savedPickupDuration < 0f) return;
            try
            {
                var t = Traverse.Create(__instance);
                t.Field("pickupDurationAfter").SetValue(_savedPickupDurationAfter);
                t.Field("pickupDuration").SetValue(_savedPickupDuration);
            }
            catch (Exception e)
            {
                FormLockPlugin.Log.LogError($"PlayerPickupState OnDeactivate postfix failed: {e}");
            }
            finally
            {
                _savedPickupDuration = -1f;
                _savedPickupDurationAfter = -1f;
            }
        }
    }
}
