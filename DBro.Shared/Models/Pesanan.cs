namespace DBro.Shared.Models;

public class Pesanan
{
    public string Id { get; set; } = null!;
    public string? Email { get; set; }

    public DateTime? Tanggal { get; set; }
    public int Subtotal { get; set; }
    public int Potongan { get; set; }
    public int Bayar { get; set; }
    public StatusPesanan StatusPesanan { get; set; }

    public List<DetailPesanan> DetailPesanan { get; set; } = null!;
    public User User { get; set; } = null!;

    public int Total => Subtotal - Potongan;
    public int Sisa => Bayar - Total;
    public TimeSpan? Jam { get; set; }
}