using Microsoft.EntityFrameworkCore;

namespace DatabaseModel.Context
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        public DatabaseContext(string connectionString, DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        public virtual DbSet<Guild> Guilds { get; set; }
        public virtual DbSet<GuildUser> GuildUsers { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<SymbolsList> SymbolsLists { get; set; }
        public virtual DbSet<Symbol> Symbols { get; set; }
        public virtual DbSet<SymbolsListsToChannels> SymbolsListsToChannels { get; set; }
        public virtual DbSet<SymbolsListsToSymbols> SymbolsListsToSymbols { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guild>(guild =>
            {
                guild.HasKey(g => g.GuildId);
                guild.Property(g => g.GuildId)
                    .HasMaxLength(20)
                    .IsRequired(true);
                guild.Property(g => g.SpyMode)
                    .HasDefaultValue(Guild.DEFAULT_SPY_MODE)
                    .IsRequired(true);
                guild.Property(g => g.BaseRole)
                    .HasDefaultValue(Guild.DEFAULT_BASE_ROLE)
                    .HasMaxLength(20)
                    .IsRequired(true);
                guild.Property(g => g.ImmunityRole)
                    .HasDefaultValue(Guild.DEFAULT_IMMUNITY_ROLE)
                    .HasMaxLength(20)
                    .IsRequired(false);
                guild.HasMany(g => g.GuildUsers)
                    .WithOne(u => u.GuildNavigation);
                guild.HasMany(g => g.Channels)
                    .WithOne(c => c.GuildNavigation);
                guild.HasMany(g => g.SymbolsLists)
                    .WithMany(sl => sl.Guilds);
            });

            modelBuilder.Entity<GuildUser>(user =>
            {
                user.HasKey(u => new { u.UserId, u.GuildId });
                user.Property(u => u.UserId)
                    .HasMaxLength(20)
                    .IsRequired(true);
                user.Property(u => u.GuildId)
                    .HasMaxLength(20)
                    .IsRequired(true);
                user.Property(u => u.LastActiveAt)
                    .HasDefaultValueSql("now()");
                user.Property(u => u.Role)
                    .HasDefaultValue(GuildUser.DEFAULT_ROLE)
                    .IsRequired(true);
                user.Property(u => u.HasImmunity)
                    .HasDefaultValueSql("false")
                    .IsRequired(true);
                user.Property(u => u.Invited)
                    .HasMaxLength(128)
                    .HasDefaultValue("unknown");
                user.Property(u => u.Introduced)
                    .HasMaxLength(128)
                    .HasDefaultValue("unknown");
                user.HasOne(u => u.GuildNavigation)
                    .WithMany(g => g.GuildUsers)
                    .HasForeignKey(u => u.GuildId);
                user.Property(u => u.QMessageId)
                    .IsRequired(false);
                user.Property(u => u.Mailing)
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<Channel>(channel =>
            {
                channel.HasKey(c => c.ChannelId);
                channel.Property(c => c.ChannelId)
                    .HasMaxLength(20)
                    .IsRequired(true);
                channel.Property(c => c.GuildId)
                    .HasMaxLength(20)
                    .IsRequired(true);
                channel.Property(c => c.Moderation)
                    .HasDefaultValue(Channel.DEFAULT_MODERATION)
                    .IsRequired(true);
                channel.Property(c => c.Warning)
                    .HasColumnType("text")
                    .HasDefaultValue("Banword detected")
                    .IsRequired(true);
                channel.HasOne(c => c.GuildNavigation)
                    .WithMany(g => g.Channels)
                    .HasForeignKey(c => c.GuildId);

                channel.HasMany(c => c.SymbolsLists)
                    .WithMany(sl => sl.Channels)
                    .UsingEntity<SymbolsListsToChannels>(
                    j => j.HasOne(sltc => sltc.SymbolsListNavigation)
                        .WithMany(sl => sl.SymbolsListsToChannels)
                        .HasForeignKey(sltc => sltc.ListId),
                    j => j.HasOne(sltc => sltc.ChannelNavigation)
                        .WithMany(c => c.SymbolsListsToChannels)
                        .HasForeignKey(sltc => sltc.ChannelId));
            });

            modelBuilder.Entity<SymbolsListsToChannels>(symbolsListsToChannels =>
            {
                symbolsListsToChannels.Property(sltc => sltc.Moderation)
                    .HasDefaultValue(DatabaseModel.SymbolsListsToChannels.DEFAULT_MODERATION)
                    .IsRequired(true);
                symbolsListsToChannels.Property(sltc => sltc.ResendChannelId)
                    .HasMaxLength(20);
                symbolsListsToChannels.HasKey(sltc => new { sltc.ChannelId, sltc.ListId });
                symbolsListsToChannels.HasOne(sltc => sltc.ChannelNavigation)
                    .WithMany(c => c.SymbolsListsToChannels)
                    .HasForeignKey(sltc => sltc.ChannelId);
                symbolsListsToChannels.HasOne(sltc => sltc.SymbolsListNavigation)
                        .WithMany(sl => sl.SymbolsListsToChannels)
                        .HasForeignKey(sltc => sltc.ListId);
            });

            modelBuilder.HasSequence<long>("LId")
                .StartsAt(1000)
                .IncrementsBy(1);

            modelBuilder.Entity<SymbolsList>(symbolsList =>
            {
                symbolsList.HasKey(sl => sl.ListId);
                symbolsList.Property(sl => sl.ListId)
                    .HasDefaultValueSql("nextval('\"LId\"')")
                    .IsRequired(true);
                symbolsList.Property(sl => sl.Title)
                    .HasDefaultValue("Untitle")
                    .IsRequired(false);
                symbolsList.HasMany(sl => sl.Symbols)
                    .WithMany(s => s.SymbolsLists);
                symbolsList.HasMany(sl => sl.Channels)
                    .WithMany(c => c.SymbolsLists)
                    .UsingEntity<SymbolsListsToChannels>();
                symbolsList.HasMany(sl => sl.Guilds)
                    .WithMany(g => g.SymbolsLists);
            });

            modelBuilder.Entity<SymbolsListsToSymbols>(symbolsListToSymbols =>
            {
                symbolsListToSymbols.HasKey(slts => new { slts.SymbolId, slts.ListId });
                symbolsListToSymbols.Property(slts => slts.IsExcluded)
                    .HasDefaultValue(false)
                    .IsRequired(true);
                symbolsListToSymbols.HasOne(slts => slts.SymbolNavigation)
                    .WithMany(s => s.SymbolsListsToSymbols)
                    .HasForeignKey(slts => slts.SymbolId);
                symbolsListToSymbols.HasOne(slts => slts.SymbolsListNavigation)
                    .WithMany(sl => sl.SymbolsListsToSymbols)
                    .HasForeignKey(slts => slts.ListId);
            });

            modelBuilder.HasSequence<long>("SId")
                .StartsAt(1000)
                .IncrementsBy(1);

            modelBuilder.Entity<Symbol>(symbol =>
            {
                symbol.HasKey(s => s.SymbolId);
                symbol.Property(s => s.SymbolId)
                    .HasDefaultValueSql("nextval('\"SId\"')")
                    .IsRequired(true);
                symbol.Property(s => s.Content)
                    .HasMaxLength(64)
                    .IsRequired(true);
                symbol.HasMany(s => s.SymbolsLists)
                    .WithMany(sl => sl.Symbols)
                    .UsingEntity<SymbolsListsToSymbols>();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
