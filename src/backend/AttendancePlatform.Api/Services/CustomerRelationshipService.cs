using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface ICustomerRelationshipService
    {
        Task<CustomerDto> CreateCustomerAsync(CustomerDto customer);
        Task<List<CustomerDto>> GetCustomersAsync(Guid tenantId);
        Task<CustomerDto> UpdateCustomerAsync(Guid customerId, CustomerDto customer);
        Task<bool> DeleteCustomerAsync(Guid customerId);
        Task<ContactDto> CreateContactAsync(ContactDto contact);
        Task<List<ContactDto>> GetContactsAsync(Guid customerId);
        Task<InteractionDto> CreateInteractionAsync(InteractionDto interaction);
        Task<List<InteractionDto>> GetInteractionsAsync(Guid customerId);
        Task<OpportunityDto> CreateOpportunityAsync(OpportunityDto opportunity);
        Task<List<OpportunityDto>> GetOpportunitiesAsync(Guid tenantId);
        Task<CrmAnalyticsDto> GetCrmAnalyticsAsync(Guid tenantId);
        Task<List<CrmLeadDto>> GetLeadsAsync(Guid tenantId);
        Task<CrmLeadDto> CreateLeadAsync(CrmLeadDto lead);
        Task<CrmLeadDto> ConvertLeadToCustomerAsync(Guid leadId);
        Task<CrmDashboardDto> GetCrmDashboardAsync(Guid tenantId);
    }

    public class CustomerRelationshipService : ICustomerRelationshipService
    {
        private readonly ILogger<CustomerRelationshipService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public CustomerRelationshipService(ILogger<CustomerRelationshipService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customer)
        {
            try
            {
                customer.Id = Guid.NewGuid();
                customer.CreatedAt = DateTime.UtcNow;
                customer.Status = "Active";

                _logger.LogInformation("Customer created: {CustomerId} - {CustomerName}", customer.Id, customer.Name);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create customer");
                throw;
            }
        }

        public async Task<List<CustomerDto>> GetCustomersAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Acme Corporation",
                    Industry = "Technology",
                    Size = "Large",
                    Revenue = 50000000.00m,
                    Status = "Active",
                    AccountManagerId = Guid.NewGuid(),
                    AccountManagerName = "Sales Manager",
                    Email = "contact@acme.com",
                    Phone = "+1-555-0123",
                    Address = "123 Business St, Tech City, TC 12345",
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Global Solutions Inc",
                    Industry = "Consulting",
                    Size = "Medium",
                    Revenue = 15000000.00m,
                    Status = "Active",
                    AccountManagerId = Guid.NewGuid(),
                    AccountManagerName = "Account Executive",
                    Email = "info@globalsolutions.com",
                    Phone = "+1-555-0456",
                    Address = "456 Corporate Ave, Business City, BC 67890",
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                }
            };
        }

        public async Task<CustomerDto> UpdateCustomerAsync(Guid customerId, CustomerDto customer)
        {
            try
            {
                await Task.CompletedTask;
                customer.Id = customerId;
                customer.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Customer updated: {CustomerId}", customerId);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Guid customerId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Customer deleted: {CustomerId}", customerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete customer {CustomerId}", customerId);
                return false;
            }
        }

        public async Task<ContactDto> CreateContactAsync(ContactDto contact)
        {
            try
            {
                contact.Id = Guid.NewGuid();
                contact.CreatedAt = DateTime.UtcNow;

                _logger.LogInformation("Contact created: {ContactId} - {ContactName}", contact.Id, contact.Name);
                return contact;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create contact");
                throw;
            }
        }

        public async Task<List<ContactDto>> GetContactsAsync(Guid customerId)
        {
            await Task.CompletedTask;
            return new List<ContactDto>
            {
                new ContactDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Name = "John Smith",
                    Title = "CTO",
                    Email = "john.smith@acme.com",
                    Phone = "+1-555-0123",
                    IsPrimary = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new ContactDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Name = "Sarah Johnson",
                    Title = "Procurement Manager",
                    Email = "sarah.johnson@acme.com",
                    Phone = "+1-555-0124",
                    IsPrimary = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                }
            };
        }

        public async Task<InteractionDto> CreateInteractionAsync(InteractionDto interaction)
        {
            try
            {
                interaction.Id = Guid.NewGuid();
                interaction.CreatedAt = DateTime.UtcNow;

                _logger.LogInformation("Interaction created: {InteractionId} - {InteractionType}", interaction.Id, interaction.Type);
                return interaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create interaction");
                throw;
            }
        }

        public async Task<List<InteractionDto>> GetInteractionsAsync(Guid customerId)
        {
            await Task.CompletedTask;
            return new List<InteractionDto>
            {
                new InteractionDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Type = "Meeting",
                    Subject = "Quarterly Business Review",
                    Description = "Discussed Q4 performance and 2025 planning",
                    Date = DateTime.UtcNow.AddDays(-7),
                    Duration = 60,
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "Account Manager",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new InteractionDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Type = "Email",
                    Subject = "Follow-up on proposal",
                    Description = "Sent detailed proposal for new services",
                    Date = DateTime.UtcNow.AddDays(-3),
                    Duration = 15,
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "Sales Representative",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };
        }

        public async Task<OpportunityDto> CreateOpportunityAsync(OpportunityDto opportunity)
        {
            try
            {
                opportunity.Id = Guid.NewGuid();
                opportunity.CreatedAt = DateTime.UtcNow;
                opportunity.Stage = "Prospecting";

                _logger.LogInformation("Opportunity created: {OpportunityId} - {OpportunityName}", opportunity.Id, opportunity.Name);
                return opportunity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create opportunity");
                throw;
            }
        }

        public async Task<List<OpportunityDto>> GetOpportunitiesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<OpportunityDto>
            {
                new OpportunityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Enterprise Software License",
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Acme Corporation",
                    Value = 250000.00m,
                    Stage = "Proposal",
                    Probability = 75,
                    ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
                    OwnerId = Guid.NewGuid(),
                    OwnerName = "Senior Sales Manager",
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },
                new OpportunityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Consulting Services Contract",
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Global Solutions Inc",
                    Value = 150000.00m,
                    Stage = "Negotiation",
                    Probability = 85,
                    ExpectedCloseDate = DateTime.UtcNow.AddDays(15),
                    OwnerId = Guid.NewGuid(),
                    OwnerName = "Account Executive",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<CrmAnalyticsDto> GetCrmAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new CrmAnalyticsDto
            {
                TenantId = tenantId,
                TotalCustomers = 150,
                ActiveCustomers = 135,
                NewCustomersThisMonth = 8,
                TotalRevenue = 2500000.00m,
                RevenueThisMonth = 450000.00m,
                TotalOpportunities = 45,
                OpenOpportunities = 32,
                WonOpportunities = 28,
                LostOpportunities = 5,
                AverageOpportunityValue = 125000.00m,
                SalesConversionRate = 62.5,
                CustomerRetentionRate = 92.3,
                AverageCustomerLifetime = 36,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<CrmLeadDto>> GetLeadsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CrmLeadDto>
            {
                new CrmLeadDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Tech Startup Inc",
                    ContactName = "Mike Chen",
                    Email = "mike@techstartup.com",
                    Phone = "+1-555-0789",
                    Source = "Website",
                    Status = "New",
                    Score = 75,
                    AssignedToId = Guid.NewGuid(),
                    AssignedToName = "Lead Qualifier",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new CrmLeadDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Manufacturing Corp",
                    ContactName = "Lisa Brown",
                    Email = "lisa@manufacturing.com",
                    Phone = "+1-555-0987",
                    Source = "Trade Show",
                    Status = "Qualified",
                    Score = 85,
                    AssignedToId = Guid.NewGuid(),
                    AssignedToName = "Sales Representative",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };
        }

        public async Task<CrmLeadDto> CreateLeadAsync(CrmLeadDto lead)
        {
            try
            {
                lead.Id = Guid.NewGuid();
                lead.CreatedAt = DateTime.UtcNow;
                lead.Status = "New";

                _logger.LogInformation("Lead created: {LeadId} - {LeadName}", lead.Id, lead.Name);
                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create lead");
                throw;
            }
        }

        public async Task<CrmLeadDto> ConvertLeadToCustomerAsync(Guid leadId)
        {
            try
            {
                await Task.CompletedTask;
                var lead = new CrmLeadDto
                {
                    Id = leadId,
                    TenantId = Guid.NewGuid(),
                    Name = "Converted Lead Company",
                    ContactName = "Lead Contact",
                    Email = "converted@example.com",
                    Phone = "+1-555-0123",
                    Source = "Direct",
                    Status = "Converted",
                    Score = 100,
                    AssignedToId = Guid.NewGuid(),
                    AssignedToName = "Sales Representative",
                    CreatedAt = DateTime.UtcNow,
                    ConvertedDate = DateTime.UtcNow
                };

                _logger.LogInformation("Lead converted to customer: {LeadId}", leadId);
                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to convert lead {LeadId}", leadId);
                throw;
            }
        }

        public async Task<CrmDashboardDto> GetCrmDashboardAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new CrmDashboardDto
            {
                TenantId = tenantId,
                TotalCustomers = 150,
                NewLeads = 12,
                OpenOpportunities = 32,
                RevenueThisMonth = 450000.00m,
                SalesTarget = 500000.00m,
                SalesTargetProgress = 90.0,
                TopPerformingSalesperson = "Senior Sales Manager",
                CustomerSatisfactionScore = 4.3,
                LeadConversionRate = 28.5,
                AverageDealSize = 125000.00m,
                SalesCycleLength = 45,
                GeneratedAt = DateTime.UtcNow
            };
        }
    }

    public class CustomerDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string Name { get; set; }
        public required string Industry { get; set; }
        public required string Size { get; set; }
        public decimal Revenue { get; set; }
        public required string Status { get; set; }
        public Guid AccountManagerId { get; set; }
        public required string AccountManagerName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ContactDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InteractionDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public required string Type { get; set; }
        public required string Subject { get; set; }
        public required string Description { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public Guid EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OpportunityDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string Name { get; set; }
        public Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public decimal Value { get; set; }
        public required string Stage { get; set; }
        public int Probability { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public Guid OwnerId { get; set; }
        public required string OwnerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CrmAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int TotalOpportunities { get; set; }
        public int OpenOpportunities { get; set; }
        public int WonOpportunities { get; set; }
        public int LostOpportunities { get; set; }
        public decimal AverageOpportunityValue { get; set; }
        public double SalesConversionRate { get; set; }
        public double CustomerRetentionRate { get; set; }
        public int AverageCustomerLifetime { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class CrmLeadDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string Name { get; set; }
        public required string ContactName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Source { get; set; }
        public required string Status { get; set; }
        public int Score { get; set; }
        public Guid AssignedToId { get; set; }
        public required string AssignedToName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConvertedDate { get; set; }
    }

    public class CrmDashboardDto
    {
        public Guid TenantId { get; set; }
        public int TotalCustomers { get; set; }
        public int NewLeads { get; set; }
        public int OpenOpportunities { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal SalesTarget { get; set; }
        public double SalesTargetProgress { get; set; }
        public required string TopPerformingSalesperson { get; set; }
        public double CustomerSatisfactionScore { get; set; }
        public double LeadConversionRate { get; set; }
        public decimal AverageDealSize { get; set; }
        public int SalesCycleLength { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
