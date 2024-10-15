namespace ShopApp.DTO
{
    public class BlogCommentDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
