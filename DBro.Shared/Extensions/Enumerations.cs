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
    Karyawan,
    Pelanggan
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

public enum JenisVarian : byte
{
    [Description("-")] None,
    [Description("Tingkat Kepedasan")] TingkatKepedasan,
    [Description("Rasa")] Rasa
}

public enum StatusPesanan : byte
{
    [Description("-")] None,
    [Description("Belum Checkout")] BelumCheckout,
    [Description("Menunggu Persetujuan")] MenungguPersetujuan,
    [Description("Diterima")] Diterima,
    [Description("Ditolak")] Ditolak
}

public enum JenisAktivitas : byte
{
    Tambah,
    Edit,
    Hapus
}

/// <summary>
/// Aktivitas
/// </summary>
public enum Entitas : byte
{
    User,
    Menu,
    Diskon,
    Promo,
    [Description("Diskon & Promo")] Sales,
    Pesanan
}