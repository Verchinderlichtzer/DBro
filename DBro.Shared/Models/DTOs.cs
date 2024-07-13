using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DBro.Shared.Models;

#region Auth

public class LoginDTO()
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class TokenClaimsDTO
{
    public string Token { get; set; } = string.Empty;
    public IEnumerable<ClaimDTO> Claims { get; set; } = null!;
}

public class ClaimDTO
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

#endregion Auth

#region Sales

/// <summary> Data yang diperlukan di List dan Form Sales </summary>
public class SalesDTO
{
    public List<Diskon> Diskon { get; set; } = null!; // Used in List & Form Sales
    public List<Promo> Promo { get; set; } = null!; // Used in List & Form Sales
    public List<Menu> Menu { get; set; } = null!; // Used in Form Sales
}

#endregion Sales

#region Pesanan

public class PesananFormDTO // Data yang diperlukan di Form Pesanan
{
    public Pesanan Pesanan { get; set; } = null!;
    public List<Menu> Menu { get; set; } = null!;
    //public List<VarianMenu> VarianMenu { get; set; } = null!;
    public List<Diskon> Diskon { get; set; } = null!;
    public List<Promo> Promo { get; set; } = null!;
}

public class MenuDidapatDTO // Menu yang didapat dari promo
{
    public Menu Menu { get; set; } = null!;
    public int Jumlah { get; set; }
}

#endregion Pesanan

#region Chart / Graph

public class LineChartDTO
{
    public int No { get; set; }
    public DateTime Tanggal { get; set; }
    public int Nominal { get; set; }
}

#endregion Chart / Graph

#region Lainnya

public class PeriodikDTO
{
    private DateTime _dari = DateTime.Today;
    private DateTime _sampai = DateTime.Today;
    public DateTime Dari
    {
        get { return _dari; }
        set { _dari = value; if (_dari > _sampai) _sampai = _dari; }
    }
    public DateTime Sampai
    {
        get { return _sampai; }
        set { _sampai = value; if (_sampai < _dari) _dari = _sampai; }
    }
}

public class EntitasDTO
{
    public string Id { get; set; } = string.Empty;
    public string Subjek { get; set; } = string.Empty; // Teks yang tampil pada list laporan entitas, biasanya nama objek.
    public string Deskripsi { get; set; } = string.Empty; // Informasi tambahan tentang objek tadi, property selain dari subjek.
}

public class RekapitulasiDTO
{
    public string Periode { get; set; } = string.Empty;
    public int JumlahPeminjaman { get; set; }
    public int Pendapatan { get; set; }
    public int Pengeluaran { get; set; }
    public int Total { get; set; }
}

#endregion Lainnya