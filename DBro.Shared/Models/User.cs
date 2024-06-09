namespace DBro.Shared.Models;

public class User
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public JenisKelamin JenisKelamin { get; set; }
    public DateTime? TanggalLahir { get; set; } = DateTime.Today;
    public string Alamat { get; set; } = string.Empty;
    public string? Telepon { get; set; }
    public JenisUser JenisUser { get; set; }

    public List<Aktivitas> Aktivitas { get; set; } = null!;
    public List<Pesanan> Pesanan { get; set; } = null!;
}