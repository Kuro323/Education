using System;
using System.Collections.Generic;

namespace education.Models;

public partial class Team
{
    public Guid TeamGuid { get; set; }

    public string TeamName { get; set; } = null!;

    public decimal Budget { get; set; }

    public int FoundedYear { get; set; }

    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
