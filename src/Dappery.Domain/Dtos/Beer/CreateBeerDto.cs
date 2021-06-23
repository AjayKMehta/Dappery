namespace Dappery.Domain.Dtos
{
    public class CreateBeerDto
    {
        public string? Name { get; set; }

        public string? Style { get; set; }

        public int BreweryId { get; set; }
    }
}
