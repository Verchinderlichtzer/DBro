namespace DBro.Shared.Models;
public class DetailPesanan
{
    public string IdPesanan { get; set; } = null!;
    public string IdMenu { get; set; } = null!;
    public int? IdVarianMenu { get; set; }

    public int Jumlah { get; set; }
    public int Harga { get; set; }
    public int Subtotal { get; set; }
    public decimal Diskon { get; set; }

    public Pesanan Pesanan { get; set; } = null!;
    public Menu Menu { get; set; } = null!;
    public VarianMenu VarianMenu { get; set; } = null!;

    public int Total => Subtotal - (int)(Subtotal * Diskon);
}