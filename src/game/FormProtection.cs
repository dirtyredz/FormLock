using Chicken.Utilities;

namespace FormLock
{
    /// <summary>
    /// Policy shared by every patch: "is the player currently holding a Cat/Bat/Aqua form tool
    /// that the config says we should keep equipped?" Both the form-retention patch and the
    /// pickup-stutter patches gate on exactly this question, so it lives in one place rather than
    /// being re-derived in each patch. Neither patch class depends on the other as a result.
    /// </summary>
    internal static class FormProtection
    {
        /// <summary>
        /// True when the grabbed item is a form tool we're configured to protect. The grabbed
        /// asset is handed back via <paramref name="grabbed"/> for callers that log its name;
        /// callers that don't care can pass <c>out _</c>.
        /// </summary>
        internal static bool TryGetProtectedForm(out ItemAsset grabbed)
        {
            grabbed = GameInventory.GrabbedItemAsset;
            return grabbed != null
                && grabbed.ToolAddon is FormToolAsset
                && IsFormEnabled(grabbed.ToolAddon);
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
