using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace ComboHint
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	[Nullable(0)]
	public static class ComboHintOverlay
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00005B10 File Offset: 0x00003D10
		public static void EnsureAttached()
		{
			try
			{
				NCombatRoom instance = NCombatRoom.Instance;
				if (instance == null)
				{
					if (ComboHintOverlay._node != null && GodotObject.IsInstanceValid(ComboHintOverlay._node))
					{
						ComboHintOverlay._node.QueueFree();
					}
					if (NGame.Instance != null)
					{
						foreach (ComboHintOverlayNode comboHintOverlayNode in NGame.Instance.GetChildren(false).OfType<ComboHintOverlayNode>().ToList<ComboHintOverlayNode>())
						{
							comboHintOverlayNode.QueueFree();
						}
					}
					ComboHintOverlay._node = null;
					ComboHintOverlay._attachedCombatCreatedMsec = 0UL;
				}
				else
				{
					if (ComboHintOverlay._attachedCombatCreatedMsec != instance.CreatedMsec)
					{
						ComboHintOverlay._node = null;
						ComboHintOverlay._attachedCombatCreatedMsec = instance.CreatedMsec;
					}
					if (ComboHintOverlay._node == null || !GodotObject.IsInstanceValid(ComboHintOverlay._node))
					{
						Node ui = instance.Ui;
						ComboHintOverlayNode comboHintOverlayNode2 = ui.GetChildren(false).OfType<ComboHintOverlayNode>().FirstOrDefault<ComboHintOverlayNode>();
						if (comboHintOverlayNode2 != null)
						{
							comboHintOverlayNode2.SetLowerLayerOrder();
							ComboHintOverlay._node = comboHintOverlayNode2;
						}
						else
						{
							ComboHintOverlayNode comboHintOverlayNode3 = new ComboHintOverlayNode();
							comboHintOverlayNode3.Name = "ComboHintOverlay";
							ui.CallDeferred(Node.MethodName.AddChild, new Variant[] { comboHintOverlayNode3 });
							ComboHintOverlay._node = comboHintOverlayNode3;
							string text = "ComboHintOverlay.Attach";
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
							defaultInterpolatedStringHandler.AppendLiteral("attached parent=");
							defaultInterpolatedStringHandler.AppendFormatted<StringName>(ui.Name);
							defaultInterpolatedStringHandler.AppendLiteral(", type=");
							defaultInterpolatedStringHandler.AppendFormatted(ui.GetType().Name);
							defaultInterpolatedStringHandler.AppendLiteral(", combatCreated=");
							defaultInterpolatedStringHandler.AppendFormatted<ulong>(instance.CreatedMsec);
							ModEntry.LogInfoToFile(text, defaultInterpolatedStringHandler.ToStringAndClear());
							comboHintOverlayNode3.CallDeferred("ForceSetupAndRefresh", Array.Empty<Variant>());
						}
					}
				}
			}
			catch (Exception ex)
			{
				ModEntry.LogErrorToFile("ComboHintOverlay.EnsureAttached", ex);
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005D04 File Offset: 0x00003F04
		public static void Refresh()
		{
			ComboHintOverlay.EnsureAttached();
			if (ComboHintOverlay._node != null && GodotObject.IsInstanceValid(ComboHintOverlay._node))
			{
				ComboHintOverlay._node.RefreshContent();
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005D28 File Offset: 0x00003F28
		public static void RefreshImmediatelyForCurrentTurn(string reason)
		{
			ComboHintOverlay.EnsureAttached();
			if (ComboHintOverlay._node == null || !GodotObject.IsInstanceValid(ComboHintOverlay._node))
			{
				return;
			}
			CombatManager instance = CombatManager.Instance;
			CombatState combatState = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (combatState != null && combatState.CurrentSide == 1)
			{
				ComboHintOverlay._node.ResetVisibilityForTurn(1);
				ModEntry.LogUi("Overlay.RefreshImmediate", "reason=" + reason + ", side=Player");
				return;
			}
			ComboHintOverlay._node.RefreshContent();
			string text = "Overlay.RefreshImmediate";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
			defaultInterpolatedStringHandler.AppendLiteral("reason=");
			defaultInterpolatedStringHandler.AppendFormatted(reason);
			defaultInterpolatedStringHandler.AppendLiteral(", side=");
			defaultInterpolatedStringHandler.AppendFormatted<CombatSide?>((combatState != null) ? new CombatSide?(combatState.CurrentSide) : null);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00005DF7 File Offset: 0x00003FF7
		public static void HideTransient(string reason)
		{
			if (ComboHintOverlay._node != null && GodotObject.IsInstanceValid(ComboHintOverlay._node))
			{
				ComboHintOverlay._node.HideOverlay(reason);
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005E17 File Offset: 0x00004017
		public static void DisposeActiveNode(string reason)
		{
			if (ComboHintOverlay._node != null && GodotObject.IsInstanceValid(ComboHintOverlay._node))
			{
				ModEntry.LogInfoToFile("ComboHintOverlay.Dispose", reason);
				ComboHintOverlay._node.QueueFree();
			}
			ComboHintOverlay._node = null;
			ComboHintOverlay._attachedCombatCreatedMsec = 0UL;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005E4E File Offset: 0x0000404E
		public static void OnSideTurnStart(CombatSide side)
		{
			if (ComboHintOverlay._node != null && GodotObject.IsInstanceValid(ComboHintOverlay._node))
			{
				ComboHintOverlay._node.ResetVisibilityForTurn(side);
			}
		}

		// Token: 0x04000055 RID: 85
		private const string OverlayNodeName = "ComboHintOverlay";

		// Token: 0x04000056 RID: 86
		[Nullable(2)]
		private static ComboHintOverlayNode _node;

		// Token: 0x04000057 RID: 87
		private static ulong _attachedCombatCreatedMsec;
	}
}
