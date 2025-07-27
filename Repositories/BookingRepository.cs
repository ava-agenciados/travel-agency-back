using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.DTOs.Responses.Dashboard;

namespace travel_agency_back.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private ApplicationDBContext _context;
        public BookingRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Implements the required interface method
        public async Task<IActionResult> CreateUserBookingAsync(int userId, int packageId, Booking booking)
        {
            if (booking == null)
                return new BadRequestObjectResult("Booking inválido.");

            booking.UserId = userId;
            booking.PackageId = packageId;
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return new OkObjectResult(booking);
        }

        public Task DeleteUserBookingAsync(int userId, int bookingId)
        {
          var booking = _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == bookingId);
            if (booking == null)
            {
                return Task.FromResult<IActionResult>(new NotFoundObjectResult("Reserva não encontrada."));
            }
             _context.Bookings.Remove(booking.Result);
            return _context.SaveChangesAsync();
        }

        public async Task<Booking> GetUserBookingByIdAsync(int userId, int bookingId)
        {
            var reservation = await _context.Bookings
                .Where(b => b.UserId == userId && b.Id == bookingId)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync();
            return reservation;
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .Include(b => b.Package)
                .ThenInclude(p => p.LodgingInfo)
                .Include(b => b.User) // Adiciona o include do usuário
                .ToListAsync();
            return bookings;
        }

        public Task UpdateUserBookingAsync(int userId, Booking booking)
        {
           var result = _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == booking.Id);
            if (result == null)
            {
                return Task.FromResult<IActionResult>(new NotFoundObjectResult("Reserva não encontrada."));
            }
            booking.UpdatedAt = DateTime.UtcNow;
            _context.Entry(result.Result).CurrentValues.SetValues(booking);
            return _context.SaveChangesAsync();
        }

        // Implements the required interface method
        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            return booking;
        }

        public async Task<List<Booking>> GetBookingsByPackageIdAsync(int packageId)
        {
            var result = await _context.Bookings
                .Where(b => b.PackageId == packageId)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync();
            return result;
        }

        public async Task<List<Booking>> GetActiveBookingsAsync()
        {
            var result = await _context.Bookings
                .Where(b => b.Status == "Ativo")
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync(); 
            return result;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            var result = await _context.Bookings
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .Include(b => b.Package)
                .ThenInclude(p => p.LodgingInfo)
                .Include(b => b.User)
                .ToListAsync();
            return result;
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            var result = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync();
            return result;
        }

        public Task<List<Booking>> GetBookingsByStatusAsync(string status)
        {
            var result = _context.Bookings
                .Where(b => b.Status == status)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync();
            return result;
        }

        public async Task<List<Booking>> GetBookingsByTravelDateAsync(DateTime travelDate)
        {
            var result = await _context.Bookings
                .Where(b => b.TravelDate.Date == travelDate.Date)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync();
            return result;
        }

        public async Task<List<Booking>> GetBookingsByBookingDateAsync(DateTime bookingDate)
        {
            var result = await _context.Bookings
                .Where(b => b.BookingDate.Date == bookingDate.Date)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync();
            return result;
        }

        public async Task<SalesMetricsDTO> GetSalesMetricsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var baseQuery = _context.Bookings
                .Include(b => b.Package)
                .Include(b => b.User)
                .Include(b => b.Payments);

            IQueryable<Booking> bookingsQuery = baseQuery;

            if (startDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= startDate.Value);
            if (endDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= endDate.Value);

            var bookings = await bookingsQuery.ToListAsync();

            // Vendas por destino
            var salesByDestination = bookings
                .GroupBy(b => b.Package.Destination)
                .Select(g => new DestinationSalesDTO
                {
                    Destination = g.Key,
                    TotalSales = g.Count(),
                    TotalAmount = g.Sum(b => b.Payments.Sum(p => p.Amount))
                }).ToList();

            // Vendas por período (mensal)
            var salesByPeriod = bookings
                .GroupBy(b => b.BookingDate.ToString("yyyy-MM"))
                .Select(g => new PeriodSalesDTO
                {
                    Period = g.Key,
                    TotalSales = g.Count(),
                    TotalAmount = g.Sum(b => b.Payments.Sum(p => p.Amount))
                }).ToList();

            // Vendas por cliente
            var salesByClient = bookings
                .GroupBy(b => new { b.User.Id, b.User.FirstName, b.User.LastName, b.User.Email })
                .Select(g => new ClientSalesDTO
                {
                    ClientName = $"{g.Key.FirstName} {g.Key.LastName}",
                    ClientEmail = g.Key.Email,
                    TotalSales = g.Count(),
                    TotalAmount = g.Sum(b => b.Payments.Sum(p => p.Amount))
                }).ToList();

            // Vendas por status
            var salesByStatus = bookings
                .GroupBy(b => b.Status)
                .Select(g => new StatusSalesDTO
                {
                    Status = g.Key,
                    TotalSales = g.Count(),
                    TotalAmount = g.Sum(b => b.Payments.Sum(p => p.Amount))
                }).ToList();

            // Receita anual (Aprovado)
            var currentYear = DateTime.UtcNow.Year;
            var annualRevenue = bookings
                .Where(b => b.BookingDate.Year == currentYear && b.Status != null && b.Status.Equals("Aprovado", StringComparison.OrdinalIgnoreCase))
                .Sum(b => b.Payments.Sum(p => p.Amount));

            // Receita mensal (Aprovado)
            var currentMonth = DateTime.UtcNow.Month;
            var monthlyRevenue = bookings
                .Where(b => b.BookingDate.Year == currentYear && b.BookingDate.Month == currentMonth && b.Status != null && b.Status.Equals("Aprovado", StringComparison.OrdinalIgnoreCase))
                .Sum(b => b.Payments.Sum(p => p.Amount));

            // Receita por destino (Aprovado)
            var revenueByDestination = bookings
                .Where(b => !string.IsNullOrEmpty(b.Package.Destination) && 
                            b.Status != null && 
                            b.Status.Equals("Aprovado", StringComparison.OrdinalIgnoreCase))
                .GroupBy(b => b.Package.Destination)
                .Select(g => new DestinationRevenueDTO
                {
                    Destination = g.Key,
                    Revenue = g.Sum(b => b.Payments.Sum(p => p.Amount))
                }).ToList();

            // Métodos de pagamento mais usados (geral)
            var mostUsedPaymentMethods = bookings
                .SelectMany(b => b.Payments)
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new PaymentMethodUsageDTO
                {
                    PaymentMethod = g.Key,
                    UsageCount = g.Count(),
                    TotalAmount = g.Sum(p => p.Amount)
                }).OrderByDescending(x => x.UsageCount).ToList();

            // Métodos de pagamento mais usados no mês (Aprovado)
            var mostUsedPaymentMethodsByMonth = bookings
                .Where(b => b.BookingDate.Year == currentYear && b.BookingDate.Month == currentMonth && b.Status != null && b.Status.Equals("Aprovado", StringComparison.OrdinalIgnoreCase))
                .SelectMany(b => b.Payments)
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new PaymentMethodUsageDTO
                {
                    PaymentMethod = g.Key,
                    UsageCount = g.Count(),
                    TotalAmount = g.Sum(p => p.Amount)
                }).OrderByDescending(x => x.UsageCount).ToList();

            // Métodos de pagamento mais usados por destino (Aprovado)
            var mostUsedPaymentMethodsByDestination = bookings
                .Where(b => !string.IsNullOrEmpty(b.Package.Destination) && 
                            b.Status != null && 
                            b.Status.Equals("Aprovado", StringComparison.OrdinalIgnoreCase))
                .GroupBy(b => b.Package.Destination)
                .SelectMany(g => g.SelectMany(b => b.Payments)
                    .GroupBy(p => p.PaymentMethod)
                    .Select(pg => new PaymentMethodUsageDTO
                    {
                        PaymentMethod = pg.Key + " (" + g.Key + ")",
                        UsageCount = pg.Count(),
                        TotalAmount = pg.Sum(p => p.Amount)
                    })
                ).OrderByDescending(x => x.UsageCount).ToList();

            return new SalesMetricsDTO
            {
                SalesByDestination = salesByDestination,
                SalesByPeriod = salesByPeriod,
                SalesByClient = salesByClient,
                SalesByStatus = salesByStatus,
                AnnualRevenue = annualRevenue,
                MonthlyRevenue = monthlyRevenue,
                RevenueByDestination = revenueByDestination,
                MostUsedPaymentMethods = mostUsedPaymentMethods,
                MostUsedPaymentMethodsByMonth = mostUsedPaymentMethodsByMonth,
                MostUsedPaymentMethodsByDestination = mostUsedPaymentMethodsByDestination
            };
        }
    }
}
