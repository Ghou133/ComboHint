using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace ComboHint
{
	// Token: 0x02000010 RID: 16
	[HarmonyPatch(typeof(NFastModeTickbox), "OnUntick")]
	public static class ComboHintFastModeTickboxOnUntickPatch
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00004380 File Offset: 0x00002580
		[NullableContext(1)]
		public static bool Prefix(NFastModeTickbox __instance)
		{
			if (__instance.Name == "ComboHintTickbox")
			{
				ModEntry.LogInject("ComboHintTickbox.OnUntick", "enabled=false");
				ModEntry.SetEnabledByGameplaySetting(false);
				ComboHintSettingsInjector.ShowSettingsToast(__instance, "TOAST_COMBO_HINT_OVERLAY_OFF");
				ComboHintOverlay.HideTransient("settings_toggle_off");
				return false;
			}
			if (__instance.Name == "SinglePlayerComboHintTickbox")
			{
				ModEntry.LogInject("SinglePlayerComboHintTickbox.OnUntick", "enabled=false");
				ModEntry.SetEnableSinglePlayerHintByGameplaySetting(false);
				ComboHintSettingsInjector.ShowSettingsToast(__instance, "TOAST_COMBO_HINT_SINGLE_PLAYER_OFF");
				ComboHintOverlay.HideTransient("single_player_setting_toggle_off");
				return false;
			}
			if (__instance.Name == "BubbleHintTickbox")
			{
				ModEntry.LogInject("BubbleHintTickbox.OnUntick", "enabled=false");
				ModEntry.SetBubbleHintByGameplaySetting(false);
				ComboHintSettingsInjector.ShowSettingsToast(__instance, "TOAST_COMBO_HINT_BUBBLE_OFF");
				return false;
			}
			return true;
		}
	}
}
