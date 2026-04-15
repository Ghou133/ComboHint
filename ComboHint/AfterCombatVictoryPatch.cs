using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;

namespace ComboHint
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(Hook), "AfterCombatVictory")]
	public static class AfterCombatVictoryPatch
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00003D94 File Offset: 0x00001F94
		public static void Postfix()
		{
			try
			{
				ComboHintOverlay.DisposeActiveNode("after_combat_victory");
			}
			catch (Exception ex)
			{
				ModEntry.LogErrorToFile("AfterCombatVictoryPatch.Postfix", ex);
			}
		}
	}
}
