namespace ShopApp.Utils
{
    public class ResponseObject
    {
        public int code { get; set; }
        public string message { get; set; }
        public object? data { get; set; }

        public ResponseObject(int code, string message)
        {
            this.code = code;
            this.message = message;
        }

        public ResponseObject(int code, string message, object? data)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }
    }
}
