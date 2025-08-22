using System;
using System.Collections.Generic;

namespace AzurePJ.Models;

public partial class ToDo
{
    public Guid Id { get; set; }

    public string? ToDo1 { get; set; }

    public DateTime DueDate { get; set; }
}
