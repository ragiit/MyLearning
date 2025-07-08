namespace Apple.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController(AppDbContext db, IProductService productService, ICouponService couponService, IConfiguration configuration, IMessageBus messageBus) : ControllerBase
    {
        private readonly ResponseDto _response = new();

        // Asumsikan Anda sudah inject IProductService di constructor controller ini.
        // private readonly IProductService _productService;

        // Asumsikan Anda sudah inject IProductService di constructor controller ini.
        // private readonly IProductService _productService;

        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                // 1. Ambil data keranjang dari database
                //var cartHeaderFromDb = await db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                //// Jika tidak ada keranjang untuk user ini, kembalikan respons kosong yang sukses.
                //if (cartHeaderFromDb == null)
                //{
                //    _response.Message = "No cart found for this user.";
                //    return _response;
                //}
                //// 2. Siapkan DTO utama
                //CartDto cart = new()
                //{
                //    CartHeader = cartHeaderFromDb.Adapt<CartHeaderDto>(),
                //    CartDetails = await db.CartDetails
                //        .Where(u => u.CartHeaderId == cartHeaderFromDb.Id)
                //        .ToListAsync().Adapt<List<CartDetailDto>>()
                //};
                //// 3. Panggil ProductAPI untuk mendapatkan detail semua produk.
                //var productList = await productService.GetAllProductsAsync();
                //// 4. Gabungkan data produk dan hitung total harga
                //foreach (var item in cart.CartDetails)
                //{
                //    item.Product = productList.FirstOrDefault(p => p.Id == item.ProductId);
                //    cart.CartHeader.CartTotal += (item.Count * (item.Product?.Price ?? 0));
                //}
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return _response; // Langsung kembalikan respons error
            }
            return _response;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                // 1. Ambil data dasar dari database
                var cartHeaderFromDb = await db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);

                // Jika tidak ada keranjang untuk user ini, kembalikan respons kosong yang sukses.
                if (cartHeaderFromDb == null)
                {
                    _response.Message = "No cart found for this user.";
                    return _response;
                }

                var cartDetailsFromDb = await db.CartDetails
                    .Where(u => u.CartHeaderId == cartHeaderFromDb.Id)
                    .ToListAsync();

                // 2. Siapkan DTO utama
                CartDto cart = new()
                {
                    CartHeader = cartHeaderFromDb.Adapt<CartHeaderDto>(),
                    // --- INI BAGIAN YANG DIPERBAIKI ---
                    // Paksa adaptasi menjadi List<T> untuk menghindari deferred execution.
                    CartDetails = cartDetailsFromDb.Adapt<List<CartDetailDto>>()
                };

                // 3. Panggil ProductAPI untuk mendapatkan detail semua produk.
                var productList = await productService.GetAllProductsAsync();

                // 4. Gabungkan data produk dan hitung total harga
                foreach (var item in cart.CartDetails)
                {
                    // Cari produk yang cocok dari daftar produk yang sudah diambil
                    item.Product = productList.FirstOrDefault(p => p.Id == item.ProductId);

                    // Hitung subtotal untuk item ini dan tambahkan ke CartTotal di header utama
                    cart.CartHeader.CartTotal += (item.Count * (item.Product?.Price ?? 0));
                }

                // TODO: Terapkan logika diskon kupon di sini jika ada
                // if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode)) { ... }

                if (!string.IsNullOrWhiteSpace(cart.CartHeader.CouponCode))
                {
                    var coupon = await couponService.GetCouponByCodeAsync(cart.CartHeader.CouponCode);
                    if (coupon != null)
                    {
                        cart.CartHeader.Discount = coupon.Discount;
                        cart.CartHeader.CartTotal -= coupon.Discount;
                    }
                    else
                    {
                        cart.CartHeader.CouponCode = ""; // Hapus kode kupon jika tidak valid
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return _response; // Langsung kembalikan respons error
            }
            return _response;
        }

        [HttpPost("Upsert")]
        public async Task<ResponseDto> Upsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    // Jika header keranjang tidak ada, buat baru.
                    var cartHeader = cartDto.CartHeader.Adapt<CartHeader>();
                    db.CartHeaders.Add(cartHeader);
                    await db.SaveChangesAsync();

                    // Set header id untuk detail
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.Id;
                    db.CartDetails.Add(cartDto.CartDetails.First().Adapt<CartDetail>());
                    await db.SaveChangesAsync();
                }
                else
                {
                    // Jika header keranjang sudah ada, cek detailnya.
                    var cartDetailsFromDb = await db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.CartHeaderId == cartHeaderFromDb.Id &&
                             u.ProductId == cartDto.CartDetails.First().ProductId);

                    if (cartDetailsFromDb == null)
                    {
                        // Jika detail item belum ada, tambahkan sebagai item baru.
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.Id;
                        db.CartDetails.Add(cartDto.CartDetails.First().Adapt<CartDetail>());
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        // Jika detail item sudah ada, perbarui jumlahnya.
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().Id = cartDetailsFromDb.Id;
                        db.CartDetails.Update(cartDto.CartDetails.First().Adapt<CartDetail>());
                        await db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return _response;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await db.CartHeaders
                    .FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartHeaderFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                db.CartHeaders.Update(cartHeaderFromDb);
                await db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await db.CartHeaders
                    .FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartHeaderFromDb.CouponCode = ""; // Hapus kode kupon
                db.CartHeaders.Update(cartHeaderFromDb);
                await db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailId)
        {
            try
            {
                var cartDetail = await db.CartDetails.FirstAsync(u => u.Id == cartDetailId);
                int totalCountofCartItem = db.CartDetails.Count(u => u.CartHeaderId == cartDetail.CartHeaderId);

                db.CartDetails.Remove(cartDetail);

                // Jika ini adalah item terakhir, hapus juga headernya.
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await db.CartHeaders.FirstAsync(u => u.Id == cartDetail.CartHeaderId);
                    db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return _response;
            }
            return _response;
        }
    }
}