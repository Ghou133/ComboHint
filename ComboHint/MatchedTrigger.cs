using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ComboHint
{
	// Token: 0x0200000B RID: 11
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct MatchedTrigger : IEquatable<MatchedTrigger>
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00003F41 File Offset: 0x00002141
		public MatchedTrigger(string Text, string ColorHex)
		{
			this.Text = Text;
			this.ColorHex = ColorHex;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00003F51 File Offset: 0x00002151
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00003F59 File Offset: 0x00002159
		public string Text { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00003F62 File Offset: 0x00002162
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00003F6A File Offset: 0x0000216A
		public string ColorHex { get; set; }

		// Token: 0x06000053 RID: 83 RVA: 0x00003F74 File Offset: 0x00002174
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MatchedTrigger");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003FC0 File Offset: 0x000021C0
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Text = ");
			builder.Append(this.Text);
			builder.Append(", ColorHex = ");
			builder.Append(this.ColorHex);
			return true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003FF5 File Offset: 0x000021F5
		[CompilerGenerated]
		public static bool operator !=(MatchedTrigger left, MatchedTrigger right)
		{
			return !(left == right);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004001 File Offset: 0x00002201
		[CompilerGenerated]
		public static bool operator ==(MatchedTrigger left, MatchedTrigger right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000400B File Offset: 0x0000220B
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(this.<Text>k__BackingField) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<ColorHex>k__BackingField);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004034 File Offset: 0x00002234
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MatchedTrigger && this.Equals((MatchedTrigger)obj);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000404C File Offset: 0x0000224C
		[CompilerGenerated]
		public bool Equals(MatchedTrigger other)
		{
			return EqualityComparer<string>.Default.Equals(this.<Text>k__BackingField, other.<Text>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<ColorHex>k__BackingField, other.<ColorHex>k__BackingField);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000407E File Offset: 0x0000227E
		[CompilerGenerated]
		public void Deconstruct(out string Text, out string ColorHex)
		{
			Text = this.Text;
			ColorHex = this.ColorHex;
		}
	}
}
