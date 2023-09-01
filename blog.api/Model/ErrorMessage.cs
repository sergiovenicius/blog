namespace blog.common.Model
{
    public class ErrorMessage
    {

        public ErrorMessage(string message)
        {
            Error = message;
        }
        public string Error { get; set; }
    }
}
