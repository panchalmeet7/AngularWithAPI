using System;
using System.Collections.Generic;

namespace AngularBackend.Entities.Models;

public partial class ImageComp
{
    public int ImageId { get; set; }

    public string ImagePath { get; set; } = null!;
}
