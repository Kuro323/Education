using System;
using System.Collections.Generic;

namespace education.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
