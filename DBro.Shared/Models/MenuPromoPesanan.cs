namespace DBro.Shared.Models;

public class MenuPromoPesanan
{
    public int Id { get; set; }
    public string IdPesanan { get; set; } = null!;
    public string IdMenu { get; set; } = null!;

    public int Jumlah { get; set; }

    public Pesanan Pesanan { get; set; } = null!;
    public Menu Menu { get; set; } = null!;
}
