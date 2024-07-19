namespace DBro.Shared.Extensions;

public enum JenisKelamin : byte
{
    None,
    [Description("Laki-laki")] Pria,
    [Description("Perempuan")] Wanita
}

public enum JenisUser : byte
{
    Admin,
    Karyawan
}

public enum Kategori : byte
{
    [Description("")] None,
    [Description("Chicken")] Chicken,
    [Description("Paket Nasi")] PaketNasi,
    [Description("Spaghetti")] Spaghetti,
    [Description("Burger")] Burger,
    [Description("DBROsis")] DBROsis,
    [Description("Sayhot")] Sayhot,
    [Description("Tteokbokki")] Tteokbokki,
    [Description("D'Prek")] DPrek,
    [Description("Minuman")] Minuman
}