using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using System.Net.Mail;
using System.Net;
using ShopApp.DTO;
using Microsoft.AspNetCore.Authorization;
using ShopApp.Enums;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context) {_context = context; }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> FindAll()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult> FingById(int id)
        {
            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .Include(x => x.User)
                .Include(x => x.CouponOrders)
                .ThenInclude(co => co.Coupon)
                .ThenInclude(c => c.CouponConditions)
                .FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                OrderFullName = order.OrderFullName,
                OrderAddress = order.OrderAddress,
                OrderPhoneNumber = order.OrderPhoneNumber,
                OrderEmail = order.OrderEmail,
                OrderDate = order.OrderDate,
                OrderPaymentMethods = order.OrderPaymentMethods,
                OrderStatusPayment = order.OrderStatusPayment,
                OrderStatus = order.OrderStatus,    
                OrderQuantity = order.OrderQuantity,
                OrderAmount = order.OrderAmount,
                OrderNote = order.OrderNote,
                User = new UserDTO
                {
                    Id = order.User.Id,
                    UserName = order.User.UserName,
                    UserFullName = order.User.UserName,
                    UserAvatar = order.User.UserAvatar,
                    UserEmail = order.User.UserEmail,
                    UserPhoneNumber = order.User.UserPhoneNumber,
                    UserAddress = order.User.UserAddress,
                    UserGender = order.User.UserGender,
                },
                Coupons = order.CouponOrders.Select(co => new CouponDTO
                {
                    CouponId = co.Coupon.CouponId,
                    Percent = co.Coupon.Percent,
                    Code = co.Coupon.Code,
                    Active = co.Coupon.Active,
                    Description = co.Coupon.Description,
                    CouponConditions = co.Coupon.CouponConditions.Select(cc => new CouponConditionDTO
                    {
                        CouponConditionId = cc.CouponConditionId,
                        Attribute = cc.Attribute,
                        Operator = cc.Operator,
                        Value = cc.Value,
                        DiscountAmount = cc.DiscountAmount,
                    }).ToList()
                }).ToList(),
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailId = od.OrderId,
                    ProductName = od.Product.ProductName,
                    ProductSlug = od.Product.ProductSlug,
                    ProductImage = od.Product.ProductImage,
                    ProductPrice = od.Product.ProductPrice,
                    ProductSalePrice = od.Product.ProductSalePrice,
                    CategoryName = od.Product.Category.CategoryName,
                    ProductStatus = od.Product.ProductStatus,
                    Quantity = od.Quantity,
                    TotalMoney = od.TotalMoney,

                }).ToList()

            };
            return Ok(new ResponseObject(200, "Query data successfully", orderDTO));
        }

        [Authorize(Roles = "User")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> FingByUserId(int userId)
        {
            var orders = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .Include(x => x.User)
                .Include(x => x.CouponOrders)
                .ThenInclude(co => co.Coupon)
                .ThenInclude(c => c.CouponConditions)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            var orderDTOs = orders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                OrderFullName = order.OrderFullName,
                OrderAddress = order.OrderAddress,
                OrderPhoneNumber = order.OrderPhoneNumber,
                OrderEmail = order.OrderEmail,
                OrderDate = order.OrderDate,
                OrderPaymentMethods = order.OrderPaymentMethods,
                OrderStatusPayment = order.OrderStatusPayment,
                OrderStatus = order.OrderStatus,
                OrderQuantity = order.OrderQuantity,
                OrderAmount = order.OrderAmount,
                OrderNote = order.OrderNote,
                User = new UserDTO
                {
                    Id = order.User.Id,
                    UserName = order.User.UserName,
                    UserFullName = order.User.UserName,
                    UserAvatar = order.User.UserAvatar,
                    UserEmail = order.User.UserEmail,
                    UserPhoneNumber = order.User.UserPhoneNumber,
                    UserAddress = order.User.UserAddress,
                    UserGender = order.User.UserGender,
                },
                Coupons = order.CouponOrders.Select(co => new CouponDTO
                {
                    CouponId = co.Coupon.CouponId,
                    Percent = co.Coupon.Percent,
                    Code = co.Coupon.Code,
                    Active = co.Coupon.Active,
                    Description = co.Coupon.Description,
                    CouponConditions = co.Coupon.CouponConditions.Select(cc => new CouponConditionDTO
                    {
                        CouponConditionId = cc.CouponConditionId,
                        Attribute = cc.Attribute,
                        Operator = cc.Operator,
                        Value = cc.Value,
                        DiscountAmount = cc.DiscountAmount,
                    }).ToList()
                }).ToList(),
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailId = od.OrderId,
                    ProductName = od.Product.ProductName,
                    ProductSlug = od.Product.ProductSlug,
                    ProductImage = od.Product.ProductImage,
                    ProductPrice = od.Product.ProductPrice,
                    ProductSalePrice = od.Product.ProductSalePrice,
                    CategoryName = od.Product.Category.CategoryName,
                    ProductStatus = od.Product.ProductStatus,
                    Quantity = od.Quantity,
                    TotalMoney = od.TotalMoney,

                }).ToList()

            });
            return Ok(new ResponseObject(200, "Query data successfully", orderDTOs));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> SaveOrder(OrderModel model)
        {
            try
            {
                List<Cart> carts = await _context.Carts.Include(x => x.Product).Where(x => x.UserId == model.UserId).ToListAsync();
                if (carts.Count > 0) {
                    TotalDTO total = new TotalDTO();
                    foreach (var item in carts)
                    {
                        total.Quantity += item.Quantity;
                        total.Amount += (double)item.TotalAmount;
                    }
                    Order order = new Order
                    {
                        OrderFullName = model.OrderFullName,
                        OrderAddress = model.OrderAddress,
                        OrderAmount = total.Amount,
                        OrderQuantity = (int)total.Quantity,
                        OrderEmail = model.OrderEmail,
                        OrderDate = DateTime.Now,
                        OrderStatus = (int) OrderStatus.CHUAN_BI_DON_HANG,
                        OrderNote = model.OrderNote,
                        OrderPhoneNumber = model.OrderPhoneNumber,
                        OrderStatusPayment = model.OrderStatusPayment,
                        OrderPaymentMethods = model.OrderPaymentMethods,
                        UserId = model.UserId,
                    };
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // get current orderId
                    var orderId = order.OrderId;
                    foreach (var item in carts)
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            OrderId = orderId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            TotalMoney = Convert.ToDouble(item.TotalAmount)
                        };
                        await _context.OrderDetails.AddAsync(orderDetail);
                    }
                    // send email confirm order success
                    EmailModel emailModel = new EmailModel()
                    {
                        Subject = "Order Confirmation",
                        To = order.OrderEmail
                    };

                    using (MailMessage mm = new MailMessage(emailModel.From, emailModel.To))
                    {
                        mm.Subject = emailModel.Subject;
                        mm.Body = BodyOrderSuccessMail(order, carts, total.Amount);
                        mm.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential NetworkCred = new NetworkCredential(emailModel.From, emailModel.Password);
                            smtp.UseDefaultCredentials = false;
                            smtp.EnableSsl = true;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }
                    }
                    // remove cart when order success
                    _context.Carts.RemoveRange(carts);
                    await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Order successfully,  please check email!"));
                }
                else
                {
                    return BadRequest(new ResponseObject(400, "Cart is empty"));
                }
                
            }
            catch (Exception ex) {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}/{status}")]
        public async Task<ActionResult<Order>> UpdateStatus(int id, int status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                try
                {
                    order.OrderStatus = status;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update status successfully", order));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
                }
            }
            else
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Ok(new ResponseObject(200, "Delete data successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        public static string BodyOrderSuccessMail(Order order, List<Cart> carts, double totalAmount)
        {
            string body = string.Empty;
            string unit = "";

            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Utils\\Mail", "OrderSuccessMail.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            foreach (var item in carts)
            {
                unit += "<tr>";
                unit += "<td width='75%' align='left' style='font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>";
                unit += WebUtility.HtmlEncode(item.Product?.ProductName ?? "Unknown Product") + " (" + item.Quantity + ")</td>";
                unit += "<td width='25%' align='left' style='font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px;'>";
                var productPrice = item.Product?.ProductSalePrice > 0 ? item.Product.ProductSalePrice : item.Product?.ProductPrice ?? 0;
                unit += string.Format("{0:#,0} VND", productPrice) + "</td>";
                unit += "</tr>";
            }

            // Thay thế các placeholder trong mẫu email với giá trị thực tế
            body = body.Replace("{{CreatedAt}}", order.OrderDate.ToString());
            body = body.Replace("{{FullName}}", WebUtility.HtmlEncode(order.OrderFullName));
            body = body.Replace("{{Phone}}", WebUtility.HtmlEncode(order.OrderPhoneNumber));
            body = body.Replace("{{Address}}", WebUtility.HtmlEncode(order.OrderAddress));
            body = body.Replace("{{OrderId}}", order.OrderId.ToString());
            body = body.Replace("{{Unit}}", unit);
            body = body.Replace("{{ToTalPrice}}", string.Format("{0:#,0} VND", totalAmount));

            return body;
        }
    }
}
