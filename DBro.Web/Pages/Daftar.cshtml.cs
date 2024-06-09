using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static DBro.Shared.Extensions.EncryptionHelper;

namespace DBro.Web.Pages;

public class DaftarModel(IUserService UserService) : PageModel
{
    [BindProperty] public UserModel Form { get; set; } = new();

    [TempData] public string Notif { get; set; } = string.Empty;

    public class UserModel
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public JenisKelamin JenisKelamin { get; set; }
        public DateTime? TanggalLahir { get; set; } = DateTime.Today;
        public string Alamat { get; set; } = string.Empty;
        public string? Telepon { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var result = await UserService.AddAsync(new User
            {
                Email = Form.Email,
                Password = Encrypt(Form.Password),
                Nama = Form.Nama,
                JenisKelamin = Form.JenisKelamin,
                TanggalLahir = Form.TanggalLahir,
                Alamat = Form.Alamat,
                Telepon = Form.Telepon,
                JenisUser = JenisUser.Pelanggan
            });

            if (result.Item1 != null)
            {
                Notif = "Registration successful! Please log in.";
                return RedirectToPage("/Login");
            }
        }
        return Page();
    }
}


//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using ProduksiManufaktur.Web.Pages.Account.Validator;
//using System.ComponentModel.DataAnnotations;

//namespace ProduksiManufaktur.Web.Pages.Account
//{
//    public class RegistrasiModel : PageModel
//    {
//        private readonly UserManager<User> _userManager;
//        private readonly SignInManager<User> _signInManager;
//        private readonly IUserService _userService;
//        private readonly IAccountService _accountService;

//        [BindProperty]
//        public InputModel Input { get; set; } = new();

//        public RegistrasiModel(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService, IAccountService accountService)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _userService = userService;
//            _accountService = accountService;
//        }

//        public class InputModel
//        {
//            [DataType(DataType.EmailAddress)]
//            [Required(ErrorMessage = "Id wajib diisi")]
//            public string Email { get; set; } = string.Empty;

//            [DataType(DataType.Password)]
//            [Required(ErrorMessage = "Password wajib diisi")]
//            [RequireDigit(ErrorMessage = "Password harus mengandung angka dan satu huruf kapital")]
//            [MinLength(6, ErrorMessage = "Panjang password minimal 6 karakter")]
//            public string Password { get; set; } = string.Empty;

//            [DataType(DataType.Password)]
//            [Display(Name = "Konfirmasi password")]
//            [Compare("Password", ErrorMessage = "Konfirmasi password harus sama dengan password")]
//            public string KonfirmasiPassword { get; set; } = string.Empty;

//            [Required(ErrorMessage = "Alamat wajib diisi")]
//            public string Alamat { get; set; } = string.Empty;

//            [Required(ErrorMessage = "Nomor Telepon wajib diisi")]
//            public string PhoneNumber { get; set; } = string.Empty;

//            [Display(Name = "Tempat Lahir")]
//            [Required(ErrorMessage = "Tempat Lahir wajib diisi")]
//            public string TempatLahir { get; set; } = string.Empty;

//            [DataType(DataType.Date)]
//            [Display(Name = "Tanggal Lahir")]
//            public DateTime TanggalLahir { get; set; } = DateTime.Now.Date;
//        }

//        public void OnGet()
//        {
//        }
//    }
//}
