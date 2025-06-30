namespace MyBlazor.Utility
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public static string Role_Customer = "Customer";
        public static string Role_User = "User";

        public static string Status_Pending = "Pending";
        public static string Status_ReadyForPickup = "ReadyForPickup";
        public static string Status_Completed = "Completed";
        public static string Status_Cancelled = "Cancelled";

        public static List<OrderDetail> ConvertShoppingCartToOrderDetails(List<ShoppingCart> shoppingCarts)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var cart in shoppingCarts)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    Product = cart.Product,
                    ProductId = cart.ProductId,
                    Count = cart.Quantity,
                    Price = Convert.ToDouble(cart.Product.Price),
                    ProductName = cart.Product.Name, // Assuming Product has a Name property
                };
                orderDetails.Add(orderDetail);
            }
            return orderDetails;
        }
    }
}