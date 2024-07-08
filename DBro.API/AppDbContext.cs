namespace DBro.API;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region DbSet

    public DbSet<User> User { get; set; }
    public DbSet<Menu> Menu { get; set; }

    //public DbSet<VarianMenu> VarianMenu { get; set; }

    public DbSet<MenuPromoPesanan> MenuPromoPesanan { get; set; }
    public DbSet<Pesanan> Pesanan { get; set; }
    public DbSet<DetailPesanan> DetailPesanan { get; set; }
    public DbSet<Aktivitas> Aktivitas { get; set; }
    public DbSet<Diskon> Diskon { get; set; }
    public DbSet<Promo> Promo { get; set; }

    #endregion DbSet

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Server=.;Database=DBro;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Model Configuration

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Email);
            e.HasIndex(x => x.Telepon).IsUnique();
        });

        modelBuilder.Entity<Aktivitas>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.Aktivitas).HasForeignKey(x => x.Email);
        });

        //modelBuilder.Entity<VarianMenu>(e =>
        //{
        //    e.HasOne(x => x.Menu).WithMany(x => x.VarianMenu).HasForeignKey(x => x.IdMenu);
        //    e.HasIndex(x => x.Nama).IsUnique();
        //    e.Ignore(x => x.Removable);
        //});

        modelBuilder.Entity<Pesanan>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.Pesanan).HasForeignKey(x => x.Email);
            e.Ignore(x => x.Jam);
        });

        modelBuilder.Entity<DetailPesanan>(e =>
        {
            e.HasKey(x => new { x.IdPesanan, x.IdMenu });
            e.HasOne(x => x.Pesanan).WithMany(x => x.DetailPesanan).HasForeignKey(x => x.IdPesanan);
            e.HasOne(x => x.Menu).WithMany(x => x.DetailPesanan).HasForeignKey(x => x.IdMenu);
            //e.HasOne(x => x.VarianMenu).WithMany(x => x.DetailPesanan).HasForeignKey(x => x.IdVarianMenu);
            e.Property(x => x.Diskon).HasPrecision(5, 2);
        });

        modelBuilder.Entity<MenuPromoPesanan>(e =>
        {
            e.HasOne(x => x.Pesanan).WithMany(x => x.MenuPromoPesanan).HasForeignKey(x => x.IdPesanan);
            e.HasOne(x => x.Menu).WithMany(x => x.MenuPromoPesanan).HasForeignKey(x => x.IdMenu);
        });

        modelBuilder.Entity<Diskon>(e =>
        {
            e.HasOne(x => x.Menu).WithMany(x => x.Diskon).HasForeignKey(x => x.IdMenu);
            e.Property(x => x.Nilai).HasPrecision(5, 2);
        });

        modelBuilder.Entity<Promo>(e =>
        {
            e.HasOne(x => x.MenuDibeli).WithMany(x => x.PromoDibeli).HasForeignKey(x => x.IdMenuDibeli).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.MenuDidapat).WithMany(x => x.PromoDidapat).HasForeignKey(x => x.IdMenuDidapat).OnDelete(DeleteBehavior.NoAction);
        });

        #endregion Model Configuration

        #region Data Initializer

        modelBuilder.Entity<User>().HasData(
        new User
        {
            Email = "sujudihanif36@gmail.com",
            Password = "IvCkErOjG9A8DPW7X23rJg==",
            Nama = "Sujudi Hanif",
            JenisKelamin = JenisKelamin.Pria,
            TanggalLahir = new(2002, 6, 11),
            Alamat = "Perumahan Bumi Anggrek Blok K No 80",
            Telepon = "085739194810"
        });

        modelBuilder.Entity<Menu>().HasData(
        new Menu
        {
            Id = "M-0001",
            Nama = "Sayap",
            JenisMenu = JenisMenu.Chicken,
            Harga = 7000
        },
        new Menu
        {
            Id = "M-0002",
            Nama = "Paha Bawah",
            JenisMenu = JenisMenu.Chicken,
            Harga = 8000
        },
        new Menu
        {
            Id = "M-0003",
            Nama = "Dada",
            JenisMenu = JenisMenu.Chicken,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0004",
            Nama = "Paha Atas",
            JenisMenu = JenisMenu.Chicken,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0005",
            Nama = "DBRO 1 Sayap + Nasi",
            JenisMenu = JenisMenu.PaketNasi,
            Harga = 10500
        },
        new Menu
        {
            Id = "M-0006",
            Nama = "DBRO 2 Paha Bawah + Nasi",
            JenisMenu = JenisMenu.PaketNasi,
            Harga = 11500
        },
        new Menu
        {
            Id = "M-0007",
            Nama = "DBRO 3 Dada / Paha Atas + Nasi",
            JenisMenu = JenisMenu.PaketNasi,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0008",
            Nama = "Spaghetti",
            JenisMenu = JenisMenu.Spaghetti,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0009",
            Nama = "DBRO Burger",
            JenisMenu = JenisMenu.Burger,
            Harga = 8000
        },
        new Menu
        {
            Id = "M-0010",
            Nama = "DBRO Reguler",
            JenisMenu = JenisMenu.Burger,
            Harga = 10500
        },
        new Menu
        {
            Id = "M-0011",
            Nama = "DBRO Chiz",
            JenisMenu = JenisMenu.Burger,
            Harga = 12000
        },
        new Menu
        {
            Id = "M-0012",
            Nama = "DBRO Premium",
            JenisMenu = JenisMenu.Burger,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0013",
            Nama = "DBRO Premium Chiz",
            JenisMenu = JenisMenu.Burger,
            Harga = 15000
        },
        new Menu
        {
            Id = "M-0014",
            Nama = "DBROsis",
            JenisMenu = JenisMenu.DBROsis,
            Harga = 9000
        },
        new Menu
        {
            Id = "M-0015",
            Nama = "DBROsis + Kentang + Minum*",
            JenisMenu = JenisMenu.DBROsis,
            Harga = 17000
        },
        new Menu
        {
            Id = "M-0016",
            Nama = "Sayhot Dada",
            JenisMenu = JenisMenu.Sayhot,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0017",
            Nama = "Sayhot Sayap",
            JenisMenu = JenisMenu.Sayhot,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0018",
            Nama = "Sayhot Paha Bawah",
            JenisMenu = JenisMenu.Sayhot,
            Harga = 11000
        },
        new Menu
        {
            Id = "M-0019",
            Nama = "Tteokbokki Reguler",
            JenisMenu = JenisMenu.Tteokbokki,
            Harga = 21000
        },
        new Menu
        {
            Id = "M-0020",
            Nama = "Tteokbokki Medium",
            JenisMenu = JenisMenu.Tteokbokki,
            Harga = 30000
        },
        new Menu
        {
            Id = "M-0021",
            Nama = "Tteokbokki Large",
            JenisMenu = JenisMenu.Tteokbokki,
            Harga = 40000
        },
        new Menu
        {
            Id = "M-0022",
            Nama = "Sayap Sambal Merah",
            JenisMenu = JenisMenu.DPrek,
            Harga = 11000
        },
        new Menu
        {
            Id = "M-0023",
            Nama = "Paha Bawah Sambal Merah",
            JenisMenu = JenisMenu.DPrek,
            Harga = 12000
        },
        new Menu
        {
            Id = "M-0024",
            Nama = "Kulit",
            JenisMenu = JenisMenu.Chicken,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0025",
            Nama = "Prima",
            JenisMenu = JenisMenu.Minuman,
            Harga = 3000
        },
        new Menu
        {
            Id = "M-0026",
            Nama = "Teh Botol Sosro",
            JenisMenu = JenisMenu.Minuman,
            Harga = 4000
        },
        new Menu
        {
            Id = "M-0027",
            Nama = "S-tee",
            JenisMenu = JenisMenu.Minuman,
            Harga = 4000
        },
        new Menu
        {
            Id = "M-0028",
            Nama = "Nestle Orange",
            JenisMenu = JenisMenu.Minuman,
            Harga = 5500
        },
        new Menu
        {
            Id = "M-0029",
            Nama = "Milo",
            JenisMenu = JenisMenu.Minuman,
            Harga = 6000
        });

        #endregion Data Initializer
    }
}