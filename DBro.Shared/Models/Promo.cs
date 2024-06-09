namespace DBro.Shared.Models;

public class Promo
{
    public string Id { get; set; } = null!;
    public string IdMenuDibeli { get; set; } = null!;
    public string IdMenuDidapat { get; set; } = null!;

    public int JumlahDibeli { get; set; }
    public int JumlahDidapat { get; set; }
    public DateTime? TanggalMulai { get; set; }
    public DateTime? TanggalAkhir { get; set; }

    public Menu MenuDibeli { get; set; } = null!;
    public Menu MenuDidapat { get; set; } = null!;
}