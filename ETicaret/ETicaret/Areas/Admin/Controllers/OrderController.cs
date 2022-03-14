using ETicaret.Data;
using ETicaret.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Diger.Role_Admin)]
    public class OrderController : Controller
    {        
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpPost]
        [Authorize(Roles =Diger.Role_Admin)]
        public IActionResult Onaylandi()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i=>i.id==OrderVM.orderHeader.id);
            orderHeader.OrderStatus = Diger.DurumOnaylandi;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = Diger.Role_Admin)]
        public IActionResult KargoyaVer()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.id == OrderVM.orderHeader.id);
            orderHeader.OrderStatus = Diger.Kargoda;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM
            {
                orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.id == id),
                OrderDetails = _db.OrderDetails.Where(x => x.OrderId == id).Include(x => x.Product)
            };
            return View(OrderVM);
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.ToList();
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
        public IActionResult Beklenen()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.OrderStatus == Diger.Beklemede);
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value&i.OrderStatus==Diger.Beklemede).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
        public IActionResult Onaylanan()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.OrderStatus == Diger.DurumOnaylandi);
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value & i.OrderStatus == Diger.DurumOnaylandi).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
        public IActionResult Kargolanan()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.OrderStatus == Diger.Kargoda);
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value & i.OrderStatus == Diger.Kargoda).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
    }
}
