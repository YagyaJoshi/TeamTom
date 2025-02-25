namespace TommAPI.Provider
{
    public class CustomError
    {
        public string Error { get; }
        public int StatusCode { get; }
        public CustomError(string message, int Code)
        {
            Error = message;
            StatusCode = Code;
        }
    }
}