using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace ComboHint
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(NFastModeTickbox), "SetFromSettings")]
	public static class ComboHintFastModeTickboxSetFromSettingsPatch
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00004198 File Offset: 0x00002398
		[NullableContext(1)]
		public static bool Prefix(NFastModeTickbox __instance)
		{
			if (__instance.Name == "ComboHintTickbox")
			{
				__instance.IsTicked = ModEntry.EnabledByGameplaySetting;
				string text = "ComboHintTickbox.SetFromSettings";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("isTicked=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(__instance.IsTicked);
				ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (__instance.Name == "SinglePlayerComboHintTickbox")
			{
				__instance.IsTicked = ModEntry.EnableSinglePlayerHint;
				string text2 = "SinglePlayerComboHintTickbox.SetFromSettings";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("isTicked=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(__instance.IsTicked);
				ModEntry.LogInject(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (__instance.Name == "BubbleHintTickbox")
			{
				__instance.IsTicked = ModEntry.EnableBubbleHint;
				string text3 = "BubbleHintTickbox.SetFromSettings";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("isTicked=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(__instance.IsTicked);
				ModEntry.LogInject(text3, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			return true;
		}
	}
}
