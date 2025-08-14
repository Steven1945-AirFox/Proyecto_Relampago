using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionBecas.Models;

public partial class ProyectoRelampagoContext : DbContext
{
    public ProyectoRelampagoContext()
    {
    }

    public ProyectoRelampagoContext(DbContextOptions<ProyectoRelampagoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AposentosVivienda> AposentosVivienda { get; set; }

    public virtual DbSet<AposentosViviendaEstudiante> AposentosViviendaEstudiante { get; set; }

    public virtual DbSet<BienesInmueblesEstudiante> BienesInmueblesEstudiante { get; set; }

    public virtual DbSet<BienesMueblesEstudiante> BienesMueblesEstudiante { get; set; }

    public virtual DbSet<Convocatorias> Convocatorias { get; set; }

    public virtual DbSet<DatosAcademicos> DatosAcademicos { get; set; }

    public virtual DbSet<Documentos> Documentos { get; set; }

    public virtual DbSet<DocumentosEstudiante> DocumentosEstudiante { get; set; }

    public virtual DbSet<EstudianteUsuario> EstudianteUsuario { get; set; }

    public virtual DbSet<FuenteIngresos> FuenteIngresos { get; set; }

    public virtual DbSet<FuenteIngresosEstudiante> FuenteIngresosEstudiante { get; set; }

    public virtual DbSet<FuenteIngresosNucleoFamiliarEstudiante> FuenteIngresosNucleoFamiliarEstudiante { get; set; }

    public virtual DbSet<IngresosEstudiante> IngresosEstudiante { get; set; }

    public virtual DbSet<PeriodoRecepcion> PeriodoRecepcion { get; set; }

    public virtual DbSet<Recibos> Recibos { get; set; }

    public virtual DbSet<RecibosEstudiante> RecibosEstudiante { get; set; }

    public virtual DbSet<Resolucion> Resolucion { get; set; }

    public virtual DbSet<TipoViviendaEstudiante> TipoViviendaEstudiante { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Estudiante");

        modelBuilder.Entity<AposentosVivienda>(entity =>
        {
            entity.HasKey(e => e.IdAposento).HasName("PK__Aposento__41EF855E93C5B272");

            entity.Property(e => e.IdAposento).HasColumnName("idAposento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AposentosViviendaEstudiante>(entity =>
        {
            entity.HasKey(e => new { e.CarnetEstudiante, e.IdAposento }).HasName("PK__Aposento__9CE8E4E6DB9353E1");

            entity.ToTable("AposentosVivienda_Estudiante");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.AposentosViviendaEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Aposentos__carne__5AEE82B9");

            entity.HasOne(d => d.IdAposentoNavigation).WithMany(p => p.AposentosViviendaEstudiante)
                .HasForeignKey(d => d.IdAposento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Aposentos__IdApo__5BE2A6F2");
        });

        modelBuilder.Entity<BienesInmueblesEstudiante>(entity =>
        {
            entity.HasKey(e => e.CarnetEstudiante).HasName("PK__BienesIn__087FE8840D2DA036");

            entity.ToTable("BienesInmuebles_Estudiante");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithOne(p => p.BienesInmueblesEstudiante)
                .HasForeignKey<BienesInmueblesEstudiante>(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienesInm__carne__46E78A0C");
        });

        modelBuilder.Entity<BienesMueblesEstudiante>(entity =>
        {
            entity.HasKey(e => new { e.CarnetEstudiante, e.NumeroPlaca }).HasName("PK__BienesMu__AD39C24185A499C2");

            entity.ToTable("BienesMuebles_Estudiante");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.NumeroPlaca)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombrePropietario)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.BienesMueblesEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienesMue__carne__49C3F6B7");
        });

        modelBuilder.Entity<Convocatorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Convocat__3214EC07DE9E14F7");

            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasDefaultValue("Activa");
            entity.Property(e => e.FechaCierre).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(255);
            entity.Property(e => e.TipoBeca).HasMaxLength(100);

            entity.HasMany(d => d.Documento).WithMany(p => p.Convocatoria)
                .UsingEntity<Dictionary<string, object>>(
                    "ConvocatoriaDocumentos",
                    r => r.HasOne<Documentos>().WithMany()
                        .HasForeignKey("DocumentoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Convocato__Docum__06CD04F7"),
                    l => l.HasOne<Convocatorias>().WithMany()
                        .HasForeignKey("ConvocatoriaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Convocato__Convo__05D8E0BE"),
                    j =>
                    {
                        j.HasKey("ConvocatoriaId", "DocumentoId").HasName("PK__Convocat__400DEEBA1DC0F63D");
                    });
        });

        modelBuilder.Entity<DatosAcademicos>(entity =>
        {
            entity.HasKey(e => e.CarnetEstudiante).HasName("PK__Datos_ac__087FE884A66E0935");

            entity.ToTable("Datos_academicos");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.NombreColegio)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreInstitucionBecaria)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TitulosUniversitariosParauniversitariosAnteriores).HasColumnName("TitulosUniversitarios_ParauniversitariosAnteriores");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithOne(p => p.DatosAcademicos)
                .HasForeignKey<DatosAcademicos>(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Datos_aca__carne__3F466844");
        });

        modelBuilder.Entity<Documentos>(entity =>
        {
            entity.HasKey(e => e.IdDocumento).HasName("PK__Document__E5207347D13DEF7D");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DocumentosEstudiante>(entity =>
        {
            entity.HasKey(e => e.IdRegistroDocumento).HasName("PK__Document__FEBC87FA263D1684");

            entity.ToTable("Documentos_Estudiante");

            entity.Property(e => e.IdRegistroDocumento).HasColumnName("idRegistroDocumento");
            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.IdDocumento).HasColumnName("idDocumento");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.DocumentosEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Documento__Estad__656C112C");

            entity.HasOne(d => d.IdDocumentoNavigation).WithMany(p => p.DocumentosEstudiante)
                .HasForeignKey(d => d.IdDocumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Documento__idDoc__66603565");
        });

        modelBuilder.Entity<EstudianteUsuario>(entity =>
        {
            entity.HasKey(e => e.Carnet).HasName("PK__Estudian__4CDEAA6FD681BF44");

            entity.ToTable("Estudiante_Usuario");

            entity.Property(e => e.Carnet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnet");
            entity.Property(e => e.Canton)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Carrera)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Distrito)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EstadoCivil)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Nacionalidad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NivelCarrera)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Provincia)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.EstudianteUsuario)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Estudiant__idUsu__3C69FB99");
        });

        modelBuilder.Entity<FuenteIngresos>(entity =>
        {
            entity.HasKey(e => e.IdFuente).HasName("PK__FuenteIn__4A469AD9013D36E5");

            entity.Property(e => e.IdFuente)
                .ValueGeneratedNever()
                .HasColumnName("idFuente");
            entity.Property(e => e.NombreFuente)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FuenteIngresosEstudiante>(entity =>
        {
            entity.HasKey(e => new { e.CarnetEstudiante, e.IdFuente }).HasName("PK__FuenteIn__8CDB8129ED63162A");

            entity.ToTable("FuenteIngresos_Estudiante");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.IdFuente).HasColumnName("idFuente");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.FuenteIngresosEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FuenteIng__carne__52593CB8");

            entity.HasOne(d => d.IdFuenteNavigation).WithMany(p => p.FuenteIngresosEstudiante)
                .HasForeignKey(d => d.IdFuente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FuenteIng__idFue__534D60F1");
        });

        modelBuilder.Entity<FuenteIngresosNucleoFamiliarEstudiante>(entity =>
        {
            entity.HasKey(e => new { e.CarnetEstudiante, e.IdFuente }).HasName("PK__FuenteIn__8CDB8129CD3762A4");

            entity.ToTable("FuenteIngresosNucleoFamiliar_Estudiante");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.IdFuente).HasColumnName("idFuente");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.FuenteIngresosNucleoFamiliarEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FuenteIng__carne__4222D4EF");

            entity.HasOne(d => d.IdFuenteNavigation).WithMany(p => p.FuenteIngresosNucleoFamiliarEstudiante)
                .HasForeignKey(d => d.IdFuente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FuenteIng__idFue__4316F928");
        });

        modelBuilder.Entity<IngresosEstudiante>(entity =>
        {
            entity.HasKey(e => e.IdIngreso).HasName("PK__Ingresos__5E6E52C40B2CFD23");

            entity.ToTable("Ingresos_Estudiante");

            entity.Property(e => e.IdIngreso).HasColumnName("idIngreso");
            entity.Property(e => e.Apellido1Pariente)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2Pariente)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.CedulaPariente)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CondicionSalud)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.InstitucionDondeLaboraPensionBeca)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("InstitucionDondeLabora_Pension_Beca");
            entity.Property(e => e.NombrePariente)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ocupacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ParentezcoConEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.IngresosEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ingresos___Estad__5EBF139D");
        });

        modelBuilder.Entity<PeriodoRecepcion>(entity =>
        {
            entity.HasKey(e => e.Idperiodo).HasName("PK__PeriodoR__EDCB2C0AD1C75CF6");

            entity.Property(e => e.Idperiodo)
                .ValueGeneratedNever()
                .HasColumnName("idperiodo");
            entity.Property(e => e.FechaHoraFin).HasColumnType("datetime");
            entity.Property(e => e.FechaHoraInicio).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Recibos>(entity =>
        {
            entity.HasKey(e => e.IdRecibo).HasName("PK__Recibos__2FEC47315DFFA9F1");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RecibosEstudiante>(entity =>
        {
            entity.HasKey(e => e.IdRegistro).HasName("PK__Recibos___62FC8F585CAC47FC");

            entity.ToTable("Recibos_Estudiante");

            entity.Property(e => e.IdRegistro).HasColumnName("idRegistro");
            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.IdRecibo).HasColumnName("idRecibo");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.RecibosEstudiante)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recibos_E__Estad__619B8048");

            entity.HasOne(d => d.IdReciboNavigation).WithMany(p => p.RecibosEstudiante)
                .HasForeignKey(d => d.IdRecibo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recibos_E__idRec__628FA481");
        });

        modelBuilder.Entity<Resolucion>(entity =>
        {
            entity.HasKey(e => e.IdResolucion).HasName("PK__Resoluci__18BB5C9F46E2D91E");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");
            entity.Property(e => e.FechaResolucion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LineaPobrezaCuc).HasColumnName("LineaPobrezaCUC");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.PersonaAprueba)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PersonaSupervisa)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Resultado).HasColumnName("RESULTADO");
            entity.Property(e => e.Sinirube).HasColumnName("SINIRUBE");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithMany(p => p.Resolucion)
                .HasForeignKey(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Resolucio__carne__6A30C649");
        });

        modelBuilder.Entity<TipoViviendaEstudiante>(entity =>
        {
            entity.HasKey(e => e.CarnetEstudiante).HasName("PK__TipoVivi__087FE8843C32861C");

            entity.Property(e => e.CarnetEstudiante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("carnetEstudiante");

            entity.HasOne(d => d.CarnetEstudianteNavigation).WithOne(p => p.TipoViviendaEstudiante)
                .HasForeignKey<TipoViviendaEstudiante>(d => d.CarnetEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TipoVivie__carne__5812160E");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario).HasName("PK__Usuario__080A97431DD4EA93");

            entity.Property(e => e.Idusuario)
                .ValueGeneratedNever()
                .HasColumnName("idusuario");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Identificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
