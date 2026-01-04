    namespace MealCraft.Utils;

    public enum ErrorType : ushort
    {
        None = 0,
        NoContent = 204,
        InvalidInput= 400,
        InvalidCredentials = 401,
        Conflict = 409,
        InternalError = 500
    }
    
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ErrorType ErrorType { get; set;} 
    }