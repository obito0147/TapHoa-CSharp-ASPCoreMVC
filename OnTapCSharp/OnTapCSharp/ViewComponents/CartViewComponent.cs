using Microsoft.AspNetCore.Mvc;
using OnTapCSharp.Helpers;
using OnTapCSharp.ViewModel;

namespace OnTapCSharp.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();
            return View("CartPanel",new CartModel
            {
                Quantity = cart.Sum(p => p.SoLuong),
                ThanhTien = cart.Sum(p => p.ThanhTien)
            }); 
        }
    }
}
