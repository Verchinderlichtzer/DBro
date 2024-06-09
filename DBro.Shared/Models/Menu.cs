namespace DBro.Shared.Models;

public class Menu
{
    public string Id { get; set; } = null!;

    public JenisMenu JenisMenu { get; set; }
    public string Nama { get; set; } = string.Empty;
    public int Harga { get; set; }
    public byte[]? Gambar { get; set; }

    public List<VarianMenu> VarianMenu { get; set; } = null!;
    public List<DetailPesanan> DetailPesanan { get; set; } = null!;
    public Diskon Diskon { get; set; } = null!;
    public Promo PromoDibeli { get; set; } = null!;
    public Promo PromoDidapat { get; set; } = null!;
}