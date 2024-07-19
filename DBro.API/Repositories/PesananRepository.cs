using DBro.Shared.Models;

namespace DBro.API.Repositories;

/// <summary>
/// GET Includable :
/// <br/>
/// • One to Many - <see cref="User"/>
/// <br/>
/// • One to Many - <see cref="DetailPesanan"/>
/// <br/>
/// • One to Many - <see cref="MenuPromoPesanan"/>
/// </summary>
public interface IPesananRepository
{
    #region Pesanan

    Task<List<Pesanan>> GetAsync(List<string> includes = null!);

    Task<(Pesanan, bool?)> FindAsync(string id, List<string> includes = null!);

    Task<(Pesanan, string)> AddAsync(Pesanan pesanan);

    Task<(bool?, string)> UpdateAsync(Pesanan pesanan);

    Task<byte> DeleteAsync(string id);

    Task<(DetailPesanan, bool?)> FindDetailAsync(string idPesanan, string idMenu);

    #endregion Pesanan
}

public class PesananRepository(AppDbContext appDbContext) : IPesananRepository
{
    #region Pesanan

    public async Task<List<Pesanan>> GetAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<Pesanan> models = appDbContext.Pesanan;

            if (includes != null)
            {
                if (includes.Contains(nameof(DetailPesanan))) models = models.Include(x => x.DetailPesanan);
                if (includes.Contains(nameof(Menu))) models = models.Include(x => x.DetailPesanan).ThenInclude(x => x.Menu);
                if (includes.Contains(nameof(User))) models = models.Include(x => x.User);
                if (includes.Contains(nameof(MenuPromoPesanan))) models = models.Include(x => x.MenuPromoPesanan);
                if (includes.Contains(nameof(Menu))) models = models.Include(x => x.MenuPromoPesanan).ThenInclude(x => x.Menu);
            }

            return await models.OrderByDescending(x => x.Id).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<(Pesanan, bool?)> FindAsync(string id, List<string> includes = null!)
    {
        try
        {
            IQueryable<Pesanan> model = appDbContext.Pesanan;

            if (includes != null)
            {
                if (includes.Contains(nameof(DetailPesanan))) model = model.Include(x => x.DetailPesanan);
                if (includes.Contains(nameof(Menu))) model = model.Include(x => x.DetailPesanan).ThenInclude(x => x.Menu);
                if (includes.Contains(nameof(User))) model = model.Include(x => x.User);
                if (includes.Contains(nameof(MenuPromoPesanan))) model = model.Include(x => x.MenuPromoPesanan);
                if (includes.Contains(nameof(Menu))) model = model.Include(x => x.MenuPromoPesanan).ThenInclude(x => x.Menu);
            }

            Pesanan pesanan = (await model.FirstOrDefaultAsync(x => x.Id == id))!;

            return pesanan != null ? (pesanan, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    public async Task<(Pesanan, string)> AddAsync(Pesanan pesanan)
    {
        try
        {
            pesanan.Id = GenerateId("PSN", 3, DateTime.Today, appDbContext.Pesanan.Where(x => x.Tanggal!.Value.Date == DateTime.Today).Select(x => x.Id));

            pesanan.DetailPesanan.ForEach(x => { x.Menu = null!; x.IdPesanan = pesanan.Id; });
            pesanan.MenuPromoPesanan.ForEach(x => { x.Menu = null!; x.IdPesanan = pesanan.Id; });
            var model = await appDbContext.Pesanan.AddAsync(pesanan);
            
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

    public async Task<(bool?, string)> UpdateAsync(Pesanan pesanan)
    {
        try
        {
            Pesanan modelPesanan = (await appDbContext.Pesanan.FirstOrDefaultAsync(x => x.Id == pesanan.Id))!;

            modelPesanan.Tanggal = pesanan.Tanggal;
            modelPesanan.Subtotal = pesanan.Subtotal;
            modelPesanan.Bayar = pesanan.Bayar;
            appDbContext.DetailPesanan.RemoveRange(await appDbContext.DetailPesanan.Where(x => x.IdPesanan == pesanan.Id).ToListAsync());

            var pesananDetail = Nullifies(pesanan.DetailPesanan!);

            await appDbContext.DetailPesanan.AddRangeAsync(pesananDetail);

            int rowsAffected = await appDbContext.SaveChangesAsync();

            return (rowsAffected > 0, rowsAffected > 0 ? null! : "Pesanan tidak ditemukan");
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
            // 1 - Pesanan tidak ditemukan
            // 2 - Pesanan pernah bertransaksi
            // 3 - Ada kesalahan saat menghapus data
            Pesanan model = (await appDbContext.Pesanan.FirstOrDefaultAsync(x => x.Id == id))!;
            if (model != null)
            {
                //bool removable = await appDbContext.Pesanan.Include(x => x.DetailPesanan).AnyAsync(x => x.Id == id && x.DetailPesanan.Count == 0);
                //if (!removable) return 2;
                appDbContext.Pesanan.Remove(model);
                
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

    public async Task<(DetailPesanan, bool?)> FindDetailAsync(string idPesanan, string idMenu)
    {
        try
        {
            DetailPesanan detailPesanan = (await appDbContext.DetailPesanan.FirstOrDefaultAsync(x => x.IdPesanan == idPesanan && x.IdMenu == idMenu))!;

            return detailPesanan != null ? (detailPesanan, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    #endregion Pesanan
}