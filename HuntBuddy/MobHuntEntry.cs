namespace HuntBuddy;

public class MobHuntEntry {
	public string? Name { get; init; }

	public string? TerritoryName { get; init; }

	public string? ExpansionName { get; init; }

	public uint ExpansionId { get; init; }

	public uint MapId { get; init; }

	public uint TerritoryType { get; init; }

	public uint MobHuntId { get; init; }

	public int BillNumber { get; init; }

	public int MarkNumber { get; init; }

	public bool IsEliteMark { get; init; }

	public int NeededKills { get; set; }

	public uint Icon { get; init; }
}
