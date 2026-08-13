using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace FormLock
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Moonlight Peaks.exe")]
    public sealed class FormLockPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.dirtyredz.moonlightpeaks.formlock";
        public const string PluginName = "Form Lock";
        // Keep in step with <Version> in the csproj - pack.ps1 names the archive from that one
        // and BepInEx reports this one. See 12-versioning-and-release.md.
        public const string PluginVersion = "1.0.0";

        internal static ManualLogSource Log;

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<bool> ApplyToPickup;
        internal static ConfigEntry<bool> ApplyToHarvest;
        internal static ConfigEntry<bool> KeepCatForm;
        internal static ConfigEntry<bool> KeepBatForm;
        internal static ConfigEntry<bool> KeepAquaForm;
        internal static ConfigEntry<bool> VerboseLogging;

        private readonly Harmony harmony = new Harmony(PluginGuid);

        // Mod Menu reads these strings out of ConfigDescription.Tags and uses them to title its
        // sections. Display names only - the .cfg section keys are untouched.
        private const string GeneralSection = "ModMenu.Section=General";
        private const string FormsSection = "ModMenu.Section=Forms";
        private const string DiagnosticsSection = "ModMenu.Section=Diagnostics";

        private void Awake()
        {
            Log = Logger;

            Enabled = Config.Bind(
                "General", "Enabled", true,
                new ConfigDescription(
                    "Master switch. Off restores vanilla behaviour entirely.",
                    null,
                    GeneralSection, "ModMenu.Label=Enabled"));

            ApplyToPickup = Config.Bind(
                "General", "ApplyToPickup", true,
                new ConfigDescription(
                    "Stay in Cat/Bat/Aqua form when picking up a loose item off the ground.",
                    null,
                    GeneralSection, "ModMenu.Label=Keep form while picking up items"));

            ApplyToHarvest = Config.Bind(
                "General", "ApplyToHarvest", true,
                new ConfigDescription(
                    "Stay in Cat/Bat/Aqua form when harvesting a bare-hand node (bushes, " +
                    "produce, and similar - not tool-based chopping/mining, which already " +
                    "requires swapping to a matching tool).",
                    null,
                    GeneralSection, "ModMenu.Label=Keep form while harvesting"));

            KeepCatForm = Config.Bind(
                "Forms", "KeepCatForm", true,
                new ConfigDescription(
                    "Apply to Cat Form.",
                    null,
                    FormsSection, "ModMenu.Label=Cat Form"));

            KeepBatForm = Config.Bind(
                "Forms", "KeepBatForm", true,
                new ConfigDescription(
                    "Apply to Bat Form. Bat Form has its own land/water positioning rules " +
                    "(EquipTeleportRange etc.) that vanilla runs when you unequip it - those " +
                    "are skipped along with the unequip, since you never leave the form.",
                    null,
                    FormsSection, "ModMenu.Label=Bat Form"));

            KeepAquaForm = Config.Bind(
                "Forms", "KeepAquaForm", true,
                new ConfigDescription(
                    "Apply to Aqua Form. Same caveat as Bat Form: its water-positioning logic " +
                    "only runs on unequip, so it is skipped while the form is kept on.",
                    null,
                    FormsSection, "ModMenu.Label=Aqua Form"));

            VerboseLogging = Config.Bind(
                "Diagnostics", "VerboseLogging", false,
                new ConfigDescription(
                    "Log every time a form is kept equipped through a pickup or harvest.",
                    null,
                    DiagnosticsSection, "ModMenu.Label=Verbose logging"));

            harmony.PatchAll(typeof(FormPatches));

            Log.LogInfo($"{PluginName} {PluginVersion} loaded. Read-only: nothing is written to your save.");
        }
    }
}
