using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrdersApp.Data;

namespace OrdersApp.Models;

public class FormEntryRepository : IFormEntryRepository
{
    private readonly FormsDbContext _db;

    public FormEntryRepository(FormsDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<FormEntry>> GetListAsync(
    string formType,
    int page,
    int pageSize,
    string? search = null,
    string? orderStatus = null,
    string? fulfillmentPriority = null,
    bool? requiresSignature = null,
    DateTime? orderDateFrom = null,
    DateTime? orderDateTo = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _db.Entries
            .Where(e => e.FormType == formType);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(e => e.Data.ToLower().Contains(term));
        }

        
        var list = await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        
        if (formType.Equals("orders", StringComparison.OrdinalIgnoreCase) &&
            (orderStatus != null ||
             fulfillmentPriority != null ||
             requiresSignature != null ||
             orderDateFrom != null ||
             orderDateTo != null))
        {
            list = list.Where(entry =>
            {
                try
                {
                    using var doc = JsonDocument.Parse(entry.Data);
                    var root = doc.RootElement;

                    bool ok = true;

                    if (!string.IsNullOrEmpty(orderStatus) &&
                        root.TryGetProperty("OrderStatus", out var osProp))
                    {
                        ok &= string.Equals(
                            osProp.GetString(),
                            orderStatus,
                            StringComparison.OrdinalIgnoreCase);
                    }
                    else if (!string.IsNullOrEmpty(orderStatus))
                    {
                        ok = false;
                    }

                    if (!string.IsNullOrEmpty(fulfillmentPriority) &&
                        root.TryGetProperty("FulfillmentPriority", out var fpProp))
                    {
                        ok &= string.Equals(
                            fpProp.GetString(),
                            fulfillmentPriority,
                            StringComparison.OrdinalIgnoreCase);
                    }
                    else if (!string.IsNullOrEmpty(fulfillmentPriority))
                    {
                        ok = false;
                    }

                    if (requiresSignature.HasValue &&
                        root.TryGetProperty("RequiresSignature", out var rsProp))
                    {
                        bool rs = rsProp.ValueKind switch
                        {
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.String => bool.TryParse(rsProp.GetString(), out var b) && b,
                            _ => false
                        };
                        ok &= rs == requiresSignature.Value;
                    }
                    else if (requiresSignature.HasValue)
                    {
                        ok = false;
                    }

                    if ((orderDateFrom != null || orderDateTo != null) &&
                        root.TryGetProperty("OrderDate", out var odProp))
                    {
                        if (DateTime.TryParse(odProp.GetString(), out var od))
                        {
                            if (orderDateFrom != null)
                                ok &= od >= orderDateFrom.Value;
                            if (orderDateTo != null)
                                ok &= od <= orderDateTo.Value;
                        }
                        else
                        {
                            ok = false;
                        }
                    }
                    else if (orderDateFrom != null || orderDateTo != null)
                    {
                        ok = false;
                    }

                    return ok;
                }
                catch
                {
                    return false;
                }
            }).ToList();
        }

        var totalCount = list.Count;

        var pageItems = list
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<FormEntry>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = pageItems
        };
    }

    public async Task<FormEntry?> GetOneAsync(string formType, Guid id)
    {
        return await _db.Entries
            .FirstOrDefaultAsync(e => e.FormType == formType && e.Id == id);
    }

    public async Task<FormEntry> CreateAsync(string formType, JsonElement data)
    {
        var json = data.GetRawText();
        var entry = new FormEntry
        {
            Id = Guid.NewGuid(),
            FormType = formType,
            Data = json,
            CreatedAt = DateTime.UtcNow
        };

        _db.Entries.Add(entry);
        await _db.SaveChangesAsync();

        return entry;
    }

    public async Task<FormEntry?> UpdateAsync(string formType, Guid id, JsonElement data)
    {
        var json = data.GetRawText();
        var entry = await _db.Entries
            .FirstOrDefaultAsync(e => e.FormType == formType && e.Id == id);

        if (entry == null)
            return null;

        entry.Data = json;
        entry.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return entry;
    }

    public async Task<bool> DeleteAsync(string formType, Guid id)
    {
        var entry = await _db.Entries
            .FirstOrDefaultAsync(e => e.FormType == formType && e.Id == id);

        if (entry == null)
            return false;

        _db.Entries.Remove(entry);
        await _db.SaveChangesAsync();

        return true;
    }
}
