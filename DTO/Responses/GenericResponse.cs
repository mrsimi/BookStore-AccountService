namespace Contoso_BookStore_AccountService.DTO.Responses
{
    public class GenericResponse <T>
    {
        public T Data {get; set;}
        public int StatusCode {get; set;}
        public string ResponseMessage {get; set;}
    }
}