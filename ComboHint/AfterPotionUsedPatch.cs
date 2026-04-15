using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace ComboHint
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(Hook), "AfterPotionUsed")]
	public static class AfterPotionUsedPatch
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00003CEC File Offset: 0x00001EEC
		[NullableContext(1)]
		public static void Postfix(PotionModel potion)
		{
			try
			{
				if (ModEntry.IsOverlayEnabledInCurrentRun())
				{
					AfterCardDrawnPatch.EnsureCombatStateFresh();
					ComboHintOverlay.EnsureAttached();
					ComboHintOverlay.Refresh();
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] failed in after potion used hook: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				Log.Error(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
				ModEntry.LogErrorToFile("AfterPotionUsedPatch.Postfix", ex);
			}
		}
	}
}
