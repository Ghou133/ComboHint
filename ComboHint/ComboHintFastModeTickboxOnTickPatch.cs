using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace ComboHint
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(NFastModeTickbox), "OnTick")]
	public static class ComboHintFastModeTickboxOnTickPatch
	{
		// Token: 0x0600005E RID: 94 RVA: 0x000042B0 File Offset: 0x000024B0
		[NullableContext(1)]
		public static bool Prefix(NFastModeTickbox __instance)
		{
			if (__instance.Name == "ComboHintTickbox")
			{
				ModEntry.LogInject("ComboHintTickbox.OnTick", "enabled=true");
				ModEntry.SetEnabledByGameplaySetting(true);
				ComboHintSettingsInjector.ShowSettingsToast(__instance, "TOAST_COMBO_HINT_OVERLAY_ON");
				ComboHintOverlay.RefreshImmediatelyForCurrentTurn("settings_toggle_on");
				return false;
			}
			if (__instance.Name == "SinglePlayerComboHintTickbox")
			{
				ModEntry.LogInject("SinglePlayerComboHintTickbox.OnTick", "enabled=true");
				ModEntry.SetEnableSinglePlayerHintByGameplaySetting(true);
				ComboHintSettingsInjector.ShowSettingsToast(__instance, "TOAST_COMBO_HINT_SINGLE_PLAYER_ON");
				ComboHintOverlay.RefreshImmediatelyForCurrentTurn("single_player_setting_toggle_on");
				return false;
			}
			if (__instance.Name == "BubbleHintTickbox")
			{
				ModEntry.LogInject("BubbleHintTickbox.OnTick", "enabled=true");
				ModEntry.SetBubbleHintByGameplaySetting(true);
				ComboHintSettingsInjector.ShowSettingsToast(__instance, "TOAST_COMBO_HINT_BUBBLE_ON");
				return false;
			}
			return true;
		}
	}
}
