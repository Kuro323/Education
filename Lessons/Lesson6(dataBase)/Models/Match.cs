using System;
using System.Collections.Generic;

namespace education.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public DateTime MatchDate { get; set; }

    public string ArenaName { get; set; } = null!;

    public Guid HomeTeamGuid { get; set; }

    public Guid AwayTeamGuid { get; set; }

    public virtual Team AwayTeam { get; set; } = null!;

    public virtual Team HomeTeam { get; set; } = null!;

    public virtual ICollection<PlayerMatchStat> PlayerMatchStats { get; set; } = new List<PlayerMatchStat>();
}
