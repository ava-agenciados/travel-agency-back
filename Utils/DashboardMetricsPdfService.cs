using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using travel_agency_back.DTOs.Responses.Dashboard;

namespace travel_agency_back.Utils
{
    public class DashboardMetricsPdfService
    {
        public static byte[] GenerateMetricsReport(SalesMetricsDTO metrics)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(c => ComposeContent(c, metrics));
                    page.Footer().AlignCenter().Text("© 2024 New Horizons Agência de Viagens. Todos os direitos reservados.").FontSize(10).FontColor("#666");
                });
            });
            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Column(col =>
                {
                    col.Item().Text("NEW HORIZONS").FontSize(22).Bold().FontColor("#2563eb");
                    col.Item().Text("AGÊNCIA DE VIAGENS").FontSize(10).FontColor("#2563eb");
                });
            });
        }

        private static void ComposeContent(IContainer container, SalesMetricsDTO metrics)
        {
            container.Column(col =>
            {
                col.Item().PaddingBottom(10).Text("Relatório de Métricas de Vendas").FontSize(18).Bold().FontColor("#2563eb");
                col.Item().PaddingBottom(10).Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor("#888");

                col.Item().PaddingBottom(2).Text("Receita Anual: R$ " + metrics.AnnualRevenue.ToString("N2", new CultureInfo("pt-BR"))).FontSize(14).Bold().FontColor("#28a745");
                col.Item().PaddingBottom(10).Text("Receita Mensal: R$ " + metrics.MonthlyRevenue.ToString("N2", new CultureInfo("pt-BR"))).FontSize(14).Bold().FontColor("#1976d2");

                // Vendas por Destino
                col.Item().Text("Vendas por Destino").FontSize(14).Bold().FontColor("#2563eb");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(100);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("Destino").Bold();
                        header.Cell().Text("Vendas").Bold();
                        header.Cell().Text("Total (R$)").Bold();
                    });
                    foreach (var d in metrics.SalesByDestination ?? new List<DestinationSalesDTO>())
                    {
                        table.Cell().Text(d.Destination);
                        table.Cell().Text(d.TotalSales.ToString());
                        table.Cell().Text(d.TotalAmount.ToString("N2", new CultureInfo("pt-BR")));
                    }
                });
                col.Item().PaddingVertical(10);

                // Vendas por Período
                col.Item().Text("Vendas por Período").FontSize(14).Bold().FontColor("#2563eb");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(100);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("Período").Bold();
                        header.Cell().Text("Vendas").Bold();
                        header.Cell().Text("Total (R$)").Bold();
                    });
                    foreach (var p in metrics.SalesByPeriod ?? new List<PeriodSalesDTO>())
                    {
                        table.Cell().Text(p.Period);
                        table.Cell().Text(p.TotalSales.ToString());
                        table.Cell().Text(p.TotalAmount.ToString("N2", new CultureInfo("pt-BR")));
                    }
                });
                col.Item().PaddingVertical(10);

                // Vendas por Cliente
                col.Item().Text("Vendas por Cliente").FontSize(14).Bold().FontColor("#2563eb");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(100);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("Cliente").Bold();
                        header.Cell().Text("Email").Bold();
                        header.Cell().Text("Vendas").Bold();
                        header.Cell().Text("Total (R$)").Bold();
                    });
                    foreach (var c in metrics.SalesByClient ?? new List<ClientSalesDTO>())
                    {
                        table.Cell().Text(c.ClientName);
                        table.Cell().Text(c.ClientEmail);
                        table.Cell().Text(c.TotalSales.ToString());
                        table.Cell().Text(c.TotalAmount.ToString("N2", new CultureInfo("pt-BR")));
                    }
                });
                col.Item().PaddingVertical(10);

                // Vendas por Status
                col.Item().Text("Vendas por Status").FontSize(14).Bold().FontColor("#2563eb");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(100);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("Status").Bold();
                        header.Cell().Text("Vendas").Bold();
                        header.Cell().Text("Total (R$)").Bold();
                    });
                    foreach (var s in metrics.SalesByStatus ?? new List<StatusSalesDTO>())
                    {
                        table.Cell().Text(s.Status);
                        table.Cell().Text(s.TotalSales.ToString());
                        table.Cell().Text(s.TotalAmount.ToString("N2", new CultureInfo("pt-BR")));
                    }
                });
                col.Item().PaddingVertical(10);

                // Métodos de Pagamento Mais Usados
                col.Item().Text("Métodos de Pagamento Mais Usados").FontSize(14).Bold().FontColor("#2563eb");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(100);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("Método").Bold();
                        header.Cell().Text("Usos").Bold();
                        header.Cell().Text("Total (R$)").Bold();
                    });
                    foreach (var m in metrics.MostUsedPaymentMethods ?? new List<PaymentMethodUsageDTO>())
                    {
                        table.Cell().Text(m.PaymentMethod);
                        table.Cell().Text(m.UsageCount.ToString());
                        table.Cell().Text(m.TotalAmount.ToString("N2", new CultureInfo("pt-BR")));
                    }
                });
                col.Item().PaddingVertical(10);

                // Receita por Destino
                col.Item().Text("Receita por Destino").FontSize(14).Bold().FontColor("#2563eb");
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("Destino").Bold();
                        header.Cell().Text("Receita (R$)").Bold();
                    });
                    foreach (var r in metrics.RevenueByDestination ?? new List<DestinationRevenueDTO>())
                    {
                        table.Cell().Text(r.Destination);
                        table.Cell().Text(r.Revenue.ToString("N2", new CultureInfo("pt-BR")));
                    }
                });
            });
        }
    }
}
