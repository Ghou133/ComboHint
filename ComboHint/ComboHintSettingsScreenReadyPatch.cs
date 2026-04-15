using System;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace ComboHint
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(NSettingsScreen), "_Ready")]
	public static class ComboHintSettingsScreenReadyPatch
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00004090 File Offset: 0x00002290
		[NullableContext(1)]
		public static void Postfix(NSettingsScreen __instance)
		{
			try
			{
				string text = "Patch.SettingsReady";
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
				ModEntry.LogInject("Patch.SettingsReady.Error", ex.ToString());
			}
		}
	}
}
