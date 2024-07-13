namespace DBro.Shared.Models;

public class Aktivitas
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;

    public DateTime Tanggal { get; set; } = DateTime.Now;
    public JenisAktivitas Jenis { get; set; }
    public Entitas Entitas { get; set; }
    public string IdEntitas { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}