namespace SistemaPermisos.Models
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ServiceResult SuccessResult(string message = "Operación exitosa.", object? data = null)
        {
            return new ServiceResult { Success = true, Message = message, Data = data };
        }

        public static ServiceResult ErrorResult(string message = "Ocurrió un error.", object? data = null)
        {
            return new ServiceResult { Success = false, Message = message, Data = data };
        }
    }
}
