namespace Dappery.Domain.Dtos
{
    public class ErrorDto
    {
        public ErrorDto(string? description, object? details = null)
        {
            this.Description = string.IsNullOrWhiteSpace(description) ? "An expected error has occured." : description;
            this.Details = details;
        }

        public string Description { get; }

        public object? Details { get; set; }
    }
}
