using System;
using Chicken.Utilities;
using HarmonyLib;

namespace FormLock
{
    /// <summary>
    /// The shipped 1.0.0 feature: keeps Cat/Bat/Aqua form equipped through item pickups and
    /// bare-hand harvests.
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
    internal static class FormRetentionPatches
    {
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
                if (!FormProtection.TryGetProtectedForm(out var grabbed))
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
    }
}
