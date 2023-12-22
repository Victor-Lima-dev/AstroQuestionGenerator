using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.context
{
    public class AppDbContext : DbContext
    {
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
      {

      }

      public DbSet<Requisicao> Requisicoes { get; set; }

        public DbSet<Pergunta> Perguntas { get; set; }

        public DbSet<TextoBase> TextosBase { get; set; }

        public DbSet<TAG> TAGs { get; set; }
        
        public DbSet<Resposta> Respostas { get; set; }

        public DbSet<Lista> Listas { get; set; }
      
    }
}