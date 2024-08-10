using Microsoft.AspNetCore.Mvc;
using OnTapCSharp.Data;
using OnTapCSharp.ViewModel;

namespace OnTapCSharp.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;
        public MenuLoaiViewComponent(Hshop2023Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                soluong = lo.HangHoas.Count()
            }).OrderBy(p => p.MaLoai);
            return View(data); //Default.cshtml
            //return View("Default", data)
        }
    }
}
