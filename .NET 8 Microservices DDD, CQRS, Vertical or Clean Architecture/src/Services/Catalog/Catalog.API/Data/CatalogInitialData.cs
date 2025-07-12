namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = await store.LightweightSerializableSessionAsync();

            if (await session.Query<Product>().AnyAsync(cancellation))
            {
                return; // Data already exists, no need to populate
            }
            var products = new List<Product>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Wireless Mouse",
                    Category = new() { "Electronics", "Accessories" },
                    Description = "Ergonomic wireless mouse with long battery life.",
                    ImageFile = "mouse.jpg",
                    Price = 199000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Mechanical Keyboard",
                    Category = new() { "Electronics", "Accessories" },
                    Description = "RGB backlit mechanical keyboard with blue switches.",
                    ImageFile = "keyboard.jpg",
                    Price = 750000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Gaming Headset",
                    Category = new() { "Electronics", "Audio" },
                    Description = "Surround sound gaming headset with noise-canceling mic.",
                    ImageFile = "headset.jpg",
                    Price = 499000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "USB-C Hub",
                    Category = new() { "Electronics", "Adapters" },
                    Description = "7-in-1 USB-C hub with HDMI, USB 3.0 and card reader.",
                    ImageFile = "usbc-hub.jpg",
                    Price = 299000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "4K Monitor",
                    Category = new() { "Electronics", "Display" },
                    Description = "27 inch 4K UHD monitor with HDR and slim bezels.",
                    ImageFile = "monitor.jpg",
                    Price = 3299000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Office Chair",
                    Category = new() { "Furniture", "Office" },
                    Description = "Ergonomic office chair with lumbar support.",
                    ImageFile = "chair.jpg",
                    Price = 850000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Smartphone Stand",
                    Category = new() { "Accessories", "Mobile" },
                    Description = "Adjustable aluminum smartphone stand.",
                    ImageFile = "phone-stand.jpg",
                    Price = 75000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Portable SSD 1TB",
                    Category = new() { "Storage", "Electronics" },
                    Description = "High-speed 1TB portable SSD with USB-C.",
                    ImageFile = "ssd.jpg",
                    Price = 1399000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "LED Desk Lamp",
                    Category = new() { "Lighting", "Home" },
                    Description = "Touch control LED lamp with adjustable brightness.",
                    ImageFile = "lamp.jpg",
                    Price = 159000
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Bluetooth Speaker",
                    Category = new() { "Audio", "Electronics" },
                    Description = "Portable waterproof Bluetooth speaker.",
                    ImageFile = "speaker.jpg",
                    Price = 299000
                }
            };

            session.Store<Product>(products);
            await session.SaveChangesAsync(cancellation);
        }
    }
}