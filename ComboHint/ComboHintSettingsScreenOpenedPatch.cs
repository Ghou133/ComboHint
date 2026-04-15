using System;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace ComboHint
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(NSettingsScreen), "OnSubmenuOpened")]
	public static class ComboHintSettingsScreenOpenedPatch
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00004114 File Offset: 0x00002314
		[NullableContext(1)]
		public static void Postfix(NSettingsScreen __instance)
		{
			try
			{
				string text = "Patch.SettingsOpened";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
				defaultInterpolatedStringHandler.AppendLiteral("screen=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(__instance.Name);
				defaultInterpolatedStringHandler.AppendLiteral(", visible=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(__instance.Visible);
				ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
				ComboHintSettingsInjector.TryInject(__instance);
			}
			catch (Exception ex)
			{
				ModEntry.LogInject("Patch.SettingsOpened.Error", ex.ToString());
			}
		}
	}
}
