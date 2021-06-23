namespace Dappery.Domain.Dtos
{
    public class UpdateBeerDto
    {
        public string? Name { get; set; }

        public string? Style { get; set; }

        public int? BreweryId { get; set; }
    }
}
