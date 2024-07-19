using static DBro.Shared.Extensions.EncryptionHelper;

namespace DBro.API.Repositories;

/// <summary>
/// GET Includable :
/// <br/>
/// • One to Many - <see cref="Aktivitas"/>
/// <br/>
/// • One to Many - <see cref="Pesanan"/>
/// </summary>
public interface IUserRepository
{
    Task<List<User>> GetAsync(List<string> includes = null!);

    Task<(User, bool?)> FindAsync(string email, List<string> includes = null!);

    Task<(User, string)> AddAsync(User user);

    Task<(bool?, string)> UpdateAsync(User user);

    Task<byte> DeleteAsync(string email);
}

public class UserRepository(AppDbContext appDbContext) : IUserRepository
{
    public async Task<List<User>> GetAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<User> models = appDbContext.User;

            if (includes != null)
            {
                if (includes.Contains(nameof(Pesanan))) models = models.Include(x => x.Pesanan);
            }

            return await models.OrderBy(x => x.Nama).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<(User, bool?)> FindAsync(string email, List<string> includes = null!)
    {
        try
        {
            IQueryable<User> model = appDbContext.User;

            if (includes != null)
            {
                if (includes.Contains(nameof(Pesanan))) model = model.Include(x => x.Pesanan);
            }

            User user = (await model.FirstOrDefaultAsync(x => x.Email == email))!;

            return user != null ? (user, true) : (null!, false);
        }
        catch (Exception)
        {
            return (null!, null);
        }
    }

    public async Task<(User, string)> AddAsync(User user)
    {
        try
        {
            var model = await appDbContext.User.AddAsync(user);
            await appDbContext.SaveChangesAsync();
            return (model.Entity, null!);
        }
        catch (Exception ex)
        {
            List<string> msg = [];
            string message = ex.InnerException!.Message;
            if (message.Contains("PK_User"))
                msg.Add("Email");
            if (message.Contains("IX_User_Telepon"))
                msg.Add("No telepon");
            return (null!, msg.Count > 0 ? $"{msg.CombineWords()} sudah digunakan".CapitalizeSentence() : "Ada kesalahan saat menyimpan data");
        }
    }

    public async Task<(bool?, string)> UpdateAsync(User user)
    {
        try
        {
            User model = (await appDbContext.User.FirstOrDefaultAsync(x => x.Email == user.Email))!;
            if (model != null)
            {
                model.Password = Encrypt(user.Password);
                model.Nama = user.Nama;
                model.JenisKelamin = user.JenisKelamin;
                model.TanggalLahir = user.TanggalLahir;
                model.Alamat = user.Alamat;
                model.Telepon = user.Telepon;
                model.JenisUser = user.JenisUser;
                await appDbContext.SaveChangesAsync();
            }
            return (model != null, model != null ? null! : "User tidak ditemukan");
        }
        catch (Exception ex)
        {
            List<string> msg = [];
            string message = ex.InnerException!.Message;
            if (message.Contains("IX_User_Telepon"))
                msg.Add("No Telepon");
            return (null!, msg.Count > 0 ? $"{msg.CombineWords()} sudah digunakan".CapitalizeSentence() : "Ada kesalahan saat menyimpan data");
        }
    }

    public async Task<byte> DeleteAsync(string email)
    {
        try
        {
            // 1 - User tidak ditemukan
            // 2 - User pernah bertransaksi
            // 3 - User pernah bertugas
            // 4 - Ada kesalahan saat menghapus data
            User model = (await appDbContext.User.FirstOrDefaultAsync(x => x.Email == email))!;
            if (model != null)
            {
                bool removable = await appDbContext.User.Include(x => x.Pesanan).Where(x => x.JenisUser == JenisUser.Karyawan).AnyAsync(x => x.Email == email && x.Pesanan.Count == 0);
                if (!removable) return 3;
                appDbContext.User.Remove(model);
                await appDbContext.SaveChangesAsync();
                return 0;
            }
            return 1;
        }
        catch (Exception)
        {
            return 4;
        }
    }
}