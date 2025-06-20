using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TelegramBot.Data.EF;

public partial class TelegramBotContext : DbContext
{
    public TelegramBotContext()
    {
    }

    public TelegramBotContext(DbContextOptions<TelegramBotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clima> Climas { get; set; }

    public virtual DbSet<Consulta> Consultas { get; set; }

    public virtual DbSet<LogsErrore> LogsErrores { get; set; }

    public virtual DbSet<Respuesta> Respuestas { get; set; }

    public virtual DbSet<TemasUniversidad> TemasUniversidads { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=TelegramBot;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clima>(entity =>
        {
            entity.HasKey(e => e.ClimaId).HasName("PK__Clima__2E0FF35BC263783B");

            entity.ToTable("Clima");

            entity.Property(e => e.ClimaId).HasColumnName("ClimaID");
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.FechaConsulta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TemperaturaActual).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.ConsultaId).HasName("PK__Consulta__7D0B7DACC8834E61");

            entity.Property(e => e.ConsultaId).HasColumnName("ConsultaID");
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FuenteRespuesta).HasMaxLength(50);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Consultas__Usuar__44FF419A");
        });

        modelBuilder.Entity<LogsErrore>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__LogsErro__5E5499A8962B26AD");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Fuente).HasMaxLength(50);
            entity.Property(e => e.TipoError).HasMaxLength(100);
        });

        modelBuilder.Entity<Respuesta>(entity =>
        {
            entity.HasKey(e => e.RespuestaId).HasName("PK__Respuest__31F7FC3152E9263A");

            entity.Property(e => e.RespuestaId).HasColumnName("RespuestaID");
            entity.Property(e => e.ConsultaId).HasColumnName("ConsultaID");
            entity.Property(e => e.GeneradaPor).HasMaxLength(50);

            entity.HasOne(d => d.Consulta).WithMany(p => p.RespuestaNavigation)
                .HasForeignKey(d => d.ConsultaId)
                .HasConstraintName("FK__Respuesta__Consu__45F365D3");
        });

        modelBuilder.Entity<TemasUniversidad>(entity =>
        {
            entity.HasKey(e => e.TemaId).HasName("PK__TemasUni__BF02E6D605DDF266");

            entity.ToTable("TemasUniversidad");

            entity.Property(e => e.TemaId).HasColumnName("TemaID");
            entity.Property(e => e.Categoria).HasMaxLength(100);
            entity.Property(e => e.Titulo).HasMaxLength(200);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798CADE4E7F");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.TelegramId).HasColumnName("TelegramID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
