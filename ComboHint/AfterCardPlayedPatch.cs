using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;

namespace ComboHint
{
	// Token: 0x02000004 RID: 4
	[HarmonyPatch(typeof(Hook), "AfterCardPlayed")]
	public static class AfterCardPlayedPatch
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00003C7C File Offset: 0x00001E7C
		[NullableContext(1)]
		public static void Postfix(CardPlay cardPlay)
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
				defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] failed in after card played hook: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				Log.Error(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
				ModEntry.LogErrorToFile("AfterCardPlayedPatch.Postfix", ex);
			}
		}
	}
}
