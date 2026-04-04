namespace ECommerce.Domain.Entities.BasketModule
{
    public class BasketItem
    {
        public int ProductId { get; set; }        // 🔑 ربط مباشر بالـ Product من الداتابيز
        public string ProductName { get; set; } = default!;  // نسخة الاسم
        public string PictureUrl { get; set; } = default!;   // نسخة الصورة
        public decimal Price { get; set; }        // نسخة السعر
        public int Quantity { get; set; }         // عدد القطع
    }
}
