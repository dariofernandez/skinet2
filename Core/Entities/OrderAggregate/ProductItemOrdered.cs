namespace Core.Entities.OrderAggregate
{
    public class ProductItemOrdered
    {

        // empty constructor needed for EF migrations
        public ProductItemOrdered()
        {
        }

        // Note: ProductItemOrdered class
        //   is a snapshot of the order
        public ProductItemOrdered(int productItemId, string productName, string pictureUrl)
        {
            ProductItemId = productItemId;
            ProductName = productName;
            PictureUrl = pictureUrl;
        }

        public int ProductItemId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
    }
}