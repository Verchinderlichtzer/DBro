using DBro.Shared.Models;
using Microsoft.Identity.Client;

namespace DBro.API.Repositories;

public interface ISalesRepository
{
    #region Sales

    Task<SalesDTO> GetSalesAsync(List<string> includes = null!);

    Task<(SalesDTO, bool?)> FindsSalesAsync(List<string> diskonIds, List<string> promoIds, List<string> includes = null!);

    Task<(SalesDTO, string)> AddsSalesAsync(string idEditor, SalesDTO sales);

    Task<(bool?, string)> UpdatesSalesAsync(string idEditor, List<Diskon> diskon, List<Promo> promo);

    Task<bool?> DeletesSalesAsync(string idEditor, List<string> diskonIds, List<string> promoIds);

    #endregion Sales

    #region Diskon

    Task<List<Diskon>> GetDiskonAsync(List<string> includes = null!);

    Task<(Diskon, bool?)> FindDiskonAsync(string id, List<string> includes = null!);

    #endregion Diskon

    #region Promo

    Task<List<Promo>> GetPromoAsync(List<string> includes = null!);

    Task<(Promo, bool?)> FindPromoAsync(string id, List<string> includes = null!);

    #endregion Promo
}

public class SalesRepository(AppDbContext appDbContext) : ISalesRepository
{
    #region Sales

    public async Task<SalesDTO> GetSalesAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<Diskon> diskonModels = appDbContext.Diskon;
            IQueryable<Promo> promoModels = appDbContext.Promo;

            if (includes != null)
            {
                if (includes.Contains(nameof(Menu)))
                {
                    diskonModels = diskonModels.Include(x => x.Menu);
                    promoModels = promoModels.Include(x => x.MenuDibeli).Include(x => x.MenuDidapat);
                }
            }

            List<Diskon> diskonList = await diskonModels.OrderByDescending(x => x.Id).ToListAsync();
            List<Promo> promoList = await promoModels.OrderByDescending(x => x.Id).ToListAsync();

            return new SalesDTO { Diskon = diskonList, Promo = promoList };
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<(SalesDTO, bool?)> FindsSalesAsync(List<string> diskonIds, List<string> promoIds, List<string> includes = null!)
    {
        try
        {
            IQueryable<Diskon> diskonModels = appDbContext.Diskon;
            IQueryable<Promo> promoModels = appDbContext.Promo;

            if (includes != null)
            {
                if (includes.Contains(nameof(Menu)))
                {
                    diskonModels = diskonModels.Include(x => x.Menu);
                    promoModels = promoModels.Include(x => x.MenuDibeli).Include(x => x.MenuDidapat);
                }
            }

            diskonIds ??= [];
            promoIds ??= [];

            List<Diskon> diskonList = await diskonModels.Where(x => diskonIds.Contains(x.Id)).OrderByDescending(x => x.Id).ToListAsync();
            List<Promo> promoList = await promoModels.Where(x => promoIds.Contains(x.Id)).OrderByDescending(x => x.Id).ToListAsync();

            return diskonList.Count > 0 || promoList.Count > 0 ? (new SalesDTO { Diskon = diskonList, Promo = promoList }, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    public async Task<(SalesDTO, string)> AddsSalesAsync(string idEditor, SalesDTO sales)
    {
        try
        {
            string d = null!;
            string p = null!;

            Nullifies(sales.Diskon);
            Nullifies(sales.Promo);

            if (sales.Diskon.Count > 0)
            {
                string[] diskonIds = GenerateId(await appDbContext.Diskon.Select(x => x.Id).ToListAsync(), 3, "D", sales.Diskon.Count);
                for (int i = 0; i < sales.Diskon.Count; i++) sales.Diskon[i].Id = diskonIds[i];
                await appDbContext.Diskon.AddRangeAsync(sales.Diskon);
                d = $"Id Diskon: {sales.Diskon.Select(x => x.Id).CombineWords()}";
            }
            if (sales.Promo.Count > 0)
            {
                string[] promoIds = GenerateId(await appDbContext.Promo.Select(x => x.Id).ToListAsync(), 3, "P", sales.Promo.Count);
                for (int i = 0; i < sales.Promo.Count; i++) sales.Promo[i].Id = promoIds[i];
                await appDbContext.Promo.AddRangeAsync(sales.Promo);
                p = $"Id Promo: {sales.Promo.Select(x => x.Id).CombineWords()}";
            }
            await appDbContext.Aktivitas.AddAsync(new()
            {
                Email = idEditor,
                Jenis = JenisAktivitas.Tambah,
                Entitas = Entitas.Sales,
                IdEntitas = new List<string> { d, p }.CombineWords(lastSeparator: ". ")
            });
            await appDbContext.SaveChangesAsync();
            return (sales, null!);
        }
        catch (Exception ex)
        {
            List<string> msg = [];
            string message = ex.InnerException!.Message;
            if (message.Contains("PK_Diskon"))
                msg.Add("Id");
            if (message.Contains("IX_Diskon_Telepon"))
                msg.Add("No telepon");
            return (null!, msg.Count > 0 ? $"{msg.CombineWords()} sudah digunakan".CapitalizeSentence() : "Ada kesalahan saat menyimpan data");
        }
    }

    public async Task<(bool?, string)> UpdatesSalesAsync(string idEditor, List<Diskon> diskon, List<Promo> promo)
    {
        try
        {
            string d = null!;
            string p = null!;
            if (diskon.Count > 0)
            {
                List<Diskon> models = await appDbContext.Diskon.Where(x => diskon.Select(y => y.Id).Contains(x.Id)).OrderBy(x => x.Id).ToListAsync();
                diskon = [.. diskon.OrderBy(x => x.Id)];
                for (int i = 0; i < models.Count; i++)
                {
                    models[i].IdMenu = diskon[i].IdMenu;
                    models[i].Nilai = diskon[i].Nilai;
                    models[i].TanggalMulai = diskon[i].TanggalMulai;
                    models[i].TanggalAkhir = diskon[i].TanggalAkhir;
                }
                d = $"Id Diskon: {diskon.Select(x => x.Id).CombineWords()}";
            }
            if (promo.Count > 0)
            {
                List<Promo> models = await appDbContext.Promo.Where(x => promo.Select(y => y.Id).Contains(x.Id)).OrderBy(x => x.Id).ToListAsync();
                promo = [.. promo.OrderBy(x => x.Id)];
                for (int i = 0; i < models.Count; i++)
                {
                    models[i].IdMenuDibeli = promo[i].IdMenuDibeli;
                    models[i].IdMenuDidapat = promo[i].IdMenuDidapat;
                    models[i].JumlahDibeli = promo[i].JumlahDibeli;
                    models[i].JumlahDidapat = promo[i].JumlahDidapat;
                    models[i].TanggalMulai = promo[i].TanggalMulai;
                    models[i].TanggalAkhir = promo[i].TanggalAkhir;
                }
                p = $"Id Promo: {promo.Select(x => x.Id).CombineWords()}";
            }
            await appDbContext.Aktivitas.AddAsync(new()
            {
                Email = idEditor,
                Jenis = JenisAktivitas.Edit,
                Entitas = Entitas.Sales,
                IdEntitas = new List<string> { d, p }.CombineWords(lastSeparator: ". ")
            });
            int rowsAffected = await appDbContext.SaveChangesAsync();

            return (rowsAffected > 0, rowsAffected > 0 ? null! : "Data tidak ditemukan");
        }
        catch (Exception ex)
        {
            List<string> msg = [];
            string message = ex.InnerException!.Message;
            if (message.Contains("IX_Diskon_Telepon"))
                msg.Add("No Telepon");
            return (null!, msg.Count > 0 ? $"{msg.CombineWords()} sudah digunakan".CapitalizeSentence() : "Ada kesalahan saat menyimpan data");
        }
    }

    public async Task<bool?> DeletesSalesAsync(string idEditor, List<string> diskonIds, List<string> promoIds)
    {
        try
        {
            // 1 - Data tidak ditemukan
            // 2 - Ada kesalahan saat menghapus data
            string d = null!;
            string p = null!;
            if (diskonIds?.Count > 0)
            {
                List<Diskon> diskonModels = await appDbContext.Diskon.Where(x => diskonIds.Contains(x.Id)).ToListAsync();
                appDbContext.Diskon.RemoveRange(diskonModels);
                d = $"Id Diskon: {diskonModels.Select(x => x.Id).CombineWords()}";
            }
            if (promoIds?.Count > 0)
            {
                List<Promo> promoModels = await appDbContext.Promo.Where(x => promoIds.Contains(x.Id)).ToListAsync();
                appDbContext.Promo.RemoveRange(promoModels);
                p = $"Id Promo: {promoModels.Select(x => x.Id).CombineWords()}";
            }
            await appDbContext.Aktivitas.AddAsync(new()
            {
                Email = idEditor,
                Jenis = JenisAktivitas.Hapus,
                Entitas = Entitas.Sales,
                IdEntitas = new List<string> { d, p }.CombineWords(lastSeparator: ". ")
            });
            int rowsAffected = await appDbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return null;
        }
    }

    #endregion Sales

    #region Diskon

    public async Task<List<Diskon>> GetDiskonAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<Diskon> models = appDbContext.Diskon;

            if (includes != null)
            {
                if (includes.Contains(nameof(Menu))) models = models.Include(x => x.Menu);
            }

            return await models.OrderByDescending(x => x.Id).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<(Diskon, bool?)> FindDiskonAsync(string id, List<string> includes = null!)
    {
        try
        {
            IQueryable<Diskon> model = appDbContext.Diskon;

            if (includes != null)
            {
                if (includes.Contains(nameof(Menu))) model = model.Include(x => x.Menu);
            }

            Diskon diskon = (await model.FirstOrDefaultAsync(x => x.Id == id))!;

            return diskon != null ? (diskon, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    #endregion Diskon

    #region Promo

    public async Task<List<Promo>> GetPromoAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<Promo> models = appDbContext.Promo;

            if (includes != null)
            {
                if (includes.Contains(nameof(Menu))) models = models.Include(x => x.MenuDibeli);
                if (includes.Contains(nameof(Menu))) models = models.Include(x => x.MenuDidapat);
            }

            return await models.OrderByDescending(x => x.Id).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<(Promo, bool?)> FindPromoAsync(string id, List<string> includes = null!)
    {
        try
        {
            IQueryable<Promo> model = appDbContext.Promo;

            if (includes != null)
            {
                if (includes.Contains(nameof(Menu))) model = model.Include(x => x.MenuDibeli);
                if (includes.Contains(nameof(Menu))) model = model.Include(x => x.MenuDidapat);
            }

            Promo promo = (await model.FirstOrDefaultAsync(x => x.Id == id))!;

            return promo != null ? (promo, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    #endregion Promo
}
