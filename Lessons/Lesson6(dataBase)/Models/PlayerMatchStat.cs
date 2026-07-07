using System;
using System.Collections.Generic;

namespace education.Models;

public partial class PlayerMatchStat
{
    public int PlayerId { get; set; }

    public int MatchId { get; set; }

    public int? PointsScored { get; set; }

    public int? FoulsCount { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
