using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace education.Models;

public partial class BasketballDbContext : DbContext
{
    public BasketballDbContext()
    {
    }

    public BasketballDbContext(DbContextOptions<BasketballDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerMatchStat> PlayerMatchStats { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(ConfigurationHelper.GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("PK__Matches__4218C837E244054E");

            entity.Property(e => e.MatchId).HasColumnName("MatchID");
            entity.Property(e => e.ArenaName).HasMaxLength(150);

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeams)
                .HasForeignKey(d => d.AwayTeamGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_AwayTeam");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeams)
                .HasForeignKey(d => d.HomeTeamGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Matches_HomeTeam");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Players__4A4E74A8CCCF03A5");

            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Position).WithMany(p => p.Players)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Players_Position");

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .HasForeignKey(d => d.TeamGuid)
                .HasConstraintName("FK_Players_Teams");
        });

        modelBuilder.Entity<PlayerMatchStat>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.MatchId }).HasName("PK__PlayerMa__2E6FF82BFBA3E2DC");

            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.MatchId).HasColumnName("MatchID");
            entity.Property(e => e.FoulsCount).HasDefaultValue(0);
            entity.Property(e => e.PointsScored).HasDefaultValue(0);

            entity.HasOne(d => d.Match).WithMany(p => p.PlayerMatchStats)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK_Stats_Matchs");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerMatchStats)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK_Stats_Players");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59D5E9E8EB");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(50);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamGuid).HasName("PK__Teams__859092F16C4E0B5B");

            entity.HasIndex(e => e.TeamName, "UIX_Teams_TeamName").IsUnique();

            entity.Property(e => e.TeamGuid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Budget).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TeamName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
