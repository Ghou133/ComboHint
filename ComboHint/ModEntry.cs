using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace ComboHint
{
	// Token: 0x02000002 RID: 2
	[NullableContext(1)]
	[Nullable(0)]
	[ModInitializer("Initialize")]
	public static class ModEntry
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public static IReadOnlyList<TriggerGroup> TriggerGroups { get; private set; } = new List<TriggerGroup>();

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205F File Offset: 0x0000025F
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002066 File Offset: 0x00000266
		public static TriggerGroup OverlayKillTriggerGroup { get; private set; } = TriggerGroup.From("overlayKill", null, "#FF7F50");

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000206E File Offset: 0x0000026E
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002075 File Offset: 0x00000275
		public static string OverlayKillTitleColorHex { get; private set; } = "#FF3B30";

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000207D File Offset: 0x0000027D
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002084 File Offset: 0x00000284
		public static double BubbleDurationSeconds { get; private set; } = 1.8;

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000208C File Offset: 0x0000028C
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002093 File Offset: 0x00000293
		public static bool EnableSinglePlayerHint { get; private set; } = true;

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000209B File Offset: 0x0000029B
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020A2 File Offset: 0x000002A2
		public static bool EnableBubbleHint { get; private set; } = true;

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020AA File Offset: 0x000002AA
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020B1 File Offset: 0x000002B1
		public static bool EnabledByGameplaySetting { get; private set; } = true;

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020B9 File Offset: 0x000002B9
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020C0 File Offset: 0x000002C0
		public static string ModRootPath { get; private set; } = string.Empty;

		// Token: 0x06000011 RID: 17 RVA: 0x000020C8 File Offset: 0x000002C8
		public static void Initialize()
		{
			ModEntry.LoadConfig();
			ModEntry.LoadManifestSettings();
			ModEntry.ResetUiLog();
			ModEntry.ResetInjectLog();
			new Harmony("local.combo_hint").PatchAll();
			int num = ModEntry.TriggerGroups.Sum((TriggerGroup g) => g.TriggerModelIds.Count);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(88, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] initialized, trigger groups: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(ModEntry.TriggerGroups.Count);
			defaultInterpolatedStringHandler.AppendLiteral(", trigger model ids: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(num);
			defaultInterpolatedStringHandler.AppendLiteral(", enableSinglePlayerHint: ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(ModEntry.EnableSinglePlayerHint);
			Log.Info(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002184 File Offset: 0x00000384
		public static void ResetUiLog()
		{
			try
			{
				string text = Path.Combine(ModEntry.ResolveModRoot(), "combo_hint.ui.log");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "yyyy-MM-dd HH:mm:ss.fff");
				defaultInterpolatedStringHandler.AppendLiteral("] ComboHintUiLog.Reset: start new session");
				defaultInterpolatedStringHandler.AppendFormatted(Environment.NewLine);
				string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				File.WriteAllText(text, text2);
			}
			catch
			{
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002204 File Offset: 0x00000404
		public static void LogUi(string tag, string message)
		{
			try
			{
				string text = Path.Combine(ModEntry.ResolveModRoot(), "combo_hint.ui.log");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 4);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "yyyy-MM-dd HH:mm:ss.fff");
				defaultInterpolatedStringHandler.AppendLiteral("] ");
				defaultInterpolatedStringHandler.AppendFormatted(tag);
				defaultInterpolatedStringHandler.AppendLiteral(": ");
				defaultInterpolatedStringHandler.AppendFormatted(message);
				defaultInterpolatedStringHandler.AppendFormatted(Environment.NewLine);
				string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				File.AppendAllText(text, text2);
			}
			catch
			{
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000229C File Offset: 0x0000049C
		public static void ResetInjectLog()
		{
			try
			{
				string text = Path.Combine(Directory.GetCurrentDirectory(), "combo_hint.inject.log");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "yyyy-MM-dd HH:mm:ss.fff");
				defaultInterpolatedStringHandler.AppendLiteral("] ComboHintInjectLog.Reset: start new session");
				defaultInterpolatedStringHandler.AppendFormatted(Environment.NewLine);
				string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				File.WriteAllText(text, text2);
			}
			catch
			{
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000231C File Offset: 0x0000051C
		public static void LogInject(string tag, string message)
		{
			try
			{
				string text = Path.Combine(Directory.GetCurrentDirectory(), "combo_hint.inject.log");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 4);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "yyyy-MM-dd HH:mm:ss.fff");
				defaultInterpolatedStringHandler.AppendLiteral("] ");
				defaultInterpolatedStringHandler.AppendFormatted(tag);
				defaultInterpolatedStringHandler.AppendLiteral(": ");
				defaultInterpolatedStringHandler.AppendFormatted(message);
				defaultInterpolatedStringHandler.AppendFormatted(Environment.NewLine);
				string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				File.AppendAllText(text, text2);
			}
			catch
			{
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023B4 File Offset: 0x000005B4
		private static string ResolveModRoot()
		{
			string text = ModEntry.ModRootPath;
			if (string.IsNullOrWhiteSpace(text))
			{
				Mod mod = ModManager.AllMods.FirstOrDefault(delegate(Mod m)
				{
					ModManifest manifest = m.manifest;
					return ((manifest != null) ? manifest.id : null) == "ComboHint";
				});
				text = ((mod != null) ? mod.path : null) ?? Directory.GetCurrentDirectory();
			}
			return text;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000240F File Offset: 0x0000060F
		public static void LogErrorToFile(string context, Exception ex)
		{
			ModEntry.LogUi("LegacyError", context + ": " + ex.Message);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000242C File Offset: 0x0000062C
		public static void LogInfoToFile(string context, string message)
		{
			ModEntry.LogUi("LegacyInfo/" + context, message);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000243F File Offset: 0x0000063F
		[NullableContext(2)]
		public static bool IsChineseLanguage(string language)
		{
			return !string.IsNullOrWhiteSpace(language) && (language.StartsWith("zh", StringComparison.OrdinalIgnoreCase) || language.Equals("zhs", StringComparison.OrdinalIgnoreCase) || language.Equals("zht", StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002475 File Offset: 0x00000675
		public static bool IsChineseUiLanguage()
		{
			LocManager instance = LocManager.Instance;
			return ModEntry.IsChineseLanguage(((instance != null) ? instance.Language : null) ?? "eng");
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002496 File Offset: 0x00000696
		public static string GetLocalizedComboHintTitle()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return "Combo Hints";
			}
			return "连携提示";
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000024AA File Offset: 0x000006AA
		public static string GetLocalizedNoMatchText()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return "No combo effects available";
			}
			return "当前没有连携效果";
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000024BE File Offset: 0x000006BE
		public static string GetLocalizedKillTitle()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return "Lethal";
			}
			return "斩杀";
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000024D2 File Offset: 0x000006D2
		public static string GetLocalizedHasConnector()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return " has ";
			}
			return "有";
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000024E6 File Offset: 0x000006E6
		public static string GetLocalizedListSeparator()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return ", ";
			}
			return "、";
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000024FA File Offset: 0x000006FA
		public static string GetLocalizedBubblePrefix()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return "I have ";
			}
			return "我有";
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000250E File Offset: 0x0000070E
		public static string GetLocalizedWeakText()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return "Weak";
			}
			return "虚弱";
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002522 File Offset: 0x00000722
		public static string GetLocalizedVulnerableText()
		{
			if (!ModEntry.IsChineseUiLanguage())
			{
				return "Vulnerable";
			}
			return "易伤";
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002538 File Offset: 0x00000738
		public static string GetDisplayCardTitleWithEnglish(CardModel card)
		{
			string entry = card.Id.Entry;
			string text = card.Title ?? entry;
			LocManager instance = LocManager.Instance;
			if (ModEntry.IsChineseLanguage(((instance != null) ? instance.Language : null) ?? "eng"))
			{
				return text;
			}
			string text2 = ModEntry.TryGetEnglishCardTitle(entry);
			if (string.IsNullOrWhiteSpace(text2))
			{
				return text;
			}
			return text2;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002594 File Offset: 0x00000794
		[return: Nullable(2)]
		private static string TryGetEnglishCardTitle(string modelId)
		{
			ModEntry.EnsureEnglishCardTitlesLoaded();
			if (ModEntry._englishCardTitlesById == null)
			{
				return null;
			}
			string text;
			if (!ModEntry._englishCardTitlesById.TryGetValue(modelId, out text))
			{
				return null;
			}
			return text;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000025C4 File Offset: 0x000007C4
		private static void EnsureEnglishCardTitlesLoaded()
		{
			if (ModEntry._englishCardTitlesLoaded)
			{
				return;
			}
			object englishCardTitlesLock = ModEntry.EnglishCardTitlesLock;
			lock (englishCardTitlesLock)
			{
				if (!ModEntry._englishCardTitlesLoaded)
				{
					ModEntry._englishCardTitlesById = ModEntry.LoadEnglishCardTitlesById();
					ModEntry._englishCardTitlesLoaded = true;
					string text = "CardTitle.EnglishLoaded";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler.AppendLiteral("count=");
					defaultInterpolatedStringHandler.AppendFormatted<int>(ModEntry._englishCardTitlesById.Count);
					ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002654 File Offset: 0x00000854
		private unsafe static Dictionary<string, string> LoadEnglishCardTitlesById()
		{
			List<string> list = new List<string>();
			list.Add(Path.Combine(Directory.GetCurrentDirectory(), "localization", "eng", "cards.json"));
			<>y__InlineArray6<string> <>y__InlineArray = default(<>y__InlineArray6<string>);
			*<PrivateImplementationDetails>.InlineArrayElementRef<<>y__InlineArray6<string>, string>(ref <>y__InlineArray, 0) = ModEntry.ResolveModRoot();
			*<PrivateImplementationDetails>.InlineArrayElementRef<<>y__InlineArray6<string>, string>(ref <>y__InlineArray, 1) = "..";
			*<PrivateImplementationDetails>.InlineArrayElementRef<<>y__InlineArray6<string>, string>(ref <>y__InlineArray, 2) = "..";
			*<PrivateImplementationDetails>.InlineArrayElementRef<<>y__InlineArray6<string>, string>(ref <>y__InlineArray, 3) = "localization";
			*<PrivateImplementationDetails>.InlineArrayElementRef<<>y__InlineArray6<string>, string>(ref <>y__InlineArray, 4) = "eng";
			*<PrivateImplementationDetails>.InlineArrayElementRef<<>y__InlineArray6<string>, string>(ref <>y__InlineArray, 5) = "cards.json";
			list.Add(Path.Combine(<PrivateImplementationDetails>.InlineArrayAsReadOnlySpan<<>y__InlineArray6<string>, string>(in <>y__InlineArray, 6)));
			foreach (string text in list)
			{
				try
				{
					string fullPath = Path.GetFullPath(text);
					if (File.Exists(fullPath))
					{
						using (JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllText(fullPath), default(JsonDocumentOptions)))
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
							foreach (JsonProperty jsonProperty in jsonDocument.RootElement.EnumerateObject())
							{
								if (jsonProperty.Name.EndsWith(".title", StringComparison.Ordinal) && jsonProperty.Value.ValueKind == JsonValueKind.String)
								{
									string text2 = jsonProperty.Name.Substring(0, jsonProperty.Name.Length - ".title".Length);
									string @string = jsonProperty.Value.GetString();
									if (!string.IsNullOrWhiteSpace(text2) && !string.IsNullOrWhiteSpace(@string))
									{
										dictionary[text2] = Regex.Replace(@string.Trim(), "\\s+", " ");
									}
								}
							}
							if (dictionary.Count > 0)
							{
								return dictionary;
							}
						}
					}
				}
				catch (Exception ex)
				{
					ModEntry.LogUi("CardTitle.EnglishLoadError", ex.Message);
				}
			}
			return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000028D8 File Offset: 0x00000AD8
		private static void LoadManifestSettings()
		{
			ModEntry.EnableSinglePlayerHint = true;
			try
			{
				Mod mod = ModManager.AllMods.FirstOrDefault(delegate(Mod m)
				{
					ModManifest manifest = m.manifest;
					return ((manifest != null) ? manifest.id : null) == "ComboHint";
				});
				if (mod != null)
				{
					string text = Path.Combine(mod.path, "combo_hint.json");
					if (File.Exists(text))
					{
						try
						{
							using (JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllText(text), default(JsonDocumentOptions)))
							{
								JsonElement jsonElement;
								if (jsonDocument.RootElement.TryGetProperty("comboHintEnabled", out jsonElement))
								{
									if (jsonElement.ValueKind == JsonValueKind.True)
									{
										ModEntry.EnabledByGameplaySetting = true;
									}
									else if (jsonElement.ValueKind == JsonValueKind.False)
									{
										ModEntry.EnabledByGameplaySetting = false;
									}
								}
								JsonElement jsonElement2;
								if (jsonDocument.RootElement.TryGetProperty("enableSinglePlayerHint", out jsonElement2))
								{
									if (jsonElement2.ValueKind == JsonValueKind.True)
									{
										ModEntry.EnableSinglePlayerHint = true;
									}
									else if (jsonElement2.ValueKind == JsonValueKind.False)
									{
										ModEntry.EnableSinglePlayerHint = false;
									}
								}
								JsonElement jsonElement3;
								if (jsonDocument.RootElement.TryGetProperty("bubbleHintEnabled", out jsonElement3))
								{
									if (jsonElement3.ValueKind == JsonValueKind.True)
									{
										ModEntry.EnableBubbleHint = true;
									}
									else if (jsonElement3.ValueKind == JsonValueKind.False)
									{
										ModEntry.EnableBubbleHint = false;
									}
								}
							}
						}
						catch (Exception ex)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 1);
							defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] failed to parse manifest to read enableSinglePlayerHint: ");
							defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
							Log.Warn(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] error while checking single-player setting: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex2);
				Log.Warn(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002AB8 File Offset: 0x00000CB8
		public static bool IsOverlayEnabledInCurrentRun()
		{
			if (!ModEntry.EnabledByGameplaySetting)
			{
				return false;
			}
			if (!ModEntry.EnableSinglePlayerHint)
			{
				RunManager instance = RunManager.Instance;
				if (instance != null && instance.IsSinglePlayerOrFakeMultiplayer)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002AE0 File Offset: 0x00000CE0
		public static bool IsBubbleEnabledInCurrentRun()
		{
			if (!ModEntry.EnableBubbleHint)
			{
				return false;
			}
			if (!ModEntry.EnableSinglePlayerHint)
			{
				RunManager instance = RunManager.Instance;
				if (instance != null && instance.IsSinglePlayerOrFakeMultiplayer)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002B08 File Offset: 0x00000D08
		public static void SetEnabledByGameplaySetting(bool enabled)
		{
			if (ModEntry.EnabledByGameplaySetting == enabled)
			{
				return;
			}
			ModEntry.EnabledByGameplaySetting = enabled;
			ModEntry.SaveManifestBool("comboHintEnabled", ModEntry.EnabledByGameplaySetting);
			string text = "GameplaySetting.ComboHint";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("enabled=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(enabled);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002B64 File Offset: 0x00000D64
		public static void SetEnableSinglePlayerHintByGameplaySetting(bool enabled)
		{
			if (ModEntry.EnableSinglePlayerHint == enabled)
			{
				return;
			}
			ModEntry.EnableSinglePlayerHint = enabled;
			ModEntry.SaveManifestBool("enableSinglePlayerHint", ModEntry.EnableSinglePlayerHint);
			string text = "GameplaySetting.SinglePlayerComboHint";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("enabled=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(enabled);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002BC0 File Offset: 0x00000DC0
		public static void SetBubbleHintByGameplaySetting(bool enabled)
		{
			if (ModEntry.EnableBubbleHint == enabled)
			{
				return;
			}
			ModEntry.EnableBubbleHint = enabled;
			ModEntry.SaveManifestBool("bubbleHintEnabled", ModEntry.EnableBubbleHint);
			string text = "GameplaySetting.BubbleHint";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("enabled=");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(enabled);
			ModEntry.LogUi(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002C1C File Offset: 0x00000E1C
		private static void SaveManifestBool(string key, bool value)
		{
			try
			{
				string text = Path.Combine(ModEntry.ResolveModRoot(), "combo_hint.json");
				JsonObject jsonObject2;
				if (File.Exists(text))
				{
					JsonObject jsonObject;
					if ((jsonObject = JsonNode.Parse(File.ReadAllText(text), null, default(JsonDocumentOptions)) as JsonObject) == null)
					{
						jsonObject = new JsonObject(null);
					}
					jsonObject2 = jsonObject;
				}
				else
				{
					jsonObject2 = new JsonObject(null);
				}
				jsonObject2[key] = value;
				string text2 = jsonObject2.ToJsonString(ModEntry.PrettyReadableJsonOptions);
				File.WriteAllText(text, text2);
			}
			catch (Exception ex)
			{
				Log.Warn("[ComboHint] failed to write combo_hint.json: " + ex.Message, 2);
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002CD8 File Offset: 0x00000ED8
		private static void LoadConfig()
		{
			try
			{
				Mod mod = ModManager.AllMods.FirstOrDefault(delegate(Mod m)
				{
					ModManifest manifest = m.manifest;
					return ((manifest != null) ? manifest.id : null) == "ComboHint";
				});
				if (mod == null)
				{
					Log.Warn("[ComboHint] could not find self in ModManager.AllMods, trigger groups will be empty.", 2);
					ModEntry.TriggerGroups = new List<TriggerGroup>();
				}
				else
				{
					ModEntry.ModRootPath = mod.path;
					string text = Path.Combine(mod.path, "combo_hint.config.json");
					if (!File.Exists(text))
					{
						Log.Warn("[ComboHint] config file not found at " + text + ", trigger groups will be empty.", 2);
						ModEntry.TriggerGroups = new List<TriggerGroup>();
						ModEntry.BubbleDurationSeconds = 1.8;
					}
					else
					{
						using (JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllText(text), default(JsonDocumentOptions)))
						{
							ModEntry.TriggerGroups = ModEntry.BuildTriggerGroups(jsonDocument.RootElement);
							ModEntry.OverlayKillTriggerGroup = ModEntry.ReadOverlayKillTriggerGroup(jsonDocument.RootElement);
							ModEntry.OverlayKillTitleColorHex = ModEntry.ReadOverlayKillTitleColor(jsonDocument.RootElement);
							ModEntry.BubbleDurationSeconds = ModEntry.ReadBubbleDurationSeconds(jsonDocument.RootElement);
							ModEntry.RewriteConfigWithReadableChinese(text, jsonDocument.RootElement);
							if (ModEntry.TriggerGroups.Count == 0)
							{
								Log.Warn("[ComboHint] no valid trigger groups in config, trigger groups will be empty.", 2);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[ComboHint] failed to load config: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				Log.Error(defaultInterpolatedStringHandler.ToStringAndClear(), 2);
				ModEntry.LogErrorToFile("LoadConfig", ex);
				ModEntry.TriggerGroups = new List<TriggerGroup>();
				ModEntry.EnabledByGameplaySetting = true;
				ModEntry.OverlayKillTriggerGroup = TriggerGroup.From("overlayKill", null, "#FF7F50");
				ModEntry.OverlayKillTitleColorHex = "#FF3B30";
				ModEntry.BubbleDurationSeconds = 1.8;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002EB8 File Offset: 0x000010B8
		private static void RewriteConfigWithReadableChinese(string configPath, JsonElement root)
		{
			try
			{
				JsonObject jsonObject = JsonNode.Parse(root.GetRawText(), null, default(JsonDocumentOptions)) as JsonObject;
				if (jsonObject != null)
				{
					jsonObject.Remove("comboHintEnabled");
					jsonObject.Remove("bubbleHintEnabled");
					string text = jsonObject.ToJsonString(ModEntry.PrettyReadableJsonOptions);
					File.WriteAllText(configPath, text);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002F2C File Offset: 0x0000112C
		private static TriggerGroup ReadOverlayKillTriggerGroup(JsonElement root)
		{
			List<string> list = null;
			JsonElement jsonElement;
			JsonElement jsonElement2;
			if (root.TryGetProperty("overlayKillTriggerModelIds", out jsonElement))
			{
				list = ModEntry.ReadStringArray(jsonElement);
			}
			else if (root.TryGetProperty("overlayKillTriggerTexts", out jsonElement2))
			{
				list = ModEntry.ReadStringArray(jsonElement2);
			}
			string text = null;
			JsonElement jsonElement3;
			if (root.TryGetProperty("overlayKillColor", out jsonElement3) && jsonElement3.ValueKind == JsonValueKind.String)
			{
				text = jsonElement3.GetString();
			}
			return TriggerGroup.From("overlayKill", list, text);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002F9C File Offset: 0x0000119C
		private static string ReadOverlayKillTitleColor(JsonElement root)
		{
			JsonElement jsonElement;
			if (root.TryGetProperty("overlayKillTitleColor", out jsonElement) && jsonElement.ValueKind == JsonValueKind.String)
			{
				string @string = jsonElement.GetString();
				if (!string.IsNullOrWhiteSpace(@string))
				{
					string text = @string.Trim();
					if (!text.StartsWith("#", StringComparison.Ordinal))
					{
						return "#" + text;
					}
					return text;
				}
			}
			return "#FF3B30";
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002FFC File Offset: 0x000011FC
		private static List<TriggerGroup> BuildTriggerGroups(JsonElement root)
		{
			List<TriggerGroup> list = new List<TriggerGroup>();
			foreach (JsonProperty jsonProperty in root.EnumerateObject())
			{
				if (jsonProperty.Name.StartsWith("triggerModelIds_", StringComparison.Ordinal))
				{
					List<string> list2 = ModEntry.ReadStringArray(jsonProperty.Value);
					string text = jsonProperty.Name.Substring("triggerModelIds_".Length);
					string text2 = "color_" + text;
					string text3 = null;
					JsonElement jsonElement;
					if (root.TryGetProperty(text2, out jsonElement) && jsonElement.ValueKind == JsonValueKind.String)
					{
						text3 = jsonElement.GetString();
					}
					TriggerGroup triggerGroup = TriggerGroup.From(text, list2, text3);
					if (triggerGroup.TriggerModelIds.Count > 0)
					{
						list.Add(triggerGroup);
					}
				}
			}
			return list;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000030E8 File Offset: 0x000012E8
		[return: Nullable(new byte[] { 2, 1 })]
		private static List<string> ReadStringArray(JsonElement element)
		{
			if (element.ValueKind != JsonValueKind.Array)
			{
				return null;
			}
			List<string> list = new List<string>();
			foreach (JsonElement jsonElement in element.EnumerateArray())
			{
				if (jsonElement.ValueKind == JsonValueKind.String)
				{
					string @string = jsonElement.GetString();
					if (!string.IsNullOrWhiteSpace(@string))
					{
						list.Add(@string);
					}
				}
			}
			return list;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003170 File Offset: 0x00001370
		private static double ReadBubbleDurationSeconds(JsonElement root)
		{
			JsonElement jsonElement;
			if (!root.TryGetProperty("bubbleDurationSeconds", out jsonElement))
			{
				return 1.8;
			}
			double num;
			if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetDouble(out num))
			{
				return Math.Clamp(num, 0.3, 10.0);
			}
			return 1.8;
		}

		// Token: 0x04000001 RID: 1
		private const string HarmonyId = "local.combo_hint";

		// Token: 0x04000002 RID: 2
		private const string ModId = "ComboHint";

		// Token: 0x04000003 RID: 3
		private const string ConfigFileName = "combo_hint.config.json";

		// Token: 0x04000004 RID: 4
		private const string ManifestFileName = "combo_hint.json";

		// Token: 0x04000005 RID: 5
		private const string UiLogFileName = "combo_hint.ui.log";

		// Token: 0x04000006 RID: 6
		private const string InjectLogFileName = "combo_hint.inject.log";

		// Token: 0x04000007 RID: 7
		private const string ConfigEnabledKey = "comboHintEnabled";

		// Token: 0x04000008 RID: 8
		private const string EnableSinglePlayerHintKey = "enableSinglePlayerHint";

		// Token: 0x04000009 RID: 9
		private const string BubbleEnabledKey = "bubbleHintEnabled";

		// Token: 0x0400000A RID: 10
		private static readonly JsonSerializerOptions PrettyReadableJsonOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};

		// Token: 0x0400000B RID: 11
		private const double DefaultBubbleDurationSeconds = 1.8;

		// Token: 0x0400000C RID: 12
		private const string DefaultOverlayKillTitleColorHex = "#FF3B30";

		// Token: 0x04000015 RID: 21
		private static readonly object EnglishCardTitlesLock = new object();

		// Token: 0x04000016 RID: 22
		[Nullable(new byte[] { 2, 1, 1 })]
		private static Dictionary<string, string> _englishCardTitlesById;

		// Token: 0x04000017 RID: 23
		private static bool _englishCardTitlesLoaded;
	}
}
