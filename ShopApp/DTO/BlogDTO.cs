namespace ShopApp.DTO
{
    public class BlogDTO
    {
        public int BlogId { get; set; }

        public string? BlogImage { get; set; }

        public string BlogTitle { get; set; }

        public string? BlogSlug { get; set; }

        public string? BlogDescription { get; set; }

        public DateTime? CreateDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; } = DateTime.Now;
        public List<BlogCommentDTO> BlogComments { get; set; } = new List<BlogCommentDTO>();

    }
}
