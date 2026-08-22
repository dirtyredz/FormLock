using System;
using Chicken.Utilities;
using HarmonyLib;

namespace FormLock
{
    /// <summary>
    /// Keeps Cat/Bat/Aqua form equipped through item pickups and bare-hand harvests.
    ///
    /// Root cause (see research/01-form-unequip.md): PlayerPickupState.OnActivate and
    /// PlayerHarvestState.OnActivate both unconditionally call GameInventory.TryGrabNone()
    /// before playing their harvest animation, so hands are free for the animation. That is
    /// fine for a held tool, but GameInventory.TryGrabPreviousGrabbedItem() (called on
    /// OnDeactivate to restore whatever was held) explicitly refuses to re-equip a Form-type
    /// tool:
    ///
    ///     if (itemAsset.ToolAddon != null &amp;&amp; itemAsset.ToolAddon.ToolType.UseType == ToolUseType.Form)
    ///         return false;
    ///
    /// so the player is silently dropped back to human form on every pickup/harvest, with no
    /// way to re-enter form except manually re-equipping it from the tool wheel.
    ///
    /// The fix is a single Prefix on TryGrabNone: when the currently grabbed item is a form
    /// tool and the active player state is one of the two above, skip the unequip entirely.
    /// TryGrabPreviousGrabbedItem's own first check (grabbed item already non-null) then makes
    /// its later OnDeactivate call a no-op, so nothing else needs to change.
    /// </summary>
    internal static class FormPatches
    {
        // Saved PlayerPickupState timing fields, restored in OnDeactivate.
        // Negative sentinel means no save is in flight.
        private static float _savedPickupDuration = -1f;
        private static float _savedPickupDurationAfter = -1f;

        [HarmonyPatch(typeof(GameInventory), nameof(GameInventory.TryGrabNone))]
        [HarmonyPrefix]
        private static bool GameInventory_TryGrabNone(bool instant, bool ignoreValidPositionChecks, bool storeCurrentAsPreviousEvenIfNone)
        {
            if (!FormLockPlugin.Enabled.Value)
            {
                return true;
            }

            try
            {
                var grabbed = GameInventory.GrabbedItemAsset;
                if (grabbed == null || !(grabbed.ToolAddon is FormToolAsset) || !IsFormEnabled(grabbed.ToolAddon))
                {
                    return true; // not holding a form we're configured to protect
                }

                var currentState = MonoBehaviourSingleton<PlayerView>.Exists
                    ? MonoBehaviourSingleton<PlayerView>.Instance.StateMachine.CurrentState
                    : null;

                var keep = (FormLockPlugin.ApplyToPickup.Value && currentState is PlayerPickupState)
                        || (FormLockPlugin.ApplyToHarvest.Value && currentState is PlayerHarvestState);

                if (!keep)
                {
                    return true;
                }

                if (FormLockPlugin.VerboseLogging.Value)
                {
                    FormLockPlugin.Log.LogInfo(
                        $"Keeping {grabbed.Name} equipped through {currentState.GetType().Name}.");
                }

                return false; // skip GameInventory.TryGrabNone entirely; stay in form
            }
            catch (Exception e)
            {
                // Never let a broken check strip forms unexpectedly or - worse - wedge the
                // pickup/harvest flow. Fall back to vanilla behaviour for this call.
                FormLockPlugin.Log.LogError($"TryGrabNone patch failed, falling back to vanilla behaviour: {e}");
                return true;
            }
        }

        // The stutter when picking up in form has two root causes that interact:
        //
        // 1. BasePlayerState.OnActivate adds "BasePlayerState" to input.InputBlocker, which
        //    fires PlayerInput.HandleInputBlockerChanged → mover.StopMove(). StopMove zeros
        //    the RVO velocity so the character snaps to a halt even if the state only lasts
        //    two frames.
        //
        // 2. The input blocker remains until OnDeactivate fires, so even after the halt
        //    ProcessInput() is skipped for the rest of that frame, adding another perceived
        //    hitch as the player re-accelerates.
        //
        // Fix:
        //   • Prefix on CharacterMover.StopMove: return false (skip) while the player is in
        //     a form-pickup combo, so the velocity is never zeroed.
        //   • Postfix on PlayerPickupState.OnActivate: immediately remove the "BasePlayerState"
        //     input-blocker key that base.OnActivate just added; ProcessInput() runs on the
        //     very next Update() as if the state never existed.
        //
        // The timer-zeroing patches below are kept so the coroutine still exits in two frames
        // rather than the default 0.46 s, eliminating the HarvestTrigger-animation pop as
        // a belt-and-suspenders measure.

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
                var grabbed = GameInventory.GrabbedItemAsset;
                if (grabbed == null || !(grabbed.ToolAddon is FormToolAsset) || !IsFormEnabled(grabbed.ToolAddon))
                    return true;
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
                var grabbed = GameInventory.GrabbedItemAsset;
                if (grabbed == null || !(grabbed.ToolAddon is FormToolAsset) || !IsFormEnabled(grabbed.ToolAddon))
                    return;

                var t = Traverse.Create(__instance);
                _savedPickupDurationAfter = t.Field("pickupDurationAfter").GetValue<float>();
                _savedPickupDuration      = t.Field("pickupDuration").GetValue<float>();
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
                var grabbed = GameInventory.GrabbedItemAsset;
                if (grabbed == null || !(grabbed.ToolAddon is FormToolAsset) || !IsFormEnabled(grabbed.ToolAddon))
                    return;
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
                _savedPickupDuration      = -1f;
                _savedPickupDurationAfter = -1f;
            }
        }

        private static bool IsFormEnabled(BaseToolItemAddon toolAddon)
        {
            switch (toolAddon)
            {
                case CatToolAsset _:
                    return FormLockPlugin.KeepCatForm.Value;
                case BatToolAsset _:
                    return FormLockPlugin.KeepBatForm.Value;
                case AquaToolAsset _:
                    return FormLockPlugin.KeepAquaForm.Value;
                default:
                    // A future/modded form type we don't know about - err on the side of
                    // vanilla behaviour rather than silently keeping an unrecognised form on.
                    return false;
            }
        }
    }
}
