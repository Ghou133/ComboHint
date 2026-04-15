using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;

namespace ComboHint
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(Hook), "AfterTurnEnd")]
	public static class AfterTurnEndPatch
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00003D5C File Offset: 0x00001F5C
		public static void Postfix()
		{
			try
			{
				ComboHintOverlay.HideTransient("after_turn_end");
			}
			catch (Exception ex)
			{
				ModEntry.LogErrorToFile("AfterTurnEndPatch.Postfix", ex);
			}
		}
	}
}
