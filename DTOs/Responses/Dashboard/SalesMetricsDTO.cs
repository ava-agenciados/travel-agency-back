namespace travel_agency_back.DTOs.Responses.Dashboard
{
    public class SalesMetricsDTO
    {
        public List<DestinationSalesDTO> SalesByDestination { get; set; } = new();
        public List<PeriodSalesDTO> SalesByPeriod { get; set; } = new();
        public List<ClientSalesDTO> SalesByClient { get; set; } = new();
        public List<StatusSalesDTO> SalesByStatus { get; set; } = new();
        public decimal AnnualRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<PaymentMethodUsageDTO> MostUsedPaymentMethods { get; set; } = new();
        public List<PaymentMethodUsageDTO> MostUsedPaymentMethodsByMonth { get; set; } = new();
        public List<PaymentMethodUsageDTO> MostUsedPaymentMethodsByDestination { get; set; } = new();
        public List<DestinationRevenueDTO> RevenueByDestination { get; set; } = new();
    }

    public class DestinationSalesDTO
    {
        public string Destination { get; set; } = string.Empty;
        public int TotalSales { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PeriodSalesDTO
    {
        public string Period { get; set; } = string.Empty; // Ex: "2024-06" ou "2024"
        public int TotalSales { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ClientSalesDTO
    {
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public int TotalSales { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class StatusSalesDTO
    {
        public string Status { get; set; } = string.Empty;
        public int TotalSales { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PaymentMethodUsageDTO
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DestinationRevenueDTO
    {
        public string Destination { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}