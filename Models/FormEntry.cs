using System;
using System.Text.Json;

namespace OrdersApp.Models;

public class FormEntry
{
    public Guid Id { get; set; }

        // Logical type/category of the form, e.g. "orders", "contact", etc.
        public string FormType { get; set; } = default!;

        // Arbitrary JSON payload
        public string Data { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
}
