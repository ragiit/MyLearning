using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetDiscount called for ProductName: {ProductName}", request.ProductName);

            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon == null)
            {
                logger.LogWarning("No discount found for ProductName: {ProductName}. Returning default coupon.", request.ProductName);

                coupon = new Models.Coupon
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Descripcion = "-"
                };
            }
            else
            {
                logger.LogInformation("Discount found for ProductName: {ProductName} - Amount: {Amount}", coupon.ProductName, coupon.Amount);
            }

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation("CreateDiscount called for ProductName: {ProductName}", request.Coupon.ProductName);

            var coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid Request Object"));
            dbContext.Coupons.Add(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount successfully created for ProductName: {ProductName}", coupon.ProductName);

            return coupon.Adapt<CouponModel>();
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation("UpdateDiscount called for ProductName: {ProductName}", request.Coupon.ProductName);

            var existingCoupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.Id == request.Coupon.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, $"Coupon with Id={request.Coupon.Id} not found."));

            // Update the fields
            existingCoupon.ProductName = request.Coupon.ProductName;
            existingCoupon.Descripcion = request.Coupon.Description;
            existingCoupon.Amount = request.Coupon.Amount;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount updated for ProductName: {ProductName}", existingCoupon.ProductName);

            return existingCoupon.Adapt<CouponModel>();
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation("DeleteDiscount called for ProductName: {ProductName}", request.ProductName);

            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
            if (coupon == null)
            {
                logger.LogWarning("Discount not found for ProductName: {ProductName}", request.ProductName);
                return new DeleteDiscountResponse { Success = false };
            }

            dbContext.Coupons.Remove(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount deleted for ProductName: {ProductName}", request.ProductName);

            return new DeleteDiscountResponse { Success = true };
        }
    }
}