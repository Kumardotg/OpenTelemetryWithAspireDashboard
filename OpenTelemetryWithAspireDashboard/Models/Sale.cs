using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OpenTelemetryWithAspireDashboard
{
    public class Sale
    {
        public int Id { get; set; }
        public CoffeeType CoffeeType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}