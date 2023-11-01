﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using api.context;

#nullable disable

namespace api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231029232501_mudanca na model da pergunta, enunciado para conteudo")]
    partial class mudancanamodeldaperguntaenunciadoparaconteudo
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("api.Models.Pergunta", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Conteudo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("RequisicaoId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Valided")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Perguntas");
                });

            modelBuilder.Entity("api.Models.Requisicao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DataFim")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DataInicio")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<bool>("Valided")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Requisicoes");
                });

            modelBuilder.Entity("api.Models.Resposta", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Conteudo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Correta")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("PerguntaId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Valided")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("PerguntaId");

                    b.ToTable("Respostas");
                });

            modelBuilder.Entity("api.Models.TextoBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RequisicaoId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("TextosBase");
                });

            modelBuilder.Entity("api.Models.Resposta", b =>
                {
                    b.HasOne("api.Models.Pergunta", null)
                        .WithMany("Respostas")
                        .HasForeignKey("PerguntaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("api.Models.Pergunta", b =>
                {
                    b.Navigation("Respostas");
                });
#pragma warning restore 612, 618
        }
    }
}
