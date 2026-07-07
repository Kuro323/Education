using System;
using System.Collections.Generic;

namespace education.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public bool? IsActive { get; set; }

    public Guid TeamGuid { get; set; }

    public int PositionId { get; set; }

    public virtual ICollection<PlayerMatchStat> PlayerMatchStats { get; set; } = new List<PlayerMatchStat>();

    public virtual Position Position { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
