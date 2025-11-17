using System;
using System.Text.Json;

namespace OrdersApp.Models;

public interface IFormEntryRepository
    {
        Task<PagedResult<FormEntry>> GetListAsync(
        string formType,
        int page,
        int pageSize,
        string? search = null,
        string? orderStatus = null,
        string? fulfillmentPriority = null,
        bool? requiresSignature = null,
        DateTime? orderDateFrom = null,
        DateTime? orderDateTo = null);
        Task<FormEntry?> GetOneAsync(string formType, Guid id);
        Task<FormEntry> CreateAsync(string formType, JsonElement data);
        Task<FormEntry?> UpdateAsync(string formType, Guid id, JsonElement data);
        Task<bool> DeleteAsync(string formType, Guid id);
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
    }
