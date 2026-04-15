using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace ComboHint
{
	// Token: 0x02000013 RID: 19
	[NullableContext(1)]
	[Nullable(0)]
	public class ComboHintOverlayNode : PanelContainer
	{
		// Token: 0x06000085 RID: 133 RVA: 0x00005E70 File Offset: 0x00004070
		public override void _EnterTree()
		{
			string text = "Overlay.EnterTree";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
			defaultInterpolatedStringHandler.AppendLiteral("parent=");
			Node parent = base.GetParent();
			defaultInterpolatedStringHandler.AppendFormatted<StringName>((parent != null) ? parent.Name : null);
			defaultInterpolatedStringHandler.AppendLiteral(", type=");
			Node parent2 = base.GetParent();
			defaultInterpolatedStringHandler.AppendFormatted((parent2 != null) ? parent2.GetType().Name : null);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005EE8 File Offset: 0x000040E8
		private void EnsureInitialized()
		{
			if (this._isReady && this._descriptionLabel != null && GodotObject.IsInstanceValid(this._descriptionLabel))
			{
				return;
			}
			base.AnchorLeft = 1f;
			base.AnchorTop = 0f;
			base.AnchorRight = 1f;
			base.AnchorBottom = 0f;
			base.OffsetLeft = -460f;
			base.OffsetTop = 110f;
			base.OffsetRight = -16f;
			base.OffsetBottom = 304f;
			base.MouseFilter = 1L;
			base.ZIndex = -1;
			base.TopLevel = false;
			base.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
			if (this._hoverTipBox == null || !GodotObject.IsInstanceValid(this._hoverTipBox))
			{
				this._hoverTipBox = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip.tscn").Instantiate<Control>(0L);
				this._hoverTipBox.Name = "HoverTipBox";
				this._hoverTipBox.MouseFilter = 1L;
				base.AddChild(this._hoverTipBox, false, 0L);
				this._hoverTipBox.MouseEntered += this.OnHoverTipMouseEntered;
				this._hoverTipBox.MouseExited += this.OnHoverTipMouseExited;
				this._titleLabel = this._hoverTipBox.GetNode<MegaLabel>("%Title");
				this._descriptionLabel = this._hoverTipBox.GetNode<MegaRichTextLabel>("%Description");
				this._descriptionLabel.BbcodeEnabled = true;
				this._descriptionLabel.AutowrapMode = 3L;
				this._descriptionLabel.MouseFilter = 2L;
				TextureRect nodeOrNull = this._hoverTipBox.GetNodeOrNull<TextureRect>("%Icon");
				if (nodeOrNull != null)
				{
					nodeOrNull.Visible = false;
				}
				string text = "Overlay.BoxCreated";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
				defaultInterpolatedStringHandler.AppendLiteral("box=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(this._hoverTipBox.Name);
				ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			this._isReady = true;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000060E4 File Offset: 0x000042E4
		public void ForceSetupAndRefresh()
		{
			this.EnsureInitialized();
			this.SetLowerLayerOrder();
			this.RefreshContent();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000060F8 File Offset: 0x000042F8
		public void SetLowerLayerOrder()
		{
			base.ZIndex = -1;
			Node parent = base.GetParent();
			if (parent != null)
			{
				parent.MoveChild(this, 0);
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006120 File Offset: 0x00004320
		public override void _Ready()
		{
			this.EnsureInitialized();
			string text = "ComboHintOverlayNode.Ready";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 8);
			defaultInterpolatedStringHandler.AppendLiteral("parent=");
			Node parent = base.GetParent();
			defaultInterpolatedStringHandler.AppendFormatted<StringName>((parent != null) ? parent.Name : null);
			defaultInterpolatedStringHandler.AppendLiteral(", type=");
			Node parent2 = base.GetParent();
			defaultInterpolatedStringHandler.AppendFormatted((parent2 != null) ? parent2.GetType().Name : null);
			defaultInterpolatedStringHandler.AppendLiteral(", topLevel=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(base.TopLevel);
			defaultInterpolatedStringHandler.AppendLiteral(", zIndex=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(base.ZIndex);
			defaultInterpolatedStringHandler.AppendLiteral(", offsets=(");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetLeft);
			defaultInterpolatedStringHandler.AppendLiteral(",");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetTop);
			defaultInterpolatedStringHandler.AppendLiteral(",");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetRight);
			defaultInterpolatedStringHandler.AppendLiteral(",");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetBottom);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			ModEntry.LogInfoToFile(text, defaultInterpolatedStringHandler.ToStringAndClear());
			base.SetProcess(true);
			this._isVisible = false;
			this.RefreshContent();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006254 File Offset: 0x00004454
		public override void _ExitTree()
		{
			if (this._hoverTipBox != null && GodotObject.IsInstanceValid(this._hoverTipBox))
			{
				this._hoverTipBox.MouseEntered -= this.OnHoverTipMouseEntered;
				this._hoverTipBox.MouseExited -= this.OnHoverTipMouseExited;
			}
			this.HideOverlay("exit_tree");
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000062B0 File Offset: 0x000044B0
		public override void _Process(double delta)
		{
			NCombatRoom instance = NCombatRoom.Instance;
			bool flag = instance != null && instance.Mode == 0;
			bool flag2 = instance != null && ActiveScreenContext.Instance.IsCurrent(instance);
			if (!flag || !flag2)
			{
				string text = "Overlay.Dispose";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 3);
				defaultInterpolatedStringHandler.AppendLiteral("isCombatActive=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(flag);
				defaultInterpolatedStringHandler.AppendLiteral(", isCombatCurrentScreen=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(flag2);
				defaultInterpolatedStringHandler.AppendLiteral(", mode=");
				defaultInterpolatedStringHandler.AppendFormatted<CombatRoomMode?>((instance != null) ? new CombatRoomMode?(instance.Mode) : null);
				ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
				if (GodotObject.IsInstanceValid(this))
				{
					if (this._hoverTipBox != null && GodotObject.IsInstanceValid(this._hoverTipBox))
					{
						this._hoverTipBox.Visible = false;
					}
					base.QueueFree();
				}
				return;
			}
			this.UpdateHoverOpacity();
			this.LogHoverAlphaWhileHovering();
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000639C File Offset: 0x0000459C
		private void UpdateHoverOpacity()
		{
			if (this._hoverTipBox == null || !GodotObject.IsInstanceValid(this._hoverTipBox) || !this._hoverTipBox.Visible)
			{
				return;
			}
			Rect2 globalRect = this._hoverTipBox.GetGlobalRect();
			Vector2 globalMousePosition = base.GetGlobalMousePosition();
			bool flag = globalRect.HasPoint(globalMousePosition);
			this.SetOverlayOpacity(flag, false);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000063F0 File Offset: 0x000045F0
		private void LogHoverAlphaWhileHovering()
		{
			if (this._hoverTipBox == null || !GodotObject.IsInstanceValid(this._hoverTipBox) || !this._hoverTipBox.Visible || !this._isHovering)
			{
				return;
			}
			ulong processFrames = Engine.GetProcessFrames();
			if (this._lastHoverAlphaLogFrame != 0UL && processFrames - this._lastHoverAlphaLogFrame < 5UL)
			{
				return;
			}
			this._lastHoverAlphaLogFrame = processFrames;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] overlay alpha while hovering: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(this._hoverTipBox.Modulate.A, "0.00");
			defaultInterpolatedStringHandler.AppendLiteral(", frame=");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(processFrames);
			Log.Info(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000649D File Offset: 0x0000469D
		private void OnHoverTipMouseEntered()
		{
			this.SetOverlayOpacity(true, true);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000064A7 File Offset: 0x000046A7
		private void OnHoverTipMouseExited()
		{
			this.SetOverlayOpacity(false, true);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000064B4 File Offset: 0x000046B4
		public void HideOverlay(string reason)
		{
			if (!GodotObject.IsInstanceValid(this))
			{
				return;
			}
			this._isVisible = false;
			if (this._hoverTipBox != null && GodotObject.IsInstanceValid(this._hoverTipBox))
			{
				this._hoverTipBox.Visible = false;
				this._isHovering = false;
			}
			string text = "Overlay.Hide";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
			defaultInterpolatedStringHandler.AppendLiteral("reason=");
			defaultInterpolatedStringHandler.AppendFormatted(reason);
			defaultInterpolatedStringHandler.AppendLiteral(", panelVisible=");
			Control hoverTipBox = this._hoverTipBox;
			defaultInterpolatedStringHandler.AppendFormatted<bool?>((hoverTipBox != null) ? new bool?(hoverTipBox.Visible) : null);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
			this.LogStateIfChanged(reason, 0);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00006564 File Offset: 0x00004764
		public void ResetVisibilityForTurn(CombatSide side)
		{
			this.EnsureInitialized();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (side == 1)
			{
				this._isVisible = true;
				this.RefreshContent();
				this.ApplyIdleOpacity();
				string text = "Overlay.TurnStartReset";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("side=Player, panelVisible=");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(this._hoverTipBox.Visible);
				ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
			defaultInterpolatedStringHandler.AppendLiteral("side_turn_start_");
			defaultInterpolatedStringHandler.AppendFormatted<CombatSide>(side);
			this.HideOverlay(defaultInterpolatedStringHandler.ToStringAndClear());
			string text2 = "Overlay.TurnStartReset";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
			defaultInterpolatedStringHandler.AppendLiteral("side=");
			defaultInterpolatedStringHandler.AppendFormatted<CombatSide>(side);
			defaultInterpolatedStringHandler.AppendLiteral(", panelVisible=");
			Control hoverTipBox = this._hoverTipBox;
			defaultInterpolatedStringHandler.AppendFormatted<bool?>((hoverTipBox != null) ? new bool?(hoverTipBox.Visible) : null);
			ModEntry.LogUi(text2, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006654 File Offset: 0x00004854
		public void RefreshContent()
		{
			try
			{
				this.EnsureInitialized();
				if (!this._isReady || this._descriptionLabel == null || !GodotObject.IsInstanceValid(this._descriptionLabel))
				{
					this.LogStateIfChanged("not_ready", 0);
				}
				else if (!ModEntry.IsOverlayEnabledInCurrentRun())
				{
					this.LogStateIfChanged("disabled_by_singleplayer_setting", 0);
					this._hoverTipBox.Visible = false;
				}
				else if (!this._isVisible)
				{
					this.LogStateIfChanged("hidden_by_turn_state", 0);
					this._hoverTipBox.Visible = false;
				}
				else
				{
					NCombatRoom instance = NCombatRoom.Instance;
					if (instance == null)
					{
						this.LogStateIfChanged("no_combat_room", 0);
						this._hoverTipBox.Visible = false;
					}
					else
					{
						List<string> list = new List<string>();
						List<Creature> list2 = (from n in instance.CreatureNodes
							select n.Entity into c
							where c != null && c.IsPlayer
							select c).ToList<Creature>();
						foreach (Creature creature in list2)
						{
							Player player = creature.Player;
							IEnumerable<CardModel> enumerable;
							if (player == null)
							{
								enumerable = null;
							}
							else
							{
								PlayerCombatState playerCombatState = player.PlayerCombatState;
								if (playerCombatState == null)
								{
									enumerable = null;
								}
								else
								{
									CardPile hand = playerCombatState.Hand;
									enumerable = ((hand != null) ? hand.Cards : null);
								}
							}
							List<MatchedTrigger> list3 = ComboHintOverlayNode.FindMatchedTextsInHand(enumerable ?? Array.Empty<CardModel>());
							if (list3.Count != 0)
							{
								string safePlayerName = ComboHintOverlayNode.GetSafePlayerName(creature);
								string text = string.Join(ModEntry.GetLocalizedListSeparator(), list3.Select((MatchedTrigger m) => ComboHintOverlayNode.FormatColoredText(m.Text, m.ColorHex)));
								list.Add(safePlayerName + ModEntry.GetLocalizedHasConnector() + text);
							}
						}
						List<string> list4 = new List<string>();
						if (ModEntry.OverlayKillTriggerGroup.TriggerModelIds.Count > 0)
						{
							foreach (Creature creature2 in list2)
							{
								Player player2 = creature2.Player;
								IEnumerable<CardModel> enumerable2;
								if (player2 == null)
								{
									enumerable2 = null;
								}
								else
								{
									PlayerCombatState playerCombatState2 = player2.PlayerCombatState;
									if (playerCombatState2 == null)
									{
										enumerable2 = null;
									}
									else
									{
										CardPile hand2 = playerCombatState2.Hand;
										enumerable2 = ((hand2 != null) ? hand2.Cards : null);
									}
								}
								List<MatchedTrigger> list5 = ComboHintOverlayNode.FindMatchedTextsInHandForGroup(enumerable2 ?? Array.Empty<CardModel>(), ModEntry.OverlayKillTriggerGroup);
								if (list5.Count != 0)
								{
									string safePlayerName2 = ComboHintOverlayNode.GetSafePlayerName(creature2);
									string text2 = string.Join(ModEntry.GetLocalizedListSeparator(), list5.Select((MatchedTrigger m) => ComboHintOverlayNode.FormatColoredText(m.Text, m.ColorHex)));
									list4.Add(safePlayerName2 + ModEntry.GetLocalizedHasConnector() + text2);
								}
							}
						}
						if (list4.Count > 0)
						{
							List<string> list6 = list;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
							defaultInterpolatedStringHandler.AppendLiteral("[color=");
							defaultInterpolatedStringHandler.AppendFormatted(ModEntry.OverlayKillTitleColorHex);
							defaultInterpolatedStringHandler.AppendLiteral("]");
							defaultInterpolatedStringHandler.AppendFormatted(ModEntry.GetLocalizedKillTitle());
							defaultInterpolatedStringHandler.AppendLiteral("[/color]");
							list6.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							list.AddRange(list4);
						}
						if (list.Count == 0)
						{
							this._titleLabel.SetTextAutoSize(ModEntry.GetLocalizedComboHintTitle());
							this._descriptionLabel.Text = ModEntry.GetLocalizedNoMatchText();
							this._hoverTipBox.Visible = true;
							this.ApplyIdleOpacity();
							this.LogStateIfChanged("no_matches_default_text", 1);
						}
						else
						{
							this._titleLabel.SetTextAutoSize(ModEntry.GetLocalizedComboHintTitle());
							this._descriptionLabel.Text = string.Join("\n", list);
							this._hoverTipBox.Visible = true;
							this.ApplyIdleOpacity();
							this.LogStateIfChanged("visible", list.Count);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] overlay refresh failed: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				Log.Warn(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
				ModEntry.LogErrorToFile("ComboHintOverlayNode.RefreshContent", ex);
				if (this._hoverTipBox != null && GodotObject.IsInstanceValid(this._hoverTipBox))
				{
					this._hoverTipBox.Visible = false;
				}
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006AAC File Offset: 0x00004CAC
		private void ApplyIdleOpacity()
		{
			if (this._hoverTipBox == null || !GodotObject.IsInstanceValid(this._hoverTipBox))
			{
				return;
			}
			if (this._hoverTipBox.Visible)
			{
				Rect2 globalRect = this._hoverTipBox.GetGlobalRect();
				Vector2 globalMousePosition = base.GetGlobalMousePosition();
				bool flag = globalRect.HasPoint(globalMousePosition);
				this.SetOverlayOpacity(flag, true);
				return;
			}
			this._isHovering = false;
			Color modulate = this._hoverTipBox.Modulate;
			modulate.A = 0.5f;
			this._hoverTipBox.Modulate = modulate;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00006B2C File Offset: 0x00004D2C
		private void SetOverlayOpacity(bool isHoveringNow, bool forceApplyAlpha = false)
		{
			if (this._hoverTipBox == null || !GodotObject.IsInstanceValid(this._hoverTipBox))
			{
				return;
			}
			float num = (isHoveringNow ? 1f : 0.5f);
			bool flag = Math.Abs(this._hoverTipBox.Modulate.A - num) < 0.001f;
			if (!forceApplyAlpha && isHoveringNow == this._isHovering && flag)
			{
				return;
			}
			this._isHovering = isHoveringNow;
			Color modulate = this._hoverTipBox.Modulate;
			modulate.A = num;
			this._hoverTipBox.Modulate = modulate;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00006BBC File Offset: 0x00004DBC
		private void LogStateIfChanged(string reason, int lineCount)
		{
			Node parent = base.GetParent();
			string text = ((parent != null) ? parent.Name : null) ?? "null";
			Node parent2 = base.GetParent();
			string text2 = ((parent2 != null) ? parent2.GetType().Name : null) ?? "null";
			bool flag = this._hoverTipBox != null && GodotObject.IsInstanceValid(this._hoverTipBox) && this._hoverTipBox.Visible;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(97, 13);
			defaultInterpolatedStringHandler.AppendLiteral("reason=");
			defaultInterpolatedStringHandler.AppendFormatted(reason);
			defaultInterpolatedStringHandler.AppendLiteral("|lineCount=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(lineCount);
			defaultInterpolatedStringHandler.AppendLiteral("|panelVisible=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(flag);
			defaultInterpolatedStringHandler.AppendLiteral("|isReady=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(this._isReady);
			defaultInterpolatedStringHandler.AppendLiteral("|isVisible=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(this._isVisible);
			defaultInterpolatedStringHandler.AppendLiteral("|parent=");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral("|parentType=");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			defaultInterpolatedStringHandler.AppendLiteral("|topLevel=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(base.TopLevel);
			defaultInterpolatedStringHandler.AppendLiteral("|z=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(base.ZIndex);
			defaultInterpolatedStringHandler.AppendLiteral("|offsets=");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetLeft);
			defaultInterpolatedStringHandler.AppendLiteral(",");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetTop);
			defaultInterpolatedStringHandler.AppendLiteral(",");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetRight);
			defaultInterpolatedStringHandler.AppendLiteral(",");
			defaultInterpolatedStringHandler.AppendFormatted<float>(base.OffsetBottom);
			string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
			if (text3 == this._lastStateSignature)
			{
				return;
			}
			this._lastStateSignature = text3;
			ModEntry.LogInfoToFile("ComboHintOverlayNode.State", text3);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006D94 File Offset: 0x00004F94
		private static string GetSafePlayerName(Creature playerCreature)
		{
			string text2;
			try
			{
				RunManager instance = RunManager.Instance;
				if (instance == null || instance.IsSinglePlayerOrFakeMultiplayer)
				{
					Player player = playerCreature.Player;
					string text;
					if (player == null)
					{
						text = null;
					}
					else
					{
						CharacterModel character = player.Character;
						text = ((character != null) ? character.Title.GetFormattedText() : null);
					}
					text2 = text ?? "玩家";
				}
				else
				{
					Player player2 = playerCreature.Player;
					ulong num = ((player2 != null) ? player2.NetId : 0UL);
					if (num == 0UL)
					{
						Player player3 = playerCreature.Player;
						string text3;
						if (player3 == null)
						{
							text3 = null;
						}
						else
						{
							CharacterModel character2 = player3.Character;
							text3 = ((character2 != null) ? character2.Title.GetFormattedText() : null);
						}
						text2 = text3 ?? "玩家";
					}
					else
					{
						text2 = PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, num);
					}
				}
			}
			catch
			{
				Player player4 = playerCreature.Player;
				string text4;
				if (player4 == null)
				{
					text4 = null;
				}
				else
				{
					CharacterModel character3 = player4.Character;
					text4 = ((character3 != null) ? character3.Title.GetFormattedText() : null);
				}
				text2 = text4 ?? "玩家";
			}
			return text2;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006E88 File Offset: 0x00005088
		private static List<MatchedTrigger> FindMatchedTextsInHand(IEnumerable<CardModel> handCards)
		{
			List<MatchedTrigger> list = new List<MatchedTrigger>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			foreach (CardModel cardModel in handCards)
			{
				foreach (MatchedTrigger matchedTrigger in ComboHintOverlayNode.FindMatchedTexts(cardModel))
				{
					if (hashSet.Add(ComboHintOverlayNode.BuildMatchDedupKey(matchedTrigger)))
					{
						list.Add(matchedTrigger);
					}
				}
			}
			return list;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006F2C File Offset: 0x0000512C
		private static List<MatchedTrigger> FindMatchedTextsInHandForGroup(IEnumerable<CardModel> handCards, TriggerGroup group)
		{
			List<MatchedTrigger> list = new List<MatchedTrigger>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			foreach (CardModel cardModel in handCards)
			{
				string entry = cardModel.Id.Entry;
				if (!string.IsNullOrWhiteSpace(entry) && group.TriggerModelIds.Contains(entry))
				{
					string displayCardTitleWithEnglish = ModEntry.GetDisplayCardTitleWithEnglish(cardModel);
					string text = group.ColorHex + "|" + displayCardTitleWithEnglish;
					if (hashSet.Add(text))
					{
						list.Add(new MatchedTrigger(displayCardTitleWithEnglish, group.ColorHex));
					}
				}
			}
			return list;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006FE0 File Offset: 0x000051E0
		private static List<MatchedTrigger> FindMatchedTexts(CardModel card)
		{
			List<MatchedTrigger> list = new List<MatchedTrigger>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			string entry = card.Id.Entry;
			if (string.IsNullOrWhiteSpace(entry))
			{
				return list;
			}
			string displayCardTitleWithEnglish = ModEntry.GetDisplayCardTitleWithEnglish(card);
			foreach (TriggerGroup triggerGroup in ModEntry.TriggerGroups)
			{
				if (triggerGroup.TriggerModelIds.Contains(entry))
				{
					string text;
					if (triggerGroup.Key.Equals("specialweaken", StringComparison.OrdinalIgnoreCase))
					{
						text = ModEntry.GetLocalizedWeakText();
					}
					else if (triggerGroup.Key.Equals("specialvulnerable", StringComparison.OrdinalIgnoreCase))
					{
						text = ModEntry.GetLocalizedVulnerableText();
					}
					else
					{
						text = displayCardTitleWithEnglish;
					}
					string text2 = triggerGroup.ColorHex + "|" + text;
					if (hashSet.Add(text2))
					{
						list.Add(new MatchedTrigger(text, triggerGroup.ColorHex));
					}
				}
			}
			return list;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000070E0 File Offset: 0x000052E0
		private static string BuildMatchDedupKey(MatchedTrigger match)
		{
			return match.ColorHex + "|" + match.Text;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000070FC File Offset: 0x000052FC
		private static string FormatColoredText(string text, string colorHex)
		{
			string text2 = text.Replace("[", "\\[").Replace("]", "\\]");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[color=");
			defaultInterpolatedStringHandler.AppendFormatted(colorHex);
			defaultInterpolatedStringHandler.AppendLiteral("]");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			defaultInterpolatedStringHandler.AppendLiteral("[/color]");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x04000058 RID: 88
		private const string HoverTipScenePath = "res://scenes/ui/hover_tip.tscn";

		// Token: 0x04000059 RID: 89
		private const float HoveredAlpha = 1f;

		// Token: 0x0400005A RID: 90
		private const float IdleAlpha = 0.5f;

		// Token: 0x0400005B RID: 91
		private const string SpecialWeakenGroupKey = "specialweaken";

		// Token: 0x0400005C RID: 92
		private const string SpecialVulnerableGroupKey = "specialvulnerable";

		// Token: 0x0400005D RID: 93
		private Control _hoverTipBox;

		// Token: 0x0400005E RID: 94
		private MegaLabel _titleLabel;

		// Token: 0x0400005F RID: 95
		private MegaRichTextLabel _descriptionLabel;

		// Token: 0x04000060 RID: 96
		private bool _isVisible;

		// Token: 0x04000061 RID: 97
		private bool _isReady;

		// Token: 0x04000062 RID: 98
		private bool _isHovering;

		// Token: 0x04000063 RID: 99
		private ulong _lastHoverAlphaLogFrame;

		// Token: 0x04000064 RID: 100
		private string _lastStateSignature = string.Empty;
	}
}
