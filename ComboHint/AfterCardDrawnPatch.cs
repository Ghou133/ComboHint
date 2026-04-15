using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace ComboHint
{
	// Token: 0x02000003 RID: 3
	[NullableContext(1)]
	[Nullable(0)]
	[HarmonyPatch(typeof(Hook), "AfterCardChangedPiles")]
	public static class AfterCardDrawnPatch
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00003258 File Offset: 0x00001458
		public static void EnsureCombatStateFresh()
		{
			NCombatRoom instance = NCombatRoom.Instance;
			ulong num = ((instance != null) ? instance.CreatedMsec : 0UL);
			if (num == 0UL || num == AfterCardDrawnPatch._lastCombatCreatedMsec)
			{
				return;
			}
			Dictionary<ulong, int> pendingDrawVersionByPlayer = AfterCardDrawnPatch.PendingDrawVersionByPlayer;
			lock (pendingDrawVersionByPlayer)
			{
				AfterCardDrawnPatch.PendingDrawVersionByPlayer.Clear();
			}
			Dictionary<ulong, AfterCardDrawnPatch.BubbleWindowSnapshot> activeWindowSnapshotByPlayer = AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer;
			lock (activeWindowSnapshotByPlayer)
			{
				AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer.Clear();
			}
			AfterCardDrawnPatch._lastCombatCreatedMsec = num;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000032F4 File Offset: 0x000014F4
		public static void Postfix(CardModel card, PileType oldPile)
		{
			try
			{
				if (card != null)
				{
					Player owner = card.Owner;
					if (((owner != null) ? owner.Creature : null) != null)
					{
						bool flag = ModEntry.IsBubbleEnabledInCurrentRun();
						bool flag2 = ModEntry.IsOverlayEnabledInCurrentRun();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
						if (!flag && !flag2)
						{
							string text = "BubbleGate.Skip";
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 3);
							defaultInterpolatedStringHandler.AppendLiteral("reason=all_disabled, card=");
							defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
							defaultInterpolatedStringHandler.AppendLiteral(", oldPile=");
							defaultInterpolatedStringHandler.AppendFormatted<PileType>(oldPile);
							defaultInterpolatedStringHandler.AppendLiteral(", newPile=");
							CardPile pile = card.Pile;
							defaultInterpolatedStringHandler.AppendFormatted<PileType?>((pile != null) ? new PileType?(pile.Type) : null);
							ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
							return;
						}
						AfterCardDrawnPatch.EnsureCombatStateFresh();
						CardPile pile2 = card.Pile;
						bool flag3 = pile2 != null && pile2.Type == 2 && oldPile != 2;
						string text2 = "BubbleGate.Event";
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(72, 6);
						defaultInterpolatedStringHandler.AppendLiteral("card=");
						defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
						defaultInterpolatedStringHandler.AppendLiteral(", oldPile=");
						defaultInterpolatedStringHandler.AppendFormatted<PileType>(oldPile);
						defaultInterpolatedStringHandler.AppendLiteral(", newPile=");
						CardPile pile3 = card.Pile;
						defaultInterpolatedStringHandler.AppendFormatted<PileType?>((pile3 != null) ? new PileType?(pile3.Type) : null);
						defaultInterpolatedStringHandler.AppendLiteral(", enteredHand=");
						defaultInterpolatedStringHandler.AppendFormatted<bool>(flag3);
						defaultInterpolatedStringHandler.AppendLiteral(", bubbleEnabled=");
						defaultInterpolatedStringHandler.AppendFormatted<bool>(flag);
						defaultInterpolatedStringHandler.AppendLiteral(", overlayEnabled=");
						defaultInterpolatedStringHandler.AppendFormatted<bool>(flag2);
						ModEntry.LogUi(text2, defaultInterpolatedStringHandler.ToStringAndClear());
						if (!flag3)
						{
							ModEntry.LogUi("BubbleGate.Skip", "reason=not_entered_hand, card=" + card.Id.Entry);
							return;
						}
						if (flag)
						{
							AfterCardDrawnPatch.OnCardEnteredHand(card);
						}
						if (flag2)
						{
							ComboHintOverlay.EnsureAttached();
							ComboHintOverlay.Refresh();
						}
						return;
					}
				}
				ModEntry.LogUi("BubbleGate.Skip", "reason=card_or_owner_null");
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] failed in hand-entry hook: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				Log.Error(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
				ModEntry.LogErrorToFile("AfterCardDrawnPatch.Postfix", ex);
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003540 File Offset: 0x00001740
		private static void OnCardEnteredHand(CardModel card)
		{
			Creature creature = card.Owner.Creature;
			Player player = creature.Player;
			ulong num = ((player != null) ? player.NetId : 0UL);
			Dictionary<ulong, int> dictionary = AfterCardDrawnPatch.PendingDrawVersionByPlayer;
			bool flag2;
			lock (dictionary)
			{
				flag2 = AfterCardDrawnPatch.PendingDrawVersionByPlayer.ContainsKey(num);
			}
			string text = "BubbleGate.HandEntry";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 3);
			defaultInterpolatedStringHandler.AppendLiteral("card=");
			defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
			defaultInterpolatedStringHandler.AppendLiteral(", netId=");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(num);
			defaultInterpolatedStringHandler.AppendLiteral(", hasActiveWindow=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(flag2);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
			bool flag3 = AfterCardDrawnPatch.DoesCardMatchAnyTrigger(card);
			if (!flag2 && !flag3)
			{
				string text2 = "BubbleGate.Skip";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
				defaultInterpolatedStringHandler.AppendLiteral("reason=first_card_not_matched, card=");
				defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
				defaultInterpolatedStringHandler.AppendLiteral(", netId=");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(num);
				ModEntry.LogUi(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!flag2 && flag3)
			{
				string text3 = "BubbleGate.WindowStart";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
				defaultInterpolatedStringHandler.AppendLiteral("card=");
				defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
				defaultInterpolatedStringHandler.AppendLiteral(", netId=");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(num);
				ModEntry.LogUi(text3, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else if (flag2)
			{
				string text4 = "BubbleGate.WindowRefresh";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
				defaultInterpolatedStringHandler.AppendLiteral("card=");
				defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
				defaultInterpolatedStringHandler.AppendLiteral(", netId=");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(num);
				ModEntry.LogUi(text4, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			dictionary = AfterCardDrawnPatch.PendingDrawVersionByPlayer;
			int num3;
			lock (dictionary)
			{
				int num2;
				if (!AfterCardDrawnPatch.PendingDrawVersionByPlayer.TryGetValue(num, out num2))
				{
					num2 = 0;
				}
				num3 = num2 + 1;
				AfterCardDrawnPatch.PendingDrawVersionByPlayer[num] = num3;
			}
			if (!flag2 && flag3)
			{
				Player player2 = creature.Player;
				IEnumerable<CardModel> enumerable;
				if (player2 == null)
				{
					enumerable = null;
				}
				else
				{
					PlayerCombatState playerCombatState = player2.PlayerCombatState;
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
				string text5 = AfterCardDrawnPatch.BuildSignature(AfterCardDrawnPatch.FindMatchedTextsInHandExcludingCard(enumerable ?? Array.Empty<CardModel>(), card));
				Dictionary<ulong, AfterCardDrawnPatch.BubbleWindowSnapshot> activeWindowSnapshotByPlayer = AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer;
				lock (activeWindowSnapshotByPlayer)
				{
					AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer[num] = new AfterCardDrawnPatch.BubbleWindowSnapshot(num3, text5);
				}
				string text6 = "BubbleGate.WindowBaseline";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 3);
				defaultInterpolatedStringHandler.AppendLiteral("netId=");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(num);
				defaultInterpolatedStringHandler.AppendLiteral(", startVersion=");
				defaultInterpolatedStringHandler.AppendFormatted<int>(num3);
				defaultInterpolatedStringHandler.AppendLiteral(", baseline=");
				defaultInterpolatedStringHandler.AppendFormatted(text5);
				ModEntry.LogUi(text6, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			string text7 = "BubbleGate.WindowVersion";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
			defaultInterpolatedStringHandler.AppendLiteral("netId=");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(num);
			defaultInterpolatedStringHandler.AppendLiteral(", version=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(num3);
			ModEntry.LogUi(text7, defaultInterpolatedStringHandler.ToStringAndClear());
			AfterCardDrawnPatch.EmitSingleBubbleAfterWindow(creature, num, num3);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003894 File Offset: 0x00001A94
		private static bool DoesCardMatchAnyTrigger(CardModel card)
		{
			int count = AfterCardDrawnPatch.FindMatchedTriggersForCard(card).Count;
			string text = "BubbleGate.CardMatch";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
			defaultInterpolatedStringHandler.AppendLiteral("card=");
			defaultInterpolatedStringHandler.AppendFormatted(card.Id.Entry);
			defaultInterpolatedStringHandler.AppendLiteral(", matchCount=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(count);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
			return count > 0;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003900 File Offset: 0x00001B00
		private static async Task EmitSingleBubbleAfterWindow(Creature ownerCreature, ulong netId, int expectedVersion)
		{
			SceneTreeTimer sceneTreeTimer = ((SceneTree)Engine.GetMainLoop()).CreateTimer(0.3, true, false, false);
			await sceneTreeTimer.ToSignal(sceneTreeTimer, SceneTreeTimer.SignalName.Timeout);
			Dictionary<ulong, int> pendingDrawVersionByPlayer = AfterCardDrawnPatch.PendingDrawVersionByPlayer;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			lock (pendingDrawVersionByPlayer)
			{
				int num;
				if (!AfterCardDrawnPatch.PendingDrawVersionByPlayer.TryGetValue(netId, out num) || num != expectedVersion)
				{
					string text = "BubbleGate.WindowCancel";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
					defaultInterpolatedStringHandler.AppendLiteral("netId=");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
					defaultInterpolatedStringHandler.AppendLiteral(", expectedVersion=");
					defaultInterpolatedStringHandler.AppendFormatted<int>(expectedVersion);
					ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
				AfterCardDrawnPatch.PendingDrawVersionByPlayer.Remove(netId);
			}
			string text2 = "BubbleGate.WindowCommit";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
			defaultInterpolatedStringHandler.AppendLiteral("netId=");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
			defaultInterpolatedStringHandler.AppendLiteral(", version=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(expectedVersion);
			ModEntry.LogUi(text2, defaultInterpolatedStringHandler.ToStringAndClear());
			Player player = ownerCreature.Player;
			IReadOnlyList<CardModel> readOnlyList;
			if (player == null)
			{
				readOnlyList = null;
			}
			else
			{
				PlayerCombatState playerCombatState = player.PlayerCombatState;
				if (playerCombatState == null)
				{
					readOnlyList = null;
				}
				else
				{
					CardPile hand = playerCombatState.Hand;
					readOnlyList = ((hand != null) ? hand.Cards : null);
				}
			}
			IReadOnlyList<CardModel> readOnlyList2 = readOnlyList ?? Array.Empty<CardModel>();
			List<MatchedTrigger> list = AfterCardDrawnPatch.FindMatchedTextsInHand(readOnlyList2);
			string text3 = AfterCardDrawnPatch.BuildSignature(list);
			AfterCardDrawnPatch.BubbleWindowSnapshot? bubbleWindowSnapshot = null;
			Dictionary<ulong, AfterCardDrawnPatch.BubbleWindowSnapshot> activeWindowSnapshotByPlayer = AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer;
			lock (activeWindowSnapshotByPlayer)
			{
				AfterCardDrawnPatch.BubbleWindowSnapshot bubbleWindowSnapshot2;
				if (AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer.TryGetValue(netId, out bubbleWindowSnapshot2))
				{
					bubbleWindowSnapshot = new AfterCardDrawnPatch.BubbleWindowSnapshot?(bubbleWindowSnapshot2);
				}
				AfterCardDrawnPatch.ActiveWindowSnapshotByPlayer.Remove(netId);
			}
			string text4 = "BubbleGate.FinalScan";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 5);
			defaultInterpolatedStringHandler.AppendLiteral("netId=");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
			defaultInterpolatedStringHandler.AppendLiteral(", handCount=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(readOnlyList2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(", matchCount=");
			defaultInterpolatedStringHandler.AppendFormatted<int>(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(", signature=");
			defaultInterpolatedStringHandler.AppendFormatted(text3);
			defaultInterpolatedStringHandler.AppendLiteral(", baseline=");
			defaultInterpolatedStringHandler.AppendFormatted(((bubbleWindowSnapshot != null) ? bubbleWindowSnapshot.GetValueOrDefault().BaselineSignature : null) ?? "<none>");
			ModEntry.LogUi(text4, defaultInterpolatedStringHandler.ToStringAndClear());
			if (bubbleWindowSnapshot != null && text3 == bubbleWindowSnapshot.Value.BaselineSignature)
			{
				string text5 = "BubbleGate.Skip";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
				defaultInterpolatedStringHandler.AppendLiteral("reason=window_no_change, netId=");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
				defaultInterpolatedStringHandler.AppendLiteral(", startVersion=");
				defaultInterpolatedStringHandler.AppendFormatted<int>(bubbleWindowSnapshot.Value.WindowStartVersion);
				ModEntry.LogUi(text5, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else if (list.Count == 0)
			{
				string text6 = "BubbleGate.Skip";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
				defaultInterpolatedStringHandler.AppendLiteral("reason=no_matches_after_window, netId=");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
				ModEntry.LogUi(text6, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				string text7 = ModEntry.GetLocalizedBubblePrefix() + string.Join(ModEntry.GetLocalizedListSeparator(), list.Select((MatchedTrigger m) => AfterCardDrawnPatch.FormatColoredText(m.Text, m.ColorHex)));
				NSpeechBubbleVfx nspeechBubbleVfx = NSpeechBubbleVfx.Create(text7, ownerCreature, ModEntry.BubbleDurationSeconds, 5);
				if (nspeechBubbleVfx != null)
				{
					NCombatRoom instance = NCombatRoom.Instance;
					if (instance != null)
					{
						instance.CombatVfxContainer.AddChild(nspeechBubbleVfx, false, 0L);
					}
					string text8 = "BubbleGate.Emit";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
					defaultInterpolatedStringHandler.AppendLiteral("netId=");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
					defaultInterpolatedStringHandler.AppendLiteral(", text=");
					defaultInterpolatedStringHandler.AppendFormatted(text7);
					ModEntry.LogUi(text8, defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					string text9 = "BubbleGate.Skip";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
					defaultInterpolatedStringHandler.AppendLiteral("reason=bubble_create_failed, netId=");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(netId);
					ModEntry.LogUi(text9, defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003954 File Offset: 0x00001B54
		private static List<MatchedTrigger> FindMatchedTextsInHand(IEnumerable<CardModel> handCards)
		{
			List<MatchedTrigger> list = new List<MatchedTrigger>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			foreach (CardModel cardModel in handCards)
			{
				foreach (MatchedTrigger matchedTrigger in AfterCardDrawnPatch.FindMatchedTriggersForCard(cardModel))
				{
					if (hashSet.Add(AfterCardDrawnPatch.BuildMatchDedupKey(matchedTrigger)))
					{
						list.Add(matchedTrigger);
					}
				}
			}
			return list;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000039F8 File Offset: 0x00001BF8
		private static List<MatchedTrigger> FindMatchedTextsInHandExcludingCard(IEnumerable<CardModel> handCards, CardModel excludedCard)
		{
			List<MatchedTrigger> list = new List<MatchedTrigger>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			foreach (CardModel cardModel in handCards)
			{
				if (cardModel != excludedCard)
				{
					foreach (MatchedTrigger matchedTrigger in AfterCardDrawnPatch.FindMatchedTriggersForCard(cardModel))
					{
						if (hashSet.Add(AfterCardDrawnPatch.BuildMatchDedupKey(matchedTrigger)))
						{
							list.Add(matchedTrigger);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003AA4 File Offset: 0x00001CA4
		private static string BuildSignature(IEnumerable<MatchedTrigger> matches)
		{
			return string.Join("|", matches.Select((MatchedTrigger m) => m.ColorHex + ":" + m.Text));
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003AD8 File Offset: 0x00001CD8
		private static List<MatchedTrigger> FindMatchedTriggersForCard(CardModel card)
		{
			List<MatchedTrigger> list = new List<MatchedTrigger>();
			string entry = card.Id.Entry;
			if (string.IsNullOrWhiteSpace(entry))
			{
				return list;
			}
			string displayCardTitleWithEnglish = ModEntry.GetDisplayCardTitleWithEnglish(card);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
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

		// Token: 0x0600003F RID: 63 RVA: 0x00003BD8 File Offset: 0x00001DD8
		private static string BuildMatchDedupKey(MatchedTrigger match)
		{
			return match.ColorHex + "|" + match.Text;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003BF4 File Offset: 0x00001DF4
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

		// Token: 0x04000018 RID: 24
		private const double HandEntryDebounceSeconds = 0.3;

		// Token: 0x04000019 RID: 25
		private const string SpecialWeakenGroupKey = "specialweaken";

		// Token: 0x0400001A RID: 26
		private const string SpecialVulnerableGroupKey = "specialvulnerable";

		// Token: 0x0400001B RID: 27
		private static readonly Dictionary<ulong, int> PendingDrawVersionByPlayer = new Dictionary<ulong, int>();

		// Token: 0x0400001C RID: 28
		private static readonly Dictionary<ulong, AfterCardDrawnPatch.BubbleWindowSnapshot> ActiveWindowSnapshotByPlayer = new Dictionary<ulong, AfterCardDrawnPatch.BubbleWindowSnapshot>();

		// Token: 0x0400001D RID: 29
		private static ulong _lastCombatCreatedMsec;

		// Token: 0x02000017 RID: 23
		[Nullable(0)]
		private readonly struct BubbleWindowSnapshot : IEquatable<AfterCardDrawnPatch.BubbleWindowSnapshot>
		{
			// Token: 0x060000A5 RID: 165 RVA: 0x0000721D File Offset: 0x0000541D
			public BubbleWindowSnapshot(int WindowStartVersion, string BaselineSignature)
			{
				this.WindowStartVersion = WindowStartVersion;
				this.BaselineSignature = BaselineSignature;
			}

			// Token: 0x1700000E RID: 14
			// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000722D File Offset: 0x0000542D
			// (set) Token: 0x060000A7 RID: 167 RVA: 0x00007235 File Offset: 0x00005435
			public int WindowStartVersion { get; set; }

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000723E File Offset: 0x0000543E
			// (set) Token: 0x060000A9 RID: 169 RVA: 0x00007246 File Offset: 0x00005446
			public string BaselineSignature { get; set; }

			// Token: 0x060000AA RID: 170 RVA: 0x00007250 File Offset: 0x00005450
			[NullableContext(0)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("BubbleWindowSnapshot");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060000AB RID: 171 RVA: 0x0000729C File Offset: 0x0000549C
			[NullableContext(0)]
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("WindowStartVersion = ");
				builder.Append(this.WindowStartVersion.ToString());
				builder.Append(", BaselineSignature = ");
				builder.Append(this.BaselineSignature);
				return true;
			}

			// Token: 0x060000AC RID: 172 RVA: 0x000072EA File Offset: 0x000054EA
			[CompilerGenerated]
			public static bool operator !=(AfterCardDrawnPatch.BubbleWindowSnapshot left, AfterCardDrawnPatch.BubbleWindowSnapshot right)
			{
				return !(left == right);
			}

			// Token: 0x060000AD RID: 173 RVA: 0x000072F6 File Offset: 0x000054F6
			[CompilerGenerated]
			public static bool operator ==(AfterCardDrawnPatch.BubbleWindowSnapshot left, AfterCardDrawnPatch.BubbleWindowSnapshot right)
			{
				return left.Equals(right);
			}

			// Token: 0x060000AE RID: 174 RVA: 0x00007300 File Offset: 0x00005500
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return EqualityComparer<int>.Default.GetHashCode(this.<WindowStartVersion>k__BackingField) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<BaselineSignature>k__BackingField);
			}

			// Token: 0x060000AF RID: 175 RVA: 0x00007329 File Offset: 0x00005529
			[NullableContext(0)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is AfterCardDrawnPatch.BubbleWindowSnapshot && this.Equals((AfterCardDrawnPatch.BubbleWindowSnapshot)obj);
			}

			// Token: 0x060000B0 RID: 176 RVA: 0x00007341 File Offset: 0x00005541
			[CompilerGenerated]
			public bool Equals(AfterCardDrawnPatch.BubbleWindowSnapshot other)
			{
				return EqualityComparer<int>.Default.Equals(this.<WindowStartVersion>k__BackingField, other.<WindowStartVersion>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<BaselineSignature>k__BackingField, other.<BaselineSignature>k__BackingField);
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x00007373 File Offset: 0x00005573
			[CompilerGenerated]
			public void Deconstruct(out int WindowStartVersion, out string BaselineSignature)
			{
				WindowStartVersion = this.WindowStartVersion;
				BaselineSignature = this.BaselineSignature;
			}
		}
	}
}
