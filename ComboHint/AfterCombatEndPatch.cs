using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;

namespace ComboHint
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(Hook), "AfterCombatEnd")]
	public static class AfterCombatEndPatch
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003DCC File Offset: 0x00001FCC
		public static void Postfix()
		{
			try
			{
				ComboHintOverlay.DisposeActiveNode("after_combat_end");
			}
			catch (Exception ex)
			{
				ModEntry.LogErrorToFile("AfterCombatEndPatch.Postfix", ex);
			}
		}
	}
}
