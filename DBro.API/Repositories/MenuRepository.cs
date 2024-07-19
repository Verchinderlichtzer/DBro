namespace DBro.API.Repositories;

/// <summary>
/// GET Includable :
/// <br/>
/// • One to Many - <see cref="MenuPromoPesanan"/>
/// <br/>
/// • One to Many - <see cref="DetailPesanan"/>
/// </summary>
public interface IMenuRepository
{
    #region Menu

    Task<List<Menu>> GetAsync(List<string> includes = null!);

    Task<(Menu, bool?)> FindAsync(string id, List<string> includes = null!);

    Task<(Menu, string)> AddAsync(Menu menu);

    Task<(bool?, string)> UpdateAsync(Menu menu);

    Task<byte> DeleteAsync(string id);

    #endregion Menu

    //#region Varian Menu

    //Task<List<VarianMenu>> GetVarianAsync(List<string> includes = null!);

    //Task<bool?> UpdatesVarianAsync(List<VarianMenu> menu);

    //#endregion Varian Menu
}

public class MenuRepository(AppDbContext appDbContext) : IMenuRepository
{
    #region Menu

    public async Task<List<Menu>> GetAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<Menu> models = appDbContext.Menu;

            if (includes != null)
            {
                //if (includes.Contains(nameof(VarianMenu))) models = models.Include(x => x.VarianMenu);
                if (includes.Contains(nameof(MenuPromoPesanan))) models = models.Include(x => x.MenuPromoPesanan);
                if (includes.Contains(nameof(DetailPesanan))) models = models.Include(x => x.DetailPesanan);
            }

            return await models.OrderBy(x => x.Nama).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<(Menu, bool?)> FindAsync(string id, List<string> includes = null!)
    {
        try
        {
            IQueryable<Menu> model = appDbContext.Menu;

            if (includes != null)
            {
                //if (includes.Contains(nameof(VarianMenu))) model = model.Include(x => x.VarianMenu);
                if (includes.Contains(nameof(MenuPromoPesanan))) model = model.Include(x => x.MenuPromoPesanan);
                if (includes.Contains(nameof(DetailPesanan))) model = model.Include(x => x.DetailPesanan);
            }

            Menu menu = (await model.FirstOrDefaultAsync(x => x.Id == id))!;

            return menu != null ? (menu, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    public async Task<(Menu, string)> AddAsync(Menu menu)
    {
        try
        {
            menu.Id = GenerateId(await appDbContext.Menu.Select(x => x.Id).ToListAsync(), 4, "M");
            var model = await appDbContext.Menu.AddAsync(menu);
            
            await appDbContext.SaveChangesAsync();
            return (model.Entity, null!);
        }
        catch (Exception)
        {
            List<string> msg = [];
            //string message = ex.InnerException!.Message;
            //if (message.Contains("PK_User"))
            //    msg.Add("Email");
            //if (message.Contains("IX_User_Telepon"))
            //    msg.Add("No telepon");
            return (null!, msg.Count > 0 ? $"{msg.CombineWords()} sudah digunakan".CapitalizeSentence() : "Ada kesalahan saat menyimpan data");
        }
    }

    public async Task<(bool?, string)> UpdateAsync(Menu menu)
    {
        try
        {
            Menu model = (await appDbContext.Menu.FirstOrDefaultAsync(x => x.Id == menu.Id))!;
            if (model != null)
            {
                model.Nama = menu.Nama;
                model.Kategori = menu.Kategori;
                model.Harga = menu.Harga;
                model.Gambar = menu.Gambar;
                
                await appDbContext.SaveChangesAsync();
            }
            return (model != null, model != null ? null! : "Menu tidak ditemukan");
        }
        catch (Exception)
        {
            List<string> msg = [];
            //string message = ex.InnerException!.Message;
            //if (message.Contains("IX_User_Telepon"))
            //    msg.Add("No Telepon");
            return (null!, msg.Count > 0 ? $"{msg.CombineWords()} sudah digunakan".CapitalizeSentence() : "Ada kesalahan saat menyimpan data");
        }
    }

    public async Task<byte> DeleteAsync(string id)
    {
        try
        {
            // 1 - Menu tidak ditemukan
            // 2 - Menu pernah bertransaksi
            // 3 - Ada kesalahan saat menghapus data
            Menu model = (await appDbContext.Menu.FirstOrDefaultAsync(x => x.Id == id))!;
            if (model != null)
            {
                bool removable = await appDbContext.Menu.Include(x => x.DetailPesanan).AnyAsync(x => x.Id == id && x.DetailPesanan.Count == 0);
                if (!removable) return 2;
                appDbContext.Menu.Remove(model);
                
                await appDbContext.SaveChangesAsync();
                return 0;
            }
            return 1;
        }
        catch (Exception)
        {
            return 3;
        }
    }

    #endregion Menu
}