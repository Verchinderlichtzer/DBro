namespace DBro.Shared.Models;

public class Menu
{
    public string Id { get; set; } = null!;

    public Kategori Kategori { get; set; }
    public string Nama { get; set; } = string.Empty;
    public int Harga { get; set; }
    public byte[]? Gambar { get; set; }

    public List<MenuPromoPesanan> MenuPromoPesanan { get; set; } = null!;
    public List<DetailPesanan> DetailPesanan { get; set; } = null!;
    public List<Diskon> Diskon { get; set; } = null!;
    public List<Promo> PromoDibeli { get; set; } = null!;
    public List<Promo> PromoDidapat { get; set; } = null!;
}