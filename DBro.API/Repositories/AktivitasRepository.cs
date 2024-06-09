namespace DBro.API.Repositories;

/// <summary>
/// GET Includable :
/// <br/>
/// • One to Many - <see cref="Aktivitas"/>
/// </summary>
public interface IAktivitasRepository
{
    Task<List<Aktivitas>> GetAsync(List<string> includes = null!);

    Task<bool?> DeleteAsync();
}

public class AktivitasRepository(AppDbContext appDbContext) : IAktivitasRepository
{
    public async Task<List<Aktivitas>> GetAsync(List<string> includes = null!)
    {
        try
        {
            IQueryable<Aktivitas> models = appDbContext.Aktivitas;

            if (includes != null)
            {
                if (includes.Contains(nameof(User))) models = models.Include(x => x.User);
            }

            return await models.OrderByDescending(x => x.Id).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<bool?> DeleteAsync()
    {
        try
        {
            int rowsAffected = await appDbContext.Aktivitas.ExecuteDeleteAsync();
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return null;
        }
    }
}