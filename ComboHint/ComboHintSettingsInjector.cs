using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace ComboHint
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(0)]
	public static class ComboHintSettingsInjector
	{
		// Token: 0x06000060 RID: 96 RVA: 0x0000444D File Offset: 0x0000264D
		private static bool UseChineseUiText()
		{
			return ModEntry.IsChineseLanguage(LocManager.Instance.Language ?? "eng");
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004467 File Offset: 0x00002667
		private static string GetHeaderLabel()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "ComboHint Settings";
			}
			return "ComboHint设置";
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000447B File Offset: 0x0000267B
		private static string GetComboHintLabel()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Combo Hint Overlay";
			}
			return "连携提示框";
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000448F File Offset: 0x0000268F
		private static string GetComboHintHoverTipTitle()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Combo Hint Overlay";
			}
			return "连携提示框";
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000044A3 File Offset: 0x000026A3
		private static string GetComboHintHoverTipDescription()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Enable the combo hint overlay in the top-right corner.";
			}
			return "启用右上角连携提示框。";
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000044B7 File Offset: 0x000026B7
		private static string GetSinglePlayerLabel()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Single-Player Combo Hint";
			}
			return "单人连携";
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000044CB File Offset: 0x000026CB
		private static string GetSinglePlayerHoverTipTitle()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Single-Player Combo Hint";
			}
			return "单人连携";
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000044DF File Offset: 0x000026DF
		private static string GetSinglePlayerHoverTipDescription()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Enable ComboHint in single-player mode.";
			}
			return "开启后游玩单人模式时启用ComboHint。";
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000044F3 File Offset: 0x000026F3
		private static string GetBubbleHintLabel()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Bubble Hint";
			}
			return "气泡开关";
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004507 File Offset: 0x00002707
		private static string GetBubbleHintHoverTipTitle()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Bubble Hint";
			}
			return "气泡开关";
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000451B File Offset: 0x0000271B
		private static string GetBubbleHintHoverTipDescription()
		{
			if (!ComboHintSettingsInjector.UseChineseUiText())
			{
				return "Enable combo hints in character speech bubbles.";
			}
			return "启用角色对话气泡连携提示。";
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004530 File Offset: 0x00002730
		private static void EnsureToastLocalizationRegistered()
		{
			try
			{
				LocTable table = LocManager.Instance.GetTable("settings_ui");
				string text = LocManager.Instance.Language ?? "eng";
				bool flag = LocString.Exists("settings_ui", "TOAST_COMBO_HINT_OVERLAY_ON");
				if (!ComboHintSettingsInjector._toastLocalizationRegistered || !(ComboHintSettingsInjector._toastLocalizationLanguage == text) || !flag)
				{
					Dictionary<string, string> dictionary = (ModEntry.IsChineseLanguage(text) ? ComboHintSettingsInjector.ToastZh : ComboHintSettingsInjector.ToastEn);
					table.MergeWith(dictionary);
					ComboHintSettingsInjector._toastLocalizationRegistered = true;
					ComboHintSettingsInjector._toastLocalizationLanguage = text;
					string text2 = "SettingsToast.Register";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
					defaultInterpolatedStringHandler.AppendLiteral("language=");
					defaultInterpolatedStringHandler.AppendFormatted(text);
					defaultInterpolatedStringHandler.AppendLiteral(", keyCount=");
					defaultInterpolatedStringHandler.AppendFormatted<int>(dictionary.Count);
					ModEntry.LogUi(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			catch (Exception ex)
			{
				ModEntry.LogUi("SettingsToast.RegisterError", ex.Message);
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004628 File Offset: 0x00002828
		public static void ShowSettingsToast(NFastModeTickbox tickbox, string key)
		{
			ComboHintSettingsInjector.EnsureToastLocalizationRegistered();
			NSettingsScreen ancestorOfType = NodeUtil.GetAncestorOfType<NSettingsScreen>(tickbox);
			if (ancestorOfType == null)
			{
				string text = "Injector.Toast.Skip";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
				defaultInterpolatedStringHandler.AppendLiteral("tickbox=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(tickbox.Name);
				defaultInterpolatedStringHandler.AppendLiteral(", reason=no_settings_screen, key=");
				defaultInterpolatedStringHandler.AppendFormatted(key);
				ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
				string text2 = "SettingsToast.Skip";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
				defaultInterpolatedStringHandler.AppendLiteral("tickbox=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(tickbox.Name);
				defaultInterpolatedStringHandler.AppendLiteral(", reason=no_settings_screen, key=");
				defaultInterpolatedStringHandler.AppendFormatted(key);
				ModEntry.LogUi(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			string text3 = key;
			if (!LocString.Exists("settings_ui", text3))
			{
				text3 = "TOAST_NOT_IMPLEMENTED";
				ModEntry.LogUi("SettingsToast.Fallback", "missingKey=" + key + ", fallback=" + text3);
			}
			try
			{
				ancestorOfType.ShowToast(new LocString("settings_ui", text3));
				string text4 = "SettingsToast.Show";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
				defaultInterpolatedStringHandler.AppendLiteral("tickbox=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(tickbox.Name);
				defaultInterpolatedStringHandler.AppendLiteral(", key=");
				defaultInterpolatedStringHandler.AppendFormatted(text3);
				ModEntry.LogUi(text4, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex)
			{
				string text5 = "SettingsToast.Error";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
				defaultInterpolatedStringHandler.AppendLiteral("tickbox=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(tickbox.Name);
				defaultInterpolatedStringHandler.AppendLiteral(", key=");
				defaultInterpolatedStringHandler.AppendFormatted(text3);
				defaultInterpolatedStringHandler.AppendLiteral(", error=");
				defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
				ModEntry.LogUi(text5, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000047D8 File Offset: 0x000029D8
		public static void TryInject(NSettingsScreen screen)
		{
			try
			{
				string text = "Injector.TryInject.Start";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral("screen=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(screen.Name);
				ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
				NSettingsPanel nodeOrNull = screen.GetNodeOrNull<NSettingsPanel>("%GeneralSettings");
				if (nodeOrNull == null)
				{
					ModEntry.LogInject("Injector.TryInject.Skip", "general settings panel not found");
				}
				else
				{
					VBoxContainer content = nodeOrNull.Content;
					string text2 = "Injector.TryInject.Panel";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
					defaultInterpolatedStringHandler.AppendLiteral("contentNode=");
					defaultInterpolatedStringHandler.AppendFormatted<StringName>((content != null) ? content.Name : null);
					defaultInterpolatedStringHandler.AppendLiteral(", childCount=");
					defaultInterpolatedStringHandler.AppendFormatted<int?>((content != null) ? new int?(content.GetChildCount(false)) : null);
					ModEntry.LogInject(text2, defaultInterpolatedStringHandler.ToStringAndClear());
					ComboHintSettingsInjector.LogDirectChildren(content);
					if (content == null)
					{
						ModEntry.LogInject("Injector.TryInject.Skip", "contentNull=true");
					}
					else
					{
						Control control = content.GetChildren(false).FirstOrDefault((Node n) => n.Name == "FastMode") as Control;
						if (control == null)
						{
							ModEntry.LogInject("Injector.TryInject.Skip", "FastMode row not found by child-name scan");
						}
						else
						{
							string text3 = "Injector.TryInject.FastMode";
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 3);
							defaultInterpolatedStringHandler.AppendLiteral("rowName=");
							defaultInterpolatedStringHandler.AppendFormatted<StringName>(control.Name);
							defaultInterpolatedStringHandler.AppendLiteral(", fastModeIndex=");
							defaultInterpolatedStringHandler.AppendFormatted<int>(control.GetIndex(false));
							defaultInterpolatedStringHandler.AppendLiteral(", rowType=");
							defaultInterpolatedStringHandler.AppendFormatted(control.GetType().Name);
							ModEntry.LogInject(text3, defaultInterpolatedStringHandler.ToStringAndClear());
							NFastModeTickbox nfastModeTickbox = control.FindChild("FastModeTickbox", true, false) as NFastModeTickbox;
							if (nfastModeTickbox == null)
							{
								ModEntry.LogInject("Injector.TryInject.Skip", "FastMode tickbox not found in source row");
							}
							else
							{
								ValueTuple<Control, NFastModeTickbox> valueTuple = ComboHintSettingsInjector.EnsureSettingRow(content, control, "ComboHint", "ComboHintDivider", "ComboHintTickbox", ComboHintSettingsInjector.GetComboHintLabel(), ComboHintSettingsInjector.GetComboHintHoverTipTitle(), ComboHintSettingsInjector.GetComboHintHoverTipDescription());
								Control control2 = ComboHintSettingsInjector.EnsureHeaderRow(content, control, "ComboHintHeader", "ComboHintHeaderDivider", ComboHintSettingsInjector.GetHeaderLabel());
								ValueTuple<Control, NFastModeTickbox> valueTuple2 = ComboHintSettingsInjector.EnsureSettingRow(content, control, "SinglePlayerComboHint", "SinglePlayerComboHintDivider", "SinglePlayerComboHintTickbox", ComboHintSettingsInjector.GetSinglePlayerLabel(), ComboHintSettingsInjector.GetSinglePlayerHoverTipTitle(), ComboHintSettingsInjector.GetSinglePlayerHoverTipDescription());
								ValueTuple<Control, NFastModeTickbox> valueTuple3 = ComboHintSettingsInjector.EnsureSettingRow(content, control, "BubbleHint", "BubbleHintDivider", "BubbleHintTickbox", ComboHintSettingsInjector.GetBubbleHintLabel(), ComboHintSettingsInjector.GetBubbleHintHoverTipTitle(), ComboHintSettingsInjector.GetBubbleHintHoverTipDescription());
								ComboHintSettingsInjector.MoveRowWithDividerToBottom(content, control, control2, "ComboHintHeaderDivider");
								ComboHintSettingsInjector.MoveRowWithDividerToBottom(content, control, valueTuple.Item1, "ComboHintDivider");
								ComboHintSettingsInjector.MoveRowWithDividerToBottom(content, control, valueTuple2.Item1, "SinglePlayerComboHintDivider");
								ComboHintSettingsInjector.MoveRowWithDividerToBottom(content, control, valueTuple3.Item1, "BubbleHintDivider");
								valueTuple.Item2.CallDeferred("SetFromSettings", Array.Empty<Variant>());
								valueTuple2.Item2.CallDeferred("SetFromSettings", Array.Empty<Variant>());
								valueTuple3.Item2.CallDeferred("SetFromSettings", Array.Empty<Variant>());
								Control control3 = content.GetNodeOrNull<Control>("ShowRunTimer/SettingsTickbox");
								if (control3 == null)
								{
									control3 = ComboHintSettingsInjector.FindNextTickboxBelow(content, valueTuple3.Item1);
								}
								nfastModeTickbox.FocusNeighborBottom = valueTuple.Item2.GetPath();
								valueTuple.Item2.FocusNeighborTop = nfastModeTickbox.GetPath();
								valueTuple.Item2.FocusNeighborBottom = valueTuple2.Item2.GetPath();
								valueTuple2.Item2.FocusNeighborTop = valueTuple.Item2.GetPath();
								valueTuple2.Item2.FocusNeighborBottom = valueTuple3.Item2.GetPath();
								valueTuple3.Item2.FocusNeighborTop = valueTuple2.Item2.GetPath();
								valueTuple3.Item2.FocusNeighborBottom = ((control3 != null) ? control3.GetPath() : null) ?? valueTuple3.Item2.GetPath();
								if (control3 != null)
								{
									control3.FocusNeighborTop = valueTuple3.Item2.GetPath();
								}
								ComboHintSettingsInjector.RefreshPanelSizeAfterInjection(nodeOrNull, content);
								string text4 = "Injector.TryInject.Success";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(71, 4);
								defaultInterpolatedStringHandler.AppendLiteral("comboTickbox=");
								defaultInterpolatedStringHandler.AppendFormatted<NodePath>(valueTuple.Item2.GetPath());
								defaultInterpolatedStringHandler.AppendLiteral(", singlePlayerTickbox=");
								defaultInterpolatedStringHandler.AppendFormatted<NodePath>(valueTuple2.Item2.GetPath());
								defaultInterpolatedStringHandler.AppendLiteral(", bubbleTickbox=");
								defaultInterpolatedStringHandler.AppendFormatted<NodePath>(valueTuple3.Item2.GetPath());
								defaultInterpolatedStringHandler.AppendLiteral(", showRunTimerFound=");
								defaultInterpolatedStringHandler.AppendFormatted<bool>(control3 != null);
								ModEntry.LogInject(text4, defaultInterpolatedStringHandler.ToStringAndClear());
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ModEntry.LogInject("Injector.TryInject.Error", ex.ToString());
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004C88 File Offset: 0x00002E88
		[return: TupleElementNames(new string[] { "Row", "Tickbox" })]
		[return: Nullable(new byte[] { 0, 1, 1 })]
		private static ValueTuple<Control, NFastModeTickbox> EnsureSettingRow(VBoxContainer content, Control sourceRow, string rowName, string dividerName, string tickboxName, string labelText, string hoverTitle, string hoverDescription)
		{
			MarginContainer marginContainer = content.GetNodeOrNull<Control>(rowName) as MarginContainer;
			if (marginContainer != null)
			{
				ComboHintSettingsInjector.ApplyRowHorizontalNudges(marginContainer, sourceRow);
				ComboHintSettingsInjector.EnsureHoverTipForRow(marginContainer, hoverTitle, hoverDescription);
				ComboHintSettingsInjector.EnsureRowLabel(marginContainer, labelText);
				NFastModeTickbox nfastModeTickbox = marginContainer.FindChild(tickboxName, true, false) as NFastModeTickbox;
				if (nfastModeTickbox == null)
				{
					nfastModeTickbox = marginContainer.FindChild("FastModeTickbox", true, false) as NFastModeTickbox;
					if (nfastModeTickbox != null)
					{
						nfastModeTickbox.Name = tickboxName;
					}
				}
				if (nfastModeTickbox != null)
				{
					ComboHintSettingsInjector.MakeTickboxVisualMaterialUnique(nfastModeTickbox);
					return new ValueTuple<Control, NFastModeTickbox>(marginContainer, nfastModeTickbox);
				}
				ComboHintSettingsInjector.RemoveTickboxesFromRow(marginContainer);
			}
			MarginContainer marginContainer2 = new MarginContainer();
			ComboHintSettingsInjector.CopyControlLayout(sourceRow, marginContainer2);
			ComboHintSettingsInjector.ApplyRowHorizontalNudges(marginContainer2, sourceRow);
			marginContainer2.MouseFilter = sourceRow.MouseFilter;
			marginContainer2.FocusMode = sourceRow.FocusMode;
			marginContainer2.Name = rowName;
			Control control = sourceRow.FindChild("Label", true, false) as Control;
			if (control == null)
			{
				throw new InvalidOperationException("FastMode label not found");
			}
			Node node = control.Duplicate(15);
			marginContainer2.AddChild(node, false, 0L);
			MegaRichTextLabel megaRichTextLabel = node as MegaRichTextLabel;
			if (megaRichTextLabel != null)
			{
				megaRichTextLabel.Text = labelText;
			}
			NFastModeTickbox nfastModeTickbox2 = sourceRow.FindChild("FastModeTickbox", true, false) as NFastModeTickbox;
			if (nfastModeTickbox2 == null)
			{
				throw new InvalidOperationException("FastMode tickbox not found in source row");
			}
			Node node2 = nfastModeTickbox2.Duplicate(15);
			marginContainer2.AddChild(node2, false, 0L);
			NFastModeTickbox nfastModeTickbox3 = node2 as NFastModeTickbox;
			if (nfastModeTickbox3 == null)
			{
				throw new InvalidOperationException("copied FastMode tickbox cast failed");
			}
			nfastModeTickbox3.Name = tickboxName;
			ComboHintSettingsInjector.MakeTickboxVisualMaterialUnique(nfastModeTickbox3);
			ComboHintSettingsInjector.EnsureHoverTipForRow(marginContainer2, hoverTitle, hoverDescription);
			ColorRect orCreateDivider = ComboHintSettingsInjector.GetOrCreateDivider(content, sourceRow, dividerName);
			if (orCreateDivider.GetParent() != content)
			{
				content.AddChild(orCreateDivider, false, 0L);
			}
			if (marginContainer2.GetParent() != content)
			{
				content.AddChild(marginContainer2, false, 0L);
			}
			string text = "Injector.TryInject.Inserted";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
			defaultInterpolatedStringHandler.AppendLiteral("row=");
			defaultInterpolatedStringHandler.AppendFormatted(rowName);
			defaultInterpolatedStringHandler.AppendLiteral(", dividerIndex=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(orCreateDivider.GetIndex(false));
			defaultInterpolatedStringHandler.AppendLiteral(", rowIndex=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(marginContainer2.GetIndex(false));
			ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
			return new ValueTuple<Control, NFastModeTickbox>(marginContainer2, nfastModeTickbox3);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004EA0 File Offset: 0x000030A0
		private static Control EnsureHeaderRow(VBoxContainer content, Control sourceRow, string rowName, string dividerName, string labelText)
		{
			MarginContainer marginContainer = content.GetNodeOrNull<Control>(rowName) as MarginContainer;
			if (marginContainer != null)
			{
				ComboHintSettingsInjector.ApplyRowHorizontalNudges(marginContainer, sourceRow);
				ComboHintSettingsInjector.EnsureRowLabel(marginContainer, labelText);
				ComboHintSettingsInjector.RemoveTickboxesFromRow(marginContainer);
				return marginContainer;
			}
			MarginContainer marginContainer2 = new MarginContainer();
			ComboHintSettingsInjector.CopyControlLayout(sourceRow, marginContainer2);
			ComboHintSettingsInjector.ApplyRowHorizontalNudges(marginContainer2, sourceRow);
			marginContainer2.MouseFilter = sourceRow.MouseFilter;
			marginContainer2.FocusMode = sourceRow.FocusMode;
			marginContainer2.Name = rowName;
			Control control = sourceRow.FindChild("Label", true, false) as Control;
			if (control == null)
			{
				throw new InvalidOperationException("FastMode label not found");
			}
			Node node = control.Duplicate(15);
			marginContainer2.AddChild(node, false, 0L);
			MegaRichTextLabel megaRichTextLabel = node as MegaRichTextLabel;
			if (megaRichTextLabel != null)
			{
				megaRichTextLabel.Text = labelText;
			}
			ComboHintSettingsInjector.RemoveTickboxesFromRow(marginContainer2);
			ColorRect orCreateDivider = ComboHintSettingsInjector.GetOrCreateDivider(content, sourceRow, dividerName);
			if (orCreateDivider.GetParent() != content)
			{
				content.AddChild(orCreateDivider, false, 0L);
			}
			if (marginContainer2.GetParent() != content)
			{
				content.AddChild(marginContainer2, false, 0L);
			}
			string text = "Injector.TryInject.Inserted";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
			defaultInterpolatedStringHandler.AppendLiteral("row=");
			defaultInterpolatedStringHandler.AppendFormatted(rowName);
			defaultInterpolatedStringHandler.AppendLiteral(", dividerIndex=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(orCreateDivider.GetIndex(false));
			defaultInterpolatedStringHandler.AppendLiteral(", rowIndex=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(marginContainer2.GetIndex(false));
			ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
			return marginContainer2;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004FF4 File Offset: 0x000031F4
		private static void MoveRowWithDividerToBottom(VBoxContainer content, Control sourceRow, Control row, string dividerName)
		{
			ColorRect orCreateDivider = ComboHintSettingsInjector.GetOrCreateDivider(content, sourceRow, dividerName);
			if (orCreateDivider.GetParent() != content)
			{
				content.AddChild(orCreateDivider, false, 0L);
			}
			if (row.GetParent() != content)
			{
				content.AddChild(row, false, 0L);
			}
			content.MoveChild(orCreateDivider, content.GetChildCount(false) - 1);
			content.MoveChild(row, content.GetChildCount(false) - 1);
			string text = "Injector.TryInject.BottomAnchor";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 3);
			defaultInterpolatedStringHandler.AppendLiteral("row=");
			defaultInterpolatedStringHandler.AppendFormatted<StringName>(row.Name);
			defaultInterpolatedStringHandler.AppendLiteral(", divider=");
			defaultInterpolatedStringHandler.AppendFormatted<StringName>(orCreateDivider.Name);
			defaultInterpolatedStringHandler.AppendLiteral(", rowIndex=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(row.GetIndex(false));
			ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000050B8 File Offset: 0x000032B8
		private static void RefreshPanelSizeAfterInjection(NSettingsPanel panel, VBoxContainer content)
		{
			Control parent = panel.GetParent<Control>();
			if (parent == null)
			{
				return;
			}
			Vector2 size = parent.Size;
			Vector2 minimumSize = content.GetMinimumSize();
			float num = minimumSize.Y;
			if (minimumSize.Y + 50f >= size.Y)
			{
				num = minimumSize.Y + size.Y * 0.4f;
			}
			panel.Size = new Vector2(content.Size.X, num);
			string text = "Injector.TryInject.PanelResized";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 3);
			defaultInterpolatedStringHandler.AppendLiteral("panelHeight=");
			defaultInterpolatedStringHandler.AppendFormatted<float>(num);
			defaultInterpolatedStringHandler.AppendLiteral(", contentMinY=");
			defaultInterpolatedStringHandler.AppendFormatted<float>(minimumSize.Y);
			defaultInterpolatedStringHandler.AppendLiteral(", parentY=");
			defaultInterpolatedStringHandler.AppendFormatted<float>(size.Y);
			ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00005188 File Offset: 0x00003388
		private static ColorRect GetOrCreateDivider(VBoxContainer content, Control sourceRow, string dividerName)
		{
			ColorRect nodeOrNull = content.GetNodeOrNull<ColorRect>(dividerName);
			if (nodeOrNull != null)
			{
				return nodeOrNull;
			}
			ColorRect colorRect = new ColorRect();
			Node node = ((sourceRow.GetIndex(false) > 0) ? content.GetChild(sourceRow.GetIndex(false) - 1, false) : null);
			if (!(node is ColorRect))
			{
				node = content.GetNodeOrNull("CombatSpeedDivider");
			}
			ColorRect colorRect2 = node as ColorRect;
			if (colorRect2 != null)
			{
				ColorRect colorRect3 = colorRect2.Duplicate(15) as ColorRect;
				if (colorRect3 != null)
				{
					colorRect = colorRect3;
				}
			}
			colorRect.Name = dividerName;
			return colorRect;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005214 File Offset: 0x00003414
		[return: Nullable(2)]
		private static Control FindNextTickboxBelow(VBoxContainer content, Control currentRow)
		{
			for (int i = currentRow.GetIndex(false) + 1; i < content.GetChildCount(false); i++)
			{
				Control control = content.GetChild(i, false) as Control;
				if (control != null)
				{
					Control control2 = control.GetChildren(false).OfType<Control>().FirstOrDefault((Control c) => c is NSettingsTickbox);
					if (control2 != null)
					{
						return control2;
					}
				}
			}
			return null;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005284 File Offset: 0x00003484
		private static void LogDirectChildren(VBoxContainer content)
		{
			List<string> list = new List<string>();
			foreach (Node node in content.GetChildren(false))
			{
				List<string> list2 = list;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(node.Name);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted(node.GetType().Name);
				list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			ModEntry.LogInject("Injector.TryInject.Children", string.Join(" | ", list));
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005328 File Offset: 0x00003528
		private static void CopyControlLayout(Control source, Control target)
		{
			target.CustomMinimumSize = source.CustomMinimumSize;
			target.SizeFlagsHorizontal = source.SizeFlagsHorizontal;
			target.SizeFlagsVertical = source.SizeFlagsVertical;
			target.AnchorLeft = source.AnchorLeft;
			target.AnchorTop = source.AnchorTop;
			target.AnchorRight = source.AnchorRight;
			target.AnchorBottom = source.AnchorBottom;
			target.OffsetLeft = source.OffsetLeft;
			target.OffsetTop = source.OffsetTop;
			target.OffsetRight = source.OffsetRight;
			target.OffsetBottom = source.OffsetBottom;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000053BC File Offset: 0x000035BC
		private static void ApplyRowHorizontalNudges(MarginContainer row, Control source)
		{
			int num = ComboHintSettingsInjector.TryGetMarginConstant(source, "margin_left", 2);
			int num2 = ComboHintSettingsInjector.TryGetMarginConstant(source, "margin_right", 4);
			int num3 = ComboHintSettingsInjector.TryGetMarginConstant(source, "margin_top", 0);
			int num4 = ComboHintSettingsInjector.TryGetMarginConstant(source, "margin_bottom", 0);
			int num5 = num + 1;
			int num6 = num2 + 1;
			row.AddThemeConstantOverride("margin_left", num5);
			row.AddThemeConstantOverride("margin_top", num3);
			row.AddThemeConstantOverride("margin_right", num6);
			row.AddThemeConstantOverride("margin_bottom", num4);
			string text = "Injector.TryInject.Nudge";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 5);
			defaultInterpolatedStringHandler.AppendLiteral("sourceType=");
			defaultInterpolatedStringHandler.AppendFormatted(source.GetType().Name);
			defaultInterpolatedStringHandler.AppendLiteral(", margin_left=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(num5);
			defaultInterpolatedStringHandler.AppendLiteral(", margin_right=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(num6);
			defaultInterpolatedStringHandler.AppendLiteral(", margin_top=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(num3);
			defaultInterpolatedStringHandler.AppendLiteral(", margin_bottom=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(num4);
			ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000054D4 File Offset: 0x000036D4
		private static void EnsureHoverTipForRow(Control row, string title, string description)
		{
			if (row.HasMeta("combo_hint_hover_tip_bound"))
			{
				row.SetMeta("combo_hint_hover_tip_title", title);
				row.SetMeta("combo_hint_hover_tip_description", description);
				return;
			}
			row.SetMeta("combo_hint_hover_tip_bound", true);
			row.SetMeta("combo_hint_hover_tip_title", title);
			row.SetMeta("combo_hint_hover_tip_description", description);
			row.MouseEntered += delegate
			{
				ComboHintSettingsInjector.ShowHoverTip(row);
			};
			row.MouseExited += delegate
			{
				ComboHintSettingsInjector.HideHoverTip(row);
			};
			row.TreeExiting += delegate
			{
				ComboHintSettingsInjector.HideHoverTip(row);
			};
			string text = "Injector.TryInject.HoverTip.Bound";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler.AppendLiteral("row=");
			defaultInterpolatedStringHandler.AppendFormatted<StringName>(row.Name);
			ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000560C File Offset: 0x0000380C
		private static void EnsureRowLabel(Control row, string labelText)
		{
			MegaRichTextLabel megaRichTextLabel = (row.FindChild("Label", true, false) as Control) as MegaRichTextLabel;
			if (megaRichTextLabel != null)
			{
				megaRichTextLabel.Text = labelText;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000563C File Offset: 0x0000383C
		private static void RemoveTickboxesFromRow(Control row)
		{
			foreach (NFastModeTickbox nfastModeTickbox in row.GetChildren(false).OfType<NFastModeTickbox>().ToList<NFastModeTickbox>())
			{
				nfastModeTickbox.QueueFree();
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005698 File Offset: 0x00003898
		private static void ShowHoverTip(Control row)
		{
			Control control;
			if (ComboHintSettingsInjector.ActiveHoverTipsByRow.TryGetValue(row, out control) && GodotObject.IsInstanceValid(control))
			{
				return;
			}
			NGame instance = NGame.Instance;
			if (((instance != null) ? instance.HoverTipsContainer : null) == null)
			{
				ModEntry.LogInject("Injector.HoverTip.Skip", "hover tips container missing");
				return;
			}
			Control control2 = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip.tscn").Instantiate<Control>(0L);
			control2.Name = "ComboHintSettingHoverTip";
			control2.CustomMinimumSize = new Vector2(360f, control2.CustomMinimumSize.Y);
			control2.Size = new Vector2(360f, control2.Size.Y);
			string comboHintHoverTipTitle = ComboHintSettingsInjector.GetComboHintHoverTipTitle();
			string comboHintHoverTipDescription = ComboHintSettingsInjector.GetComboHintHoverTipDescription();
			string text = (row.HasMeta("combo_hint_hover_tip_title") ? (row.GetMeta("combo_hint_hover_tip_title", default(Variant)).ToString() ?? comboHintHoverTipTitle) : comboHintHoverTipTitle);
			string text2 = (row.HasMeta("combo_hint_hover_tip_description") ? (row.GetMeta("combo_hint_hover_tip_description", default(Variant)).ToString() ?? comboHintHoverTipDescription) : comboHintHoverTipDescription);
			MegaLabel nodeOrNull = control2.GetNodeOrNull<MegaLabel>("%Title");
			if (nodeOrNull != null)
			{
				nodeOrNull.SetTextAutoSize(text);
			}
			MegaRichTextLabel nodeOrNull2 = control2.GetNodeOrNull<MegaRichTextLabel>("%Description");
			if (nodeOrNull2 != null)
			{
				nodeOrNull2.Text = text2;
			}
			TextureRect nodeOrNull3 = control2.GetNodeOrNull<TextureRect>("%Icon");
			if (nodeOrNull3 != null)
			{
				nodeOrNull3.Visible = false;
			}
			NGame.Instance.HoverTipsContainer.AddChild(control2, false, 0L);
			control2.GlobalPosition = row.GlobalPosition + NSettingsScreen.settingTipsOffset;
			ComboHintSettingsInjector.ActiveHoverTipsByRow[row] = control2;
			string text3 = "Injector.HoverTip.Show";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
			defaultInterpolatedStringHandler.AppendLiteral("row=");
			defaultInterpolatedStringHandler.AppendFormatted<StringName>(row.Name);
			defaultInterpolatedStringHandler.AppendLiteral(", pos=");
			defaultInterpolatedStringHandler.AppendFormatted<Vector2>(control2.GlobalPosition);
			ModEntry.LogInject(text3, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000058B4 File Offset: 0x00003AB4
		private static void HideHoverTip(Control row)
		{
			Control control;
			if (ComboHintSettingsInjector.ActiveHoverTipsByRow.TryGetValue(row, out control))
			{
				if (GodotObject.IsInstanceValid(control))
				{
					control.QueueFree();
				}
				ComboHintSettingsInjector.ActiveHoverTipsByRow.Remove(row);
				string text = "Injector.HoverTip.Hide";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
				defaultInterpolatedStringHandler.AppendLiteral("row=");
				defaultInterpolatedStringHandler.AppendFormatted<StringName>(row.Name);
				ModEntry.LogInject(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005920 File Offset: 0x00003B20
		private static int TryGetMarginConstant(Control source, string key, int fallback)
		{
			int num;
			try
			{
				num = source.GetThemeConstant(key, "MarginContainer");
			}
			catch
			{
				num = fallback;
			}
			return num;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000595C File Offset: 0x00003B5C
		private static void MakeTickboxVisualMaterialUnique(NFastModeTickbox tickbox)
		{
			Control nodeOrNull = tickbox.GetNodeOrNull<Control>("%TickboxVisuals");
			if (nodeOrNull == null)
			{
				return;
			}
			if (nodeOrNull.HasMeta("combo_hint_tickbox_material_bound"))
			{
				return;
			}
			ShaderMaterial shaderMaterial = nodeOrNull.Material as ShaderMaterial;
			if (shaderMaterial != null)
			{
				ShaderMaterial shaderMaterial2 = shaderMaterial.Duplicate(false) as ShaderMaterial;
				if (shaderMaterial2 != null)
				{
					shaderMaterial2.ResourceLocalToScene = true;
					nodeOrNull.Material = shaderMaterial2;
					nodeOrNull.SetMeta("combo_hint_tickbox_material_bound", true);
				}
				return;
			}
			string text = "TickboxMaterial.Skip";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("tickbox=");
			defaultInterpolatedStringHandler.AppendFormatted<StringName>(tickbox.Name);
			defaultInterpolatedStringHandler.AppendLiteral(", reason=non_shader_material");
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04000023 RID: 35
		private const string ComboHintHeaderRowName = "ComboHintHeader";

		// Token: 0x04000024 RID: 36
		private const string ComboHintRowName = "ComboHint";

		// Token: 0x04000025 RID: 37
		private const string SinglePlayerComboHintRowName = "SinglePlayerComboHint";

		// Token: 0x04000026 RID: 38
		private const string BubbleHintRowName = "BubbleHint";

		// Token: 0x04000027 RID: 39
		private const string ComboHintHeaderDividerName = "ComboHintHeaderDivider";

		// Token: 0x04000028 RID: 40
		private const string ComboHintDividerName = "ComboHintDivider";

		// Token: 0x04000029 RID: 41
		private const string SinglePlayerComboHintDividerName = "SinglePlayerComboHintDivider";

		// Token: 0x0400002A RID: 42
		private const string BubbleHintDividerName = "BubbleHintDivider";

		// Token: 0x0400002B RID: 43
		public const string ComboHintTickboxName = "ComboHintTickbox";

		// Token: 0x0400002C RID: 44
		public const string SinglePlayerComboHintTickboxName = "SinglePlayerComboHintTickbox";

		// Token: 0x0400002D RID: 45
		public const string BubbleHintTickboxName = "BubbleHintTickbox";

		// Token: 0x0400002E RID: 46
		public const string ToastOverlayOnKey = "TOAST_COMBO_HINT_OVERLAY_ON";

		// Token: 0x0400002F RID: 47
		public const string ToastOverlayOffKey = "TOAST_COMBO_HINT_OVERLAY_OFF";

		// Token: 0x04000030 RID: 48
		public const string ToastSinglePlayerOnKey = "TOAST_COMBO_HINT_SINGLE_PLAYER_ON";

		// Token: 0x04000031 RID: 49
		public const string ToastSinglePlayerOffKey = "TOAST_COMBO_HINT_SINGLE_PLAYER_OFF";

		// Token: 0x04000032 RID: 50
		public const string ToastBubbleOnKey = "TOAST_COMBO_HINT_BUBBLE_ON";

		// Token: 0x04000033 RID: 51
		public const string ToastBubbleOffKey = "TOAST_COMBO_HINT_BUBBLE_OFF";

		// Token: 0x04000034 RID: 52
		private const string HoverTipScenePath = "res://scenes/ui/hover_tip.tscn";

		// Token: 0x04000035 RID: 53
		private const string ComboHintHeaderZh = "ComboHint设置";

		// Token: 0x04000036 RID: 54
		private const string ComboHintHeaderEn = "ComboHint Settings";

		// Token: 0x04000037 RID: 55
		private const string ComboHintLabelZh = "连携提示框";

		// Token: 0x04000038 RID: 56
		private const string ComboHintLabelEn = "Combo Hint Overlay";

		// Token: 0x04000039 RID: 57
		private const string ComboHintHoverTipTitleZh = "连携提示框";

		// Token: 0x0400003A RID: 58
		private const string ComboHintHoverTipTitleEn = "Combo Hint Overlay";

		// Token: 0x0400003B RID: 59
		private const string ComboHintHoverTipDescriptionZh = "启用右上角连携提示框。";

		// Token: 0x0400003C RID: 60
		private const string ComboHintHoverTipDescriptionEn = "Enable the combo hint overlay in the top-right corner.";

		// Token: 0x0400003D RID: 61
		private const string SinglePlayerComboHintLabelZh = "单人连携";

		// Token: 0x0400003E RID: 62
		private const string SinglePlayerComboHintLabelEn = "Single-Player Combo Hint";

		// Token: 0x0400003F RID: 63
		private const string SinglePlayerComboHintHoverTipTitleZh = "单人连携";

		// Token: 0x04000040 RID: 64
		private const string SinglePlayerComboHintHoverTipTitleEn = "Single-Player Combo Hint";

		// Token: 0x04000041 RID: 65
		private const string SinglePlayerComboHintHoverTipDescriptionZh = "开启后游玩单人模式时启用ComboHint。";

		// Token: 0x04000042 RID: 66
		private const string SinglePlayerComboHintHoverTipDescriptionEn = "Enable ComboHint in single-player mode.";

		// Token: 0x04000043 RID: 67
		private const string BubbleHintLabelZh = "气泡开关";

		// Token: 0x04000044 RID: 68
		private const string BubbleHintLabelEn = "Bubble Hint";

		// Token: 0x04000045 RID: 69
		private const string BubbleHintHoverTipTitleZh = "气泡开关";

		// Token: 0x04000046 RID: 70
		private const string BubbleHintHoverTipTitleEn = "Bubble Hint";

		// Token: 0x04000047 RID: 71
		private const string BubbleHintHoverTipDescriptionZh = "启用角色对话气泡连携提示。";

		// Token: 0x04000048 RID: 72
		private const string BubbleHintHoverTipDescriptionEn = "Enable combo hints in character speech bubbles.";

		// Token: 0x04000049 RID: 73
		private const string HoverTipBoundMetaKey = "combo_hint_hover_tip_bound";

		// Token: 0x0400004A RID: 74
		private const string HoverTipTitleMetaKey = "combo_hint_hover_tip_title";

		// Token: 0x0400004B RID: 75
		private const string HoverTipDescriptionMetaKey = "combo_hint_hover_tip_description";

		// Token: 0x0400004C RID: 76
		private const float ComboHintHoverTipWidth = 360f;

		// Token: 0x0400004D RID: 77
		private const int ComboHintLabelRightNudgePx = 1;

		// Token: 0x0400004E RID: 78
		private const int ComboHintTickboxLeftNudgePx = 1;

		// Token: 0x0400004F RID: 79
		private const string TickboxMaterialBoundMetaKey = "combo_hint_tickbox_material_bound";

		// Token: 0x04000050 RID: 80
		private static readonly Dictionary<Control, Control> ActiveHoverTipsByRow = new Dictionary<Control, Control>();

		// Token: 0x04000051 RID: 81
		private static bool _toastLocalizationRegistered;

		// Token: 0x04000052 RID: 82
		private static string _toastLocalizationLanguage = string.Empty;

		// Token: 0x04000053 RID: 83
		private static readonly Dictionary<string, string> ToastZh = new Dictionary<string, string>
		{
			{ "TOAST_COMBO_HINT_OVERLAY_ON", "战斗场景中右上方显示连携提示框" },
			{ "TOAST_COMBO_HINT_OVERLAY_OFF", "已关闭右上方连携提示框" },
			{ "TOAST_COMBO_HINT_SINGLE_PLAYER_ON", "单人模式下启用连携提示" },
			{ "TOAST_COMBO_HINT_SINGLE_PLAYER_OFF", "单人模式下不使用连携提示" },
			{ "TOAST_COMBO_HINT_BUBBLE_ON", "开启角色气泡连携提示" },
			{ "TOAST_COMBO_HINT_BUBBLE_OFF", "关闭角色气泡连携提示" }
		};

		// Token: 0x04000054 RID: 84
		private static readonly Dictionary<string, string> ToastEn = new Dictionary<string, string>
		{
			{ "TOAST_COMBO_HINT_OVERLAY_ON", "Display combo hints in the top-right corner of battle scenes" },
			{ "TOAST_COMBO_HINT_OVERLAY_OFF", "Combo hints in the top-right corner of battle scenes disabled" },
			{ "TOAST_COMBO_HINT_SINGLE_PLAYER_ON", "Enable combo hints in single-player mode" },
			{ "TOAST_COMBO_HINT_SINGLE_PLAYER_OFF", "Disable combo hints in single-player mode" },
			{ "TOAST_COMBO_HINT_BUBBLE_ON", "Enable character bubble combo hints" },
			{ "TOAST_COMBO_HINT_BUBBLE_OFF", "Disable character bubble combo hints" }
		};
	}
}
