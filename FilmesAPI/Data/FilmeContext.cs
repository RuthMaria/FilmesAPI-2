﻿using FilmesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmesAPI.Data;

public class FilmeContext : DbContext
{
    private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=filme;Integrated Security=True;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString).UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Sessao>()
            .HasKey(sessao => new { sessao.FilmeId, sessao.CinemaId });

        builder.Entity<Sessao>()
            .HasOne(sessao => sessao.Cinema)
            .WithMany(cinema => cinema.Sessoes)
            .HasForeignKey(sessao => sessao.CinemaId);

        builder.Entity<Sessao>()
            .HasOne(sessao => sessao.Filme)
            .WithMany(filme => filme.Sessoes)
            .HasForeignKey(sessao => sessao.FilmeId);
    }

    public DbSet<Filme> Filmes { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Sessao> Sessoes { get; set; }

}
