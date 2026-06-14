using EFT.UI.WeaponModding;
using SPT.Reflection.Patching;
using System.Reflection;
using WeaponBuilderSearch.UI;

namespace WeaponBuilderSearch.Patches
{
    internal sealed class DropDownMenuShowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DropDownMenu).GetMethod(nameof(DropDownMenu.Show),
                BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void Postfix(DropDownMenu __instance)
        {
            if (!Plugin.Enabled.Value)
                return;

            var controller = __instance.gameObject.GetComponent<AttachmentSearchController>();
            if (controller == null)
                controller = __instance.gameObject.AddComponent<AttachmentSearchController>();

            controller.OnMenuOpened(__instance);
        }
    }

    internal sealed class DropDownMenuClosePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DropDownMenu).GetMethod(nameof(DropDownMenu.Close),
                BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void Postfix(DropDownMenu __instance)
        {
            if (!Plugin.Enabled.Value)
                return;

            var controller = __instance.gameObject.GetComponent<AttachmentSearchController>();
            if (controller != null)
                controller.OnMenuClosed();
        }
    }
}
