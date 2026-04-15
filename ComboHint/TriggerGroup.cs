using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ComboHint
{
	// Token: 0x0200000A RID: 10
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TriggerGroup
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00003E3C File Offset: 0x0000203C
		public string Key { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00003E44 File Offset: 0x00002044
		public IReadOnlySet<string> TriggerModelIds { get; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00003E4C File Offset: 0x0000204C
		public string ColorHex { get; }

		// Token: 0x0600004B RID: 75 RVA: 0x00003E54 File Offset: 0x00002054
		private TriggerGroup(string key, IReadOnlySet<string> triggerModelIds, string colorHex)
		{
			this.Key = key;
			this.TriggerModelIds = triggerModelIds;
			this.ColorHex = colorHex;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003E74 File Offset: 0x00002074
		public static TriggerGroup From(string key, [Nullable(new byte[] { 2, 1 })] List<string> rawModelIds, [Nullable(2)] string rawColor)
		{
			HashSet<string> hashSet;
			if (rawModelIds == null)
			{
				hashSet = null;
			}
			else
			{
				hashSet = (from s in rawModelIds
					where !string.IsNullOrWhiteSpace(s)
					select s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToHashSet(StringComparer.OrdinalIgnoreCase);
			}
			HashSet<string> hashSet2 = hashSet ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string text = TriggerGroup.NormalizeColor(rawColor);
			return new TriggerGroup(key, hashSet2, text);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003F04 File Offset: 0x00002104
		private static string NormalizeColor([Nullable(2)] string rawColor)
		{
			if (string.IsNullOrWhiteSpace(rawColor))
			{
				return "#FFFFFF";
			}
			string text = rawColor.Trim();
			if (text.StartsWith("#", StringComparison.Ordinal))
			{
				return text;
			}
			return "#" + text;
		}
	}
}
