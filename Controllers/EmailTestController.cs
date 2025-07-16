using Microsoft.AspNetCore.Mvc;
using travel_agency_back.Third_party.Mail;
using travel_agency_back.Third_party.PaymentGateway;

namespace travel_agency_back.Controllers
{
    /// <summary>
    /// Controller para teste de envio de emails do sistema de pagamentos.
    /// 
    /// Este controller permite testar todos os tipos de emails que s�o enviados durante
    /// o processo de pagamento da ag�ncia de viagens, incluindo:
    /// - PIX (aprovado instantaneamente)
    /// - Boleto banc�rio (pendente)
    /// - Cart�o de cr�dito/d�bito (recusado para demonstra��o)
    /// - Reset de senha
    /// 
    /// IMPORTANTE: Este � um controller para TESTES e DEMONSTRA��O apenas.
    /// Em produ��o, este controller deve ser removido ou protegido adequadamente.
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
            // Cria uma nova inst�ncia do PaymentGateway para testes
            _paymentGateway = new PaymentGateway();
        }

        /// <summary>
        /// Endpoint para testar envio de email de PIX (sempre aprovado)
        /// 
        /// Este endpoint simula um pagamento via PIX que � processado instantaneamente
        /// e sempre retorna como aprovado, enviando um email de confirma��o.
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
                // VALIDA��ES OBRIGAT�RIAS PARA PIX
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote � obrigat�rio");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // PROCESSA PAGAMENTO PIX (n�o precisa de dados de cart�o)
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
                        processing = "Instant�neo"
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
        /// Este endpoint simula um pagamento via Boleto banc�rio que fica
        /// com status pendente aguardando compensa��o.
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
                // VALIDA��ES OBRIGAT�RIAS PARA BOLETO
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote � obrigat�rio");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // PROCESSA BOLETO (n�o precisa de dados de cart�o)
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
                        message = "C�digo de barras e linha digit�vel gerados automaticamente",
                        expiration = "3 dias",
                        processing = "At� 3 dias �teis para compensar"
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
        /// Endpoint para testar envio de email de Cart�o de Cr�dito (sempre recusado)
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
                // VALIDA��ES OBRIGAT�RIAS PARA CART�O DE CR�DITO
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote � obrigat�rio");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // VALIDA��ES ESPEC�FICAS DO CART�O DE CR�DITO
                if (string.IsNullOrEmpty(request.CardNumber))
                    return BadRequest("N�mero do cart�o � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CardHolderName))
                    return BadRequest("Nome do titular � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.ExpiryDate))
                    return BadRequest("Data de validade � obrigat�ria");
                
                if (string.IsNullOrEmpty(request.CVV))
                    return BadRequest("CVV � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CardBrand))
                    return BadRequest("Bandeira do cart�o � obrigat�ria");
                
                if (request.Installments < 1 || request.Installments > 24)
                    return BadRequest("N�mero de parcelas deve ser entre 1 e 24");

                // MONTA DADOS DO CART�O DE CR�DITO
                var cardData = new PaymentGateway.CardData
                {
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName.ToUpper(),
                    ExpiryDate = request.ExpiryDate,
                    CVV = request.CVV,
                    Brand = request.CardBrand,
                    Installments = request.Installments
                };

                // PROCESSA PAGAMENTO DE CART�O DE CR�DITO
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
                    message = "Cart�o de Cr�dito processado! Status: RECUSADO",
                    paymentMethod = "Cart�o de Cr�dito",
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
                    message = $"Erro ao processar cart�o de cr�dito: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Endpoint para testar envio de email de Cart�o de D�bito (sempre recusado)
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
                // VALIDA��ES OBRIGAT�RIAS PARA CART�O DE D�BITO
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.FirstName))
                    return BadRequest("Nome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.LastName))
                    return BadRequest("Sobrenome � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CPFPassport))
                    return BadRequest("CPF/Passaporte � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.PackageName))
                    return BadRequest("Nome do pacote � obrigat�rio");
                
                if (request.Amount <= 0)
                    return BadRequest("Valor do pacote deve ser maior que zero");

                // VALIDA��ES ESPEC�FICAS DO CART�O DE D�BITO
                if (string.IsNullOrEmpty(request.CardNumber))
                    return BadRequest("N�mero do cart�o � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CardHolderName))
                    return BadRequest("Nome do titular � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.ExpiryDate))
                    return BadRequest("Data de validade � obrigat�ria");
                
                if (string.IsNullOrEmpty(request.CVV))
                    return BadRequest("CVV � obrigat�rio");
                
                if (string.IsNullOrEmpty(request.CardBrand))
                    return BadRequest("Bandeira do cart�o � obrigat�ria");

                // MONTA DADOS DO CART�O DE D�BITO (sempre 1 parcela)
                var cardData = new PaymentGateway.CardData
                {
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName.ToUpper(),
                    ExpiryDate = request.ExpiryDate,
                    CVV = request.CVV,
                    Brand = request.CardBrand,
                    Installments = 1 // D�bito sempre � vista
                };

                // PROCESSA PAGAMENTO DE CART�O DE D�BITO
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
                    message = "Cart�o de D�bito processado! Status: RECUSADO",
                    paymentMethod = "Cart�o de D�bito",
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
                        paymentType = "� vista"
                    },
                    emailSent = request.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Erro ao processar cart�o de d�bito: {ex.Message}" 
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
                    return BadRequest("Email � obrigat�rio");
                }

                // Simula um link de reset de senha
                var resetToken = Guid.NewGuid().ToString();
                var resetLink = $"https://newhorizon.com/reset-password?token={resetToken}";
                var username = request.FirstName ?? "Usu�rio";

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
        /// Este endpoint � �til para testar rapidamente todos os templates
        /// de email do sistema enviando para o mesmo endere�o.
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
                    return BadRequest("Email � obrigat�rio");
                }

                var results = new List<object>();

                // 1. Teste PIX (Aprovado)
                try
                {
                    _paymentGateway.DemonstrateSpecificPaymentType(
                        PaymentMethod.Pix,
                        request.Email,
                        "Jo�o", "Silva", "12345678901",
                        "Paris", "S�o Paulo", "Pacote Europa Cl�ssica",
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

                // 3. Teste Cart�o de Cr�dito (Recusado) - COM DADOS DO CART�O
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
                        "Nova York", "Bras�lia", "Pacote Estados Unidos Completo",
                        DateTime.UtcNow.AddDays(60), DateTime.UtcNow.AddDays(67),
                        creditCardData
                    );
                    results.Add(new { tipo = "Cart�o de Cr�dito", status = "Recusado", parcelas = 6, enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "Cart�o de Cr�dito", status = "Erro", erro = ex.Message });
                }

                // 4. Teste Cart�o de D�bito (Recusado) - COM DADOS DO CART�O
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
                        "Gramado", "Porto Alegre", "Pacote Serra Ga�cha Rom�ntico",
                        DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(25),
                        debitCardData
                    );
                    results.Add(new { tipo = "Cart�o de D�bito", status = "Recusado", parcelas = 1, enviado = true });
                }
                catch (Exception ex)
                {
                    results.Add(new { tipo = "Cart�o de D�bito", status = "Erro", erro = ex.Message });
                }

                // 5. Teste Reset de Senha
                try
                {
                    var resetToken = Guid.NewGuid().ToString();
                    var resetLink = $"https://newhorizon.com/reset-password?token={resetToken}";
                    EmailService.SendPasswordResetEmail(request.Email, "Usu�rio Teste", resetLink);
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
    /// Cont�m dados comuns: cliente e pacote
    /// </summary>
    public abstract class BasePaymentRequest
    {
        /// <summary>
        /// Email do cliente (obrigat�rio)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Primeiro nome do cliente (obrigat�rio)
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do cliente (obrigat�rio)
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// CPF ou Passaporte do cliente (obrigat�rio)
        /// </summary>
        public string CPFPassport { get; set; } = string.Empty;

        /// <summary>
        /// Nome do pacote de viagem escolhido (obrigat�rio)
        /// </summary>
        public string PackageName { get; set; } = string.Empty;

        /// <summary>
        /// Cidade/pa�s de origem (obrigat�rio)
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Cidade/pa�s de destino (obrigat�rio)
        /// </summary>
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Data de in�cio da viagem (obrigat�rio)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data de fim da viagem (obrigat�rio)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Valor total do pacote (obrigat�rio)
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// DTO para pagamento via PIX
    /// PIX s� precisa dos dados b�sicos - sem dados de cart�o
    /// </summary>
    public class PixPaymentRequest : BasePaymentRequest
    {
        // PIX n�o precisa de campos adicionais
        // O QR Code/copia-e-cola � gerado automaticamente pelo sistema
    }

    /// <summary>
    /// DTO para pagamento via Boleto
    /// Boleto s� precisa dos dados b�sicos - sem dados de cart�o
    /// </summary>
    public class BoletoPaymentRequest : BasePaymentRequest
    {
        // Boleto n�o precisa de campos adicionais
        // O c�digo de barras e linha digit�vel s�o gerados automaticamente
    }

    /// <summary>
    /// DTO para pagamento via Cart�o de Cr�dito
    /// Inclui todos os campos do cart�o + n�mero de parcelas
    /// </summary>
    public class CreditCardPaymentRequest : BasePaymentRequest
    {
        /// <summary>
        /// N�mero do cart�o de cr�dito (obrigat�rio)
        /// Exemplo: "4111111111111111"
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Nome do titular como no cart�o (obrigat�rio)
        /// Exemplo: "JOAO SILVA"
        /// </summary>
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Data de validade MM/AA (obrigat�rio)
        /// Exemplo: "12/28"
        /// </summary>
        public string ExpiryDate { get; set; } = string.Empty;

        /// <summary>
        /// C�digo CVV do cart�o (obrigat�rio)
        /// Exemplo: "123"
        /// </summary>
        public string CVV { get; set; } = string.Empty;

        /// <summary>
        /// Bandeira do cart�o (obrigat�rio)
        /// Exemplo: "Visa", "Mastercard", "Elo"
        /// </summary>
        public string CardBrand { get; set; } = string.Empty;

        /// <summary>
        /// N�mero de parcelas (obrigat�rio)
        /// M�nimo: 1, M�ximo: 24
        /// </summary>
        public int Installments { get; set; } = 1;
    }

    /// <summary>
    /// DTO para pagamento via Cart�o de D�bito
    /// Inclui dados do cart�o mas SEM parcelas (sempre � vista)
    /// </summary>
    public class DebitCardPaymentRequest : BasePaymentRequest
    {
        /// <summary>
        /// N�mero do cart�o de d�bito (obrigat�rio)
        /// Exemplo: "5555555555554444"
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Nome do titular como no cart�o (obrigat�rio)
        /// Exemplo: "JOAO SILVA"
        /// </summary>
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Data de validade MM/AA (obrigat�rio)
        /// Exemplo: "03/27"
        /// </summary>
        public string ExpiryDate { get; set; } = string.Empty;

        /// <summary>
        /// C�digo CVV do cart�o (obrigat�rio)
        /// Exemplo: "456"
        /// </summary>
        public string CVV { get; set; } = string.Empty;

        /// <summary>
        /// Bandeira do cart�o (obrigat�rio)
        /// Exemplo: "Visa", "Mastercard", "Elo"
        /// </summary>
        public string CardBrand { get; set; } = string.Empty;
        
        // Nota: D�bito n�o tem campo Installments pois � sempre � vista (1x)
    }

    /// <summary>
    /// DTO para reset de senha
    /// S� precisa do email
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Email do usu�rio (obrigat�rio)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usu�rio (opcional)
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