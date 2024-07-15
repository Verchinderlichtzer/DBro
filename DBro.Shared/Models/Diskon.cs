namespace DBro.Shared.Models;

public class Diskon
{
    public string Id { get; set; } = null!;
    public string IdMenu { get; set; } = null!;

    public decimal Nilai { get; set; }
    public DateTime? TanggalMulai { get; set; }
    public DateTime? TanggalAkhir { get; set; }

    public Menu Menu { get; set; } = null!;
}