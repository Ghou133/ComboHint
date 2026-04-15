using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Hooks;

namespace ComboHint
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(Hook), "AfterSideTurnStart")]
	public static class AfterSideTurnStartPatch
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00003E04 File Offset: 0x00002004
		[NullableContext(1)]
		public static void Postfix(CombatState combatState, CombatSide side)
		{
			try
			{
				ComboHintOverlay.OnSideTurnStart(side);
			}
			catch (Exception ex)
			{
				ModEntry.LogUi("AfterSideTurnStartPatch.Error", ex.Message);
			}
		}
	}
}
