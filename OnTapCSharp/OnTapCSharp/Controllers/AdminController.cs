using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OnTapCSharp.Data;
using OnTapCSharp.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnTapCSharp.Helpers;

namespace OnTapCSharp.Controllers
{
    public class AdminController : Controller
    {
        private readonly Hshop2023Context db;

        public AdminController(Hshop2023Context context) {
            db = context;
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginAdminVM model)
        {
            if (ModelState.IsValid)
            {
                var nhanVien = db.NhanViens.SingleOrDefault(p => p.Email == model.email);
                if (nhanVien == null) {
                    ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                }
                else
                {
                    if (nhanVien.MatKhau != model.password)
                    {
                        ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email,model.email),
                            new Claim(ClaimTypes.Name, nhanVien.HoTen),
                            new Claim("AdminID",nhanVien.MaNv),
                            new Claim(ClaimTypes.Role, "Admin")  // Thêm claim cho vai trò
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

                        return RedirectToAction("DanhSachKhachHang", "Admin");
                    }
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DanhSachKhachHang(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value; // kiểm tra pageNumber có null hoặc < 0 không nếu có thì pageNumber = 1 , ngước lại pageNumber = page
            var result = db.KhachHangs.Select(p => new KhachHangVM
            {
                MaKh = p.MaKh,
                HoTen = p.HoTen,
                DiaChi = p.DiaChi,
                DienThoai = p.DienThoai,
                Email = p.Email,
                HieuLuc = p.HieuLuc,
            });
            PagedList<KhachHangVM> lstKhachHang = new PagedList<KhachHangVM>(result, pageNumber, pageSize);
            return View(lstKhachHang);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult BlockKhachHang(string id) 
        {
            var data = db.KhachHangs.SingleOrDefault(p => p.MaKh == id);
            var result = new KhachHangVM
            {
                MaKh = data.MaKh,
                HoTen = data.HoTen,
                DiaChi = data.DiaChi,
                DienThoai = data.DienThoai,
                Email = data.Email,
                HieuLuc = data.HieuLuc
            };
            ViewData["HieuLuc"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "True", Value = "true" },
                new SelectListItem { Text = "False", Value = "false" }
            }, "Value", "Text");

            return View(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> BlockKhachHang(KhachHangVM model)
        {
            var khachHang = await db.KhachHangs.FindAsync(model.MaKh);
            ViewData["HieuLuc"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "True", Value = "true" },
                new SelectListItem { Text = "False", Value = "false" }
            }, "Value", "Text");

            if (khachHang != null)
            {
                khachHang.HoTen = model.HoTen;
                khachHang.DiaChi = model.DiaChi;
                khachHang.DienThoai = model.DienThoai;
                khachHang.Email = model.Email;
                khachHang.HieuLuc = model.HieuLuc;

                await db.SaveChangesAsync();
            }
            return RedirectToAction("DanhSachKhachHang");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DanhSachSanPham(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value; // kiểm tra pageNumber có null hoặc < 0 không nếu có thì pageNumber = 1 , ngước lại pageNumber = page
            var result = db.HangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                Hinh = p.Hinh,
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi,
                TenLoai = p.MaLoaiNavigation.TenLoai,
            });
            PagedList<HangHoaVM> lstHangHoa = new PagedList<HangHoaVM>(result, pageNumber, pageSize);
            return View(lstHangHoa);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UpdateSanPham(int id)
        {
            var data = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
            var loai = db.Loais.ToList();
            ViewData["LoaiSanPham"] = new SelectList(loai, "MaLoai", "TenLoai");
             
            var result = new HangHoaVM
            {
                MaHH = data.MaHh,
                TenHH = data.TenHh,
                Hinh = data.Hinh,
                DonGia = data.DonGia ?? 0,
                MoTaNgan = data.MoTaDonVi,
                TenLoai = data.MaLoaiNavigation.TenLoai
            };
            
            return View(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateSanPham(HangHoaVM model, IFormFile Hinh)
        {
            var hangHoa = await db.HangHoas.FindAsync(model.MaHH);
            var loai = db.Loais.ToList();
            ViewData["LoaiSanPham"] = new SelectList(loai, "MaLoai", "TenLoai");
             
             
            if (hangHoa != null)
            {
                hangHoa.TenHh = model.TenHH;
                 
                hangHoa.DonGia = model.DonGia;
                hangHoa.MoTaDonVi = model.MoTaNgan;
                hangHoa.MaLoaiNavigation.TenLoai = model.TenLoai;
                 

                if (Hinh != null && Hinh.Length > 0)
                {
                    // Upload file và lấy tên file
                    string fileName = RandomKey.UploadHinh(Hinh, "HangHoa");

                    // Cập nhật tên file vào thuộc tính Hinh của đối tượng hangHoa
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        hangHoa.Hinh = fileName;
                    }
                }
                else
                {
                    // Nếu không có file mới, giữ lại tên file cũ
                    hangHoa.Hinh = model.Hinh;
                }


                await db.SaveChangesAsync();
            }
            return RedirectToAction("DanhSachSanPham");
        }
        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("DangNhap");
        }
    }
}
