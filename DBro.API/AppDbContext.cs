namespace DBro.API;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region DbSet

    public DbSet<User> User { get; set; }
    public DbSet<Menu> Menu { get; set; }
    public DbSet<MenuPromoPesanan> MenuPromoPesanan { get; set; }
    public DbSet<Pesanan> Pesanan { get; set; }
    public DbSet<DetailPesanan> DetailPesanan { get; set; }
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
            Email = "admin@gmail.com",
            Password = "OTBnuUPex7mRFGGakAC5Qw==",
            Nama = "Admin",
            JenisKelamin = JenisKelamin.Pria,
            TanggalLahir = new(1974, 3, 28),
            Alamat = "Bekasi",
            Telepon = "0853 6466 2362"
        });

        modelBuilder.Entity<Menu>().HasData(
        new Menu
        {
            Id = "M-0001",
            Nama = "Sayap",
            Kategori = Kategori.Chicken,
            Harga = 7000
        },
        new Menu
        {
            Id = "M-0002",
            Nama = "Paha Bawah",
            Kategori = Kategori.Chicken,
            Harga = 8000
        },
        new Menu
        {
            Id = "M-0003",
            Nama = "Dada",
            Kategori = Kategori.Chicken,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0004",
            Nama = "Paha Atas",
            Kategori = Kategori.Chicken,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0005",
            Nama = "DBRO 1 Sayap + Nasi",
            Kategori = Kategori.PaketNasi,
            Harga = 10500
        },
        new Menu
        {
            Id = "M-0006",
            Nama = "DBRO 2 Paha Bawah + Nasi",
            Kategori = Kategori.PaketNasi,
            Harga = 11500
        },
        new Menu
        {
            Id = "M-0007",
            Nama = "DBRO 3 Dada / Paha Atas + Nasi",
            Kategori = Kategori.PaketNasi,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0008",
            Nama = "Spaghetti",
            Kategori = Kategori.Spaghetti,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0009",
            Nama = "DBRO Burger",
            Kategori = Kategori.Burger,
            Harga = 8000
        },
        new Menu
        {
            Id = "M-0010",
            Nama = "DBRO Reguler",
            Kategori = Kategori.Burger,
            Harga = 10500
        },
        new Menu
        {
            Id = "M-0011",
            Nama = "DBRO Chiz",
            Kategori = Kategori.Burger,
            Harga = 12000
        },
        new Menu
        {
            Id = "M-0012",
            Nama = "DBRO Premium",
            Kategori = Kategori.Burger,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0013",
            Nama = "DBRO Premium Chiz",
            Kategori = Kategori.Burger,
            Harga = 15000
        },
        new Menu
        {
            Id = "M-0014",
            Nama = "DBROsis",
            Kategori = Kategori.DBROsis,
            Harga = 9000
        },
        new Menu
        {
            Id = "M-0015",
            Nama = "DBROsis + Kentang + Minum*",
            Kategori = Kategori.DBROsis,
            Harga = 17000
        },
        new Menu
        {
            Id = "M-0016",
            Nama = "Sayhot Dada",
            Kategori = Kategori.Sayhot,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0017",
            Nama = "Sayhot Sayap",
            Kategori = Kategori.Sayhot,
            Harga = 10000
        },
        new Menu
        {
            Id = "M-0018",
            Nama = "Sayhot Paha Bawah",
            Kategori = Kategori.Sayhot,
            Harga = 11000
        },
        new Menu
        {
            Id = "M-0019",
            Nama = "Tteokbokki Reguler",
            Kategori = Kategori.Tteokbokki,
            Harga = 21000
        },
        new Menu
        {
            Id = "M-0020",
            Nama = "Tteokbokki Medium",
            Kategori = Kategori.Tteokbokki,
            Harga = 30000
        },
        new Menu
        {
            Id = "M-0021",
            Nama = "Tteokbokki Large",
            Kategori = Kategori.Tteokbokki,
            Harga = 40000
        },
        new Menu
        {
            Id = "M-0022",
            Nama = "Sayap Sambal Merah",
            Kategori = Kategori.DPrek,
            Harga = 11000
        },
        new Menu
        {
            Id = "M-0023",
            Nama = "Paha Bawah Sambal Merah",
            Kategori = Kategori.DPrek,
            Harga = 12000
        },
        new Menu
        {
            Id = "M-0024",
            Nama = "Kulit",
            Kategori = Kategori.Chicken,
            Harga = 13000
        },
        new Menu
        {
            Id = "M-0025",
            Nama = "Prima",
            Kategori = Kategori.Minuman,
            Harga = 3000
        },
        new Menu
        {
            Id = "M-0026",
            Nama = "Teh Botol Sosro",
            Kategori = Kategori.Minuman,
            Harga = 4000
        },
        new Menu
        {
            Id = "M-0027",
            Nama = "S-tee",
            Kategori = Kategori.Minuman,
            Harga = 4000
        },
        new Menu
        {
            Id = "M-0028",
            Nama = "Nestle Orange",
            Kategori = Kategori.Minuman,
            Harga = 5500
        },
        new Menu
        {
            Id = "M-0029",
            Nama = "Milo",
            Kategori = Kategori.Minuman,
            Harga = 6000
        });

        #endregion Data Initializer
    }
}