using Microsoft.AspNetCore.Mvc;
using travel_agency_back.Third_party.Mail;
using travel_agency_back.Third_party.PaymentGateway;

namespace travel_agency_back.Controllers
{
    /// <summary>
    /// Controller para teste de envio de emails do sistema de pagamentos.
    /// 
    /// Este controller permite testar todos os tipos de emails que são enviados durante
    /// o processo de pagamento da agência de viagens, incluindo:
    /// - PIX (aprovado instantaneamente)
    /// - Boleto bancário (pendente)
    /// - Cartão de crédito/débito (recusado para demonstração)
    /// - Reset de senha
    /// 
    /// IMPORTANTE: Este é um controller para TESTES e DEMONSTRAÇÃO apenas.
    /// Em produção, este controller deve ser removido ou protegido adequadamente.
    /// </summary>
    [Route("api/v1/test")]
    [ApiController]
    public class EmailTestController : ControllerBase
    {
        private readonly PaymentGateway _paymentGateway;

        /// <summary>
        /// Construtor que injeta o PaymentGateway para simular pagamentos
        /// </summary>
        public EmailTestController()
        {
            // Cria uma nova instância do PaymentGateway para testes
            _paymentGateway = new PaymentGateway();
        }

        /// <summary>
        /// Endpoint para testar envio de email de PIX (sempre aprovado)
        /// 
        /// Este endpoint simula um pagamento via PIX que é processado instantaneamente
        /// e sempre retorna como aprovado, enviando um email de confirmação.
        /// 
        /// POST /api/v1/test/email-pix
        /// </summary>
        [HttpPost("email-pix")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult TestPixEmail([FromBody] PixPaymentRequest request)
        {
            try
            {
                // VALIDAÇÕES OBRIGATÓRIAS PARA PIX
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email é obrigatório");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome é obrigatório");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome é obrigatório");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte é obrigatório");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote é obrigatório");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // PROCESSA PAGAMENTO PIX (não precisa de dados de cartão)
                _paymentGateway.DemonstrateSpecificPaymentType(
                    PaymentMethod.Pix,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.CPFPassport,
                    request.Destination,
                    request.Origin,
                    request.PackageName,
                    request.StartDate,
                    request.EndDate
                );

                return Ok(new { 
                    success = true, 
                    message = "PIX processado! Pagamento APROVADO instantaneamente",
                    paymentMethod = "PIX",
                    status = "Aprovado",
                    pixInfo = new {
                        message = "QR Code PIX gerado automaticamente",
                        expiration = "15 minutos",
                        processing = "Instantâneo"
                    },
                    packageData = new {
                        packageName = request.PackageName,
                        destination = request.Destination,
                        origin = request.Origin,
                        startDate = request.StartDate.ToString("dd/MM/yyyy"),
                        endDate = request.EndDate.ToString("dd/MM/yyyy"),
                        amount = request.Amount
                    },
                    emailSent = request.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro ao processar PIX: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Endpoint para testar envio de email de Boleto (sempre pendente)
        /// 
        /// Este endpoint simula um pagamento via Boleto bancário que fica
        /// com status pendente aguardando compensação.
        /// 
        /// POST /api/v1/test/email-boleto
        /// </summary>
        [HttpPost("email-boleto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult TestBoletoEmail([FromBody] BoletoPaymentRequest request)
        {
            try
            {
                // VALIDAÇÕES OBRIGATÓRIAS PARA BOLETO
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email é obrigatório");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome é obrigatório");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome é obrigatório");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte é obrigatório");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote é obrigatório");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // PROCESSA BOLETO (não precisa de dados de cartão)
                _paymentGateway.DemonstrateSpecificPaymentType(
                    PaymentMethod.Boleto,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.CPFPassport,
                    request.Destination,
                    request.Origin,
                    request.PackageName,
                    request.StartDate,
                    request.EndDate
                );

                return Ok(new { 
                    success = true, 
                    message = "Boleto gerado! Status: PENDENTE aguardando pagamento",
                    paymentMethod = "Boleto",
                    status = "Pendente",
                    boletoInfo = new {
                        message = "Código de barras e linha digitável gerados automaticamente",
                        expiration = "3 dias",
                        processing = "Até 3 dias úteis para compensar"
                    },
                    packageData = new {
                        packageName = request.PackageName,
                        destination = request.Destination,
                        origin = request.Origin,
                        startDate = request.StartDate.ToString("dd/MM/yyyy"),
                        endDate = request.EndDate.ToString("dd/MM/yyyy"),
                        amount = request.Amount
                    },
                    emailSent = request.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro ao gerar boleto: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Endpoint para testar envio de email de Cartão de Crédito (sempre recusado)
        /// 
        /// POST /api/v1/test/email-cartao
        /// </summary>
        [HttpPost("email-cartao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult TestCartaoEmail([FromBody] CreditCardPaymentRequest request)
        {
            try
            {
                // VALIDAÇÕES OBRIGATÓRIAS PARA CARTÃO DE CRÉDITO
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email é obrigatório");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome é obrigatório");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome é obrigatório");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte é obrigatório");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote é obrigatório");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // VALIDAÇÕES ESPECÍFICAS DO CARTÃO DE CRÉDITO
                if (string.IsNullOrEmpty(request.CardNumber))
                    return BadRequest("Número do cartão é obrigatório");
                
                if (string.IsNullOrEmpty(request.CardHolderName))
                    return BadRequest("Nome do titular é obrigatório");
                
                if (string.IsNullOrEmpty(request.ExpiryDate))
                    return BadRequest("Data de validade é obrigatória");
                
                if (string.IsNullOrEmpty(request.CVV))
                    return BadRequest("CVV é obrigatório");
                
                if (string.IsNullOrEmpty(request.CardBrand))
                    return BadRequest("Bandeira do cartão é obrigatória");
                
                if (request.Installments < 1 || request.Installments > 24)
                    return BadRequest("Número de parcelas deve ser entre 1 e 24");

                // MONTA DADOS DO CARTÃO DE CRÉDITO
                var cardData = new PaymentGateway.CardData
                {
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName.ToUpper(),
                    ExpiryDate = request.ExpiryDate,
                    CVV = request.CVV,
                    Brand = request.CardBrand,
                    Installments = request.Installments
                };

                // PROCESSA PAGAMENTO DE CARTÃO DE CRÉDITO
                _paymentGateway.DemonstrateSpecificPaymentType(
                    PaymentMethod.CartaoCredito,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.CPFPassport,
                    request.Destination,
                    request.Origin,
                    request.PackageName,
                    request.StartDate,
                    request.EndDate,
                    cardData
                );

                return Ok(new { 
                    success = true, 
                    message = "Cartão de Crédito processado! Status: RECUSADO",
                    paymentMethod = "Cartão de Crédito",
                    status = "Recusado",
                    packageData = new {
                        packageName = request.PackageName,
                        destination = request.Destination,
                        origin = request.Origin,
                        startDate = request.StartDate.ToString("dd/MM/yyyy"),
                        endDate = request.EndDate.ToString("dd/MM/yyyy"),
                        amount = request.Amount
                    },
                    cardData = new {
                        cardNumber = $"**** **** **** {request.CardNumber.Substring(request.CardNumber.Length - 4)}",
                        cardHolder = cardData.CardHolderName,
                        expiryDate = cardData.ExpiryDate,
                        brand = cardData.Brand,
                        installments = cardData.Installments,
                        installmentAmount = Math.Round(request.Amount / request.Installments, 2)
                    },
                    emailSent = request.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro ao processar cartão de crédito: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Endpoint para testar envio de email de Cartão de Débito (sempre recusado)
        /// 
        /// POST /api/v1/test/email-debito
        /// </summary>
        [HttpPost("email-debito")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult TestDebitoEmail([FromBody] DebitCardPaymentRequest request)
        {
            try
            {
                // VALIDAÇÕES OBRIGATÓRIAS PARA CARTÃO DE DÉBITO
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email é obrigatório");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome é obrigatório");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome é obrigatório");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte é obrigatório");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote é obrigatório");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // VALIDAÇÕES ESPECÍFICAS DO CARTÃO DE DÉBITO
                if (string.IsNullOrEmpty(request.CardNumber))
                    return BadRequest("Número do cartão é obrigatório");
                
                if (string.IsNullOrEmpty(request.CardHolderName))
                    return BadRequest("Nome do titular é obrigatório");
                
                if (string.IsNullOrEmpty(request.ExpiryDate))
                    return BadRequest("Data de validade é obrigatória");
                
                if (string.IsNullOrEmpty(request.CVV))
                    return BadRequest("CVV é obrigatório");
                
                if (string.IsNullOrEmpty(request.CardBrand))
                    return BadRequest("Bandeira do cartão é obrigatória");

                // MONTA DADOS DO CARTÃO DE DÉBITO (sempre 1 parcela)
                var cardData = new PaymentGateway.CardData
                {
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName.ToUpper(),
                    ExpiryDate = request.ExpiryDate,
                    CVV = request.CVV,
                    Brand = request.CardBrand,
                    Installments = 1 // Débito sempre à vista
                };

                // PROCESSA PAGAMENTO DE CARTÃO DE DÉBITO
                _paymentGateway.DemonstrateSpecificPaymentType(
                    PaymentMethod.CartaoDebito,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.CPFPassport,
                    request.Destination,
                    request.Origin,
                    request.PackageName,
                    request.StartDate,
                    request.EndDate,
                    cardData
                );

                return Ok(new { 
                    success = true, 
                    message = "Cartão de Débito processado! Status: RECUSADO",
                    paymentMethod = "Cartão de Débito",
                    status = "Recusado",
                    packageData = new {
                        packageName = request.PackageName,
                        destination = request.Destination,
                        origin = request.Origin,
                        startDate = request.StartDate.ToString("dd/MM/yyyy"),
                        endDate = request.EndDate.ToString("dd/MM/yyyy"),
                        amount = request.Amount
                    },
                    cardData = new {
                        cardNumber = $"**** **** **** {request.CardNumber.Substring(request.CardNumber.Length - 4)}",
                        cardHolder = cardData.CardHolderName,
                        expiryDate = cardData.ExpiryDate,
                        brand = cardData.Brand,
                        installments = 1,
                        paymentType = "À vista"
                    },
                    emailSent = request.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro ao processar cartão de débito: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Endpoint para testar envio de email de reset de senha
        /// 
        /// POST /api/v1/test/email-reset-password
        /// </summary>
        [HttpPost("email-reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult TestResetPasswordEmail([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest("Email é obrigatório");
                }

                // Simula um link de reset de senha
                var resetToken = Guid.NewGuid().ToString();
                var resetLink = $"https://newhorizon.com/reset-password?token={resetToken}";
                var username = request.FirstName ?? "Usuário";

                // Chama diretamente o EmailService para enviar email de reset
                EmailService.SendPasswordResetEmail(request.Email, username, resetLink);

                return Ok(new { 
                    success = true, 
                    message = "Email de reset de senha enviado com sucesso!",
                    emailType = "Reset Password",
                    emailSent = request.Email,
                    resetLink = resetLink,
                    expiresIn = "24 horas"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro ao enviar email: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Endpoint para testar TODOS os tipos de email de uma vez
        /// 
        /// Este endpoint é útil para testar rapidamente todos os templates
        /// de email do sistema enviando para o mesmo endereço.
        /// 
        /// POST /api/v1/test/email-all-types
        /// </summary>
        [HttpPost("email-all-types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult TestAllEmailTypes([FromBody] EmailTestRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest("Email é obrigatório");
                }

                var results = new List<object>();

                // 1. Teste PIX (Aprovado)
                try
                {
                    _paymentGateway.DemonstrateSpecificPaymentType(
                        PaymentMethod.Pix,
                        request.Email,
                        "João", "Silva", "12345678901",
                        "Paris", "São Paulo", "Pacote Europa Clássica",
                        DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(37)
                    );
                    results.Add(new { tipo = "PIX", status = "Aprovado", enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "PIX", status = "Erro", erro = ex.Message });
                }

                // 2. Teste Boleto (Pendente)
                try
                {
                    _paymentGateway.DemonstrateSpecificPaymentType(
                        PaymentMethod.Boleto,
                        request.Email,
                        "Maria", "Santos", "98765432100",
                        "Londres", "Rio de Janeiro", "Pacote Reino Unido Premium",
                        DateTime.UtcNow.AddDays(45), DateTime.UtcNow.AddDays(52)
                    );
                    results.Add(new { tipo = "Boleto", status = "Pendente", enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "Boleto", status = "Erro", erro = ex.Message });
                }

                // 3. Teste Cartão de Crédito (Recusado) - COM DADOS DO CARTÃO
                try
                {
                    var creditCardData = new PaymentGateway.CardData
                    {
                        CardNumber = "4111111111111111",
                        CardHolderName = "CARLOS OLIVEIRA",
                        ExpiryDate = "12/28",
                        CVV = "123",
                        Brand = "Visa",
                        Installments = 6
                    };

                    _paymentGateway.DemonstrateSpecificPaymentType(
                        PaymentMethod.CartaoCredito,
                        request.Email,
                        "Carlos", "Oliveira", "11122233344",
                        "Nova York", "Brasília", "Pacote Estados Unidos Completo",
                        DateTime.UtcNow.AddDays(60), DateTime.UtcNow.AddDays(67),
                        creditCardData
                    );
                    results.Add(new { tipo = "Cartão de Crédito", status = "Recusado", parcelas = 6, enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "Cartão de Crédito", status = "Erro", erro = ex.Message });
                }

                // 4. Teste Cartão de Débito (Recusado) - COM DADOS DO CARTÃO
                try
                {
                    var debitCardData = new PaymentGateway.CardData
                    {
                        CardNumber = "5555555555554444",
                        CardHolderName = "ANA COSTA",
                        ExpiryDate = "03/27",
                        CVV = "456",
                        Brand = "Mastercard",
                        Installments = 1
                    };

                    _paymentGateway.DemonstrateSpecificPaymentType(
                        PaymentMethod.CartaoDebito,
                        request.Email,
                        "Ana", "Costa", "55566677788",
                        "Gramado", "Porto Alegre", "Pacote Serra Gaúcha Romântico",
                        DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(25),
                        debitCardData
                    );
                    results.Add(new { tipo = "Cartão de Débito", status = "Recusado", parcelas = 1, enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "Cartão de Débito", status = "Erro", erro = ex.Message });
                }

                // 5. Teste Reset de Senha
                try
                {
                    var resetToken = Guid.NewGuid().ToString();
                    var resetLink = $"https://newhorizon.com/reset-password?token={resetToken}";
                    EmailService.SendPasswordResetEmail(request.Email, "Usuário Teste", resetLink);
                    results.Add(new { tipo = "Reset Password", status = "Enviado", enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "Reset Password", status = "Erro", erro = ex.Message });
                }

                return Ok(new { 
                    success = true, 
                    message = "Teste completo de TODOS os tipos de email finalizado!",
                    emailSent = request.Email,
                    results = results,
                    totalTested = results.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro geral ao enviar emails: {ex.Message}" 
                });
            }
        }
    }

    /// <summary>
    /// DTO BASE para todos os tipos de pagamento
    /// Contém dados comuns: cliente e pacote
    /// </summary>
    public abstract class BasePaymentRequest
    {
        /// <summary>
        /// Email do cliente (obrigatório)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Primeiro nome do cliente (obrigatório)
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do cliente (obrigatório)
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// CPF ou Passaporte do cliente (obrigatório)
        /// </summary>
        public string CPFPassport { get; set; } = string.Empty;

        /// <summary>
        /// Nome do pacote de viagem escolhido (obrigatório)
        /// </summary>
        public string PackageName { get; set; } = string.Empty;

        /// <summary>
        /// Cidade/país de origem (obrigatório)
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Cidade/país de destino (obrigatório)
        /// </summary>
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Data de início da viagem (obrigatório)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data de fim da viagem (obrigatório)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Valor total do pacote (obrigatório)
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// DTO para pagamento via PIX
    /// PIX só precisa dos dados básicos - sem dados de cartão
    /// </summary>
    public class PixPaymentRequest : BasePaymentRequest
    {
        // PIX não precisa de campos adicionais
        // O QR Code/copia-e-cola é gerado automaticamente pelo sistema
    }

    /// <summary>
    /// DTO para pagamento via Boleto
    /// Boleto só precisa dos dados básicos - sem dados de cartão
    /// </summary>
    public class BoletoPaymentRequest : BasePaymentRequest
    {
        // Boleto não precisa de campos adicionais
        // O código de barras e linha digitável são gerados automaticamente
    }

    /// <summary>
    /// DTO para pagamento via Cartão de Crédito
    /// Inclui todos os campos do cartão + número de parcelas
    /// </summary>
    public class CreditCardPaymentRequest : BasePaymentRequest
    {
        /// <summary>
        /// Número do cartão de crédito (obrigatório)
        /// Exemplo: "4111111111111111"
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Nome do titular como no cartão (obrigatório)
        /// Exemplo: "JOAO SILVA"
        /// </summary>
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Data de validade MM/AA (obrigatório)
        /// Exemplo: "12/28"
        /// </summary>
        public string ExpiryDate { get; set; } = string.Empty;

        /// <summary>
        /// Código CVV do cartão (obrigatório)
        /// Exemplo: "123"
        /// </summary>
        public string CVV { get; set; } = string.Empty;

        /// <summary>
        /// Bandeira do cartão (obrigatório)
        /// Exemplo: "Visa", "Mastercard", "Elo"
        /// </summary>
        public string CardBrand { get; set; } = string.Empty;

        /// <summary>
        /// Número de parcelas (obrigatório)
        /// Mínimo: 1, Máximo: 24
        /// </summary>
        public int Installments { get; set; } = 1;
    }

    /// <summary>
    /// DTO para pagamento via Cartão de Débito
    /// Inclui dados do cartão mas SEM parcelas (sempre à vista)
    /// </summary>
    public class DebitCardPaymentRequest : BasePaymentRequest
    {
        /// <summary>
        /// Número do cartão de débito (obrigatório)
        /// Exemplo: "5555555555554444"
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Nome do titular como no cartão (obrigatório)
        /// Exemplo: "JOAO SILVA"
        /// </summary>
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Data de validade MM/AA (obrigatório)
        /// Exemplo: "03/27"
        /// </summary>
        public string ExpiryDate { get; set; } = string.Empty;

        /// <summary>
        /// Código CVV do cartão (obrigatório)
        /// Exemplo: "456"
        /// </summary>
        public string CVV { get; set; } = string.Empty;

        /// <summary>
        /// Bandeira do cartão (obrigatório)
        /// Exemplo: "Visa", "Mastercard", "Elo"
        /// </summary>
        public string CardBrand { get; set; } = string.Empty;
        
        // Nota: Débito não tem campo Installments pois é sempre à vista (1x)
    }

    /// <summary>
    /// DTO para reset de senha
    /// Só precisa do email
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Email do usuário (obrigatório)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário (opcional)
        /// </summary>
        public string? FirstName { get; set; }
    }

    // DTO legado mantido para compatibilidade com email-all-types
    public class EmailTestRequest : BasePaymentRequest
    {
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardBrand { get; set; }
        public int? Installments { get; set; }
    }
}