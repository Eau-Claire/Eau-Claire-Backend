using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Fishbreed
{
    public int BreedId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<Tank> Tanks { get; set; } = new List<Tank>();
}
