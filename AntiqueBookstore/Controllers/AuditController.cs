using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace AntiqueBookstore.Controllers
{
    [Authorize(Roles = "Manager")] // work in progress
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const int PageSize = 10; // Pagination, Number of records per page/table


        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Audit
        public async Task<IActionResult> Index(string? selectedLogin/*, int page = 1*/)
        {
            // Base query excludes tracking 
            var auditLogsQuery = _context.SalesAuditLogs
                .AsNoTracking();

            // Limit query scope
            auditLogsQuery = auditLogsQuery
                .Where(log =>
                    log.Login != null &&
                    log.Login != "System" &&
                    !log.TableName.StartsWith("AspNet")
                );
                // filters
                // log => log.Login != null
                // log.Login != "System"
                // !log.TableName.StartsWith("AspNet")

            // Apply User login filter 
            if (!string.IsNullOrEmpty(selectedLogin))
            {
                auditLogsQuery = auditLogsQuery
                    .Where(log => log.Login == selectedLogin);
                    // filters 
                    //log => log.Login == selectedLogin
            }

            // Order the results (newest first) before pagination
            auditLogsQuery = auditLogsQuery
                .OrderByDescending(log => log.Timestamp);

            // Get the paginated list of logs entires for the current page using PagedList.Core [PagedList.Core.Mvc]
            //var pagedLogs = await auditLogsQuery.ToPagedListAsync(page, PageSize); 
            // ISSUE: does not have ToPagedListAsync() method ?? <PackageReference Include="PagedList.Core.Mvc" Version="3.0.0" />
            //var pagedLogs = PagedList.Core.PagedListExtensions.ToPagedList(auditLogsQuery, page, PageSize);
            
            // ISSUE: temporary exclude pagination query and materialize to List
            List<SalesAuditLog> auditLogsListNoPage = await auditLogsQuery.ToListAsync();

            // Get ALL logins PRESENT IN THE LOG TABLE for the filter dropdown
            var distinctLogins = await _context.SalesAuditLogs
                                        .Select(log => log.Login)
                                        .Where(login => login != null && login != "System") // Exclude System/null logins
                                        .Distinct()
                                        .OrderBy(login => login)
                                        .ToListAsync();

            // Create the ViewModel
            var viewModel = new AuditLogViewModel
            {
                // ISSUE: temporary exclude pagination
                //AuditLogs = pagedLogs,
                AuditLogs = auditLogsListNoPage,
                
                // SelectList for the dropdown, marking the current selection
                LoginList = new SelectList(distinctLogins, selectedLogin),

                SelectedLogin = selectedLogin // Pass the filter back to the View
            };

            return View(viewModel);
        }

        // GET: /Audit/ExportAuditLogCsv
        public async Task<IActionResult> ExportAuditLogCsv()
        {
            // Base query
            var auditLogsQuery = _context.SalesAuditLogs.AsNoTracking();

            // Order the results for consistency in the export file
            var logsToExport = await auditLogsQuery
                                     .OrderByDescending(log => log.Timestamp)
                                     .ToListAsync();

            // Generate CSV using CsvHelper
            using (var memoryStream = new MemoryStream())

            // Use UTF8 with BOM for better Excel compatibility with non-ASCII characters
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))

            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Write the header row explicitly if needed if CsvHelper does do it automatically
                // csv.WriteHeader<SalesAuditLog>();
                // csv.NextRecord();

                // Write the log records
                await csv.WriteRecordsAsync(logsToExport); 

                await writer.FlushAsync(); // Ensure all data is written to the stream
                memoryStream.Position = 0; // Reset stream position to the beginning

                // Return the file (FileContentResult)
                string fileName = $"SalesAuditLog_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
        }
    }
}
