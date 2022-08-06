namespace PatikaHomework4.Dto.Response
{
    public class GenericResponse<T> where T : class
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
    }
}
