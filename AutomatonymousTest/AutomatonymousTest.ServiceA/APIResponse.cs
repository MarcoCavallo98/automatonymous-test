namespace AutomatonymousTest.ServiceA
{
    public class APIResponse<T>
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public T Body { get; set; }
    }
}
