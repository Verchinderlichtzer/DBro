namespace DBro.Shared.Models;

public class VarianMenu
{
    public int Id { get; set; }
    public string IdMenu { get; set; } = null!;

    public JenisVarian JenisVarian { get; set; }
    public string Nama { get; set; } = string.Empty;
    public int Harga { get; set; }

    public Menu Menu { get; set; } = null!;
    public List<DetailPesanan> DetailPesanan { get; set; } = null!;

    public bool Removable { get; set; }
}
