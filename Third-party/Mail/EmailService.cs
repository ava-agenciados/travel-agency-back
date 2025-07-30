using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Runtime.ConstrainedExecution;
using travel_agency_back.DTOs.Requests.Booking;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;

namespace travel_agency_back.Third_party.Mail
{
    public class EmailService
    {
        private static int _smtPort { get; set; } = 587;
        private static string _smtpUser { get; set; } = "noreplyagenciaviagens@gmail.com";
        private static string _smtpPassword = "qbwy nhlp juyh sjxp";

        public static async Task<IActionResult> SendPasswordResetEmail(string email, string username, string linkReset)
        {
            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(email);
            mail.Subject = "Solicitação de alteração de senha";
            mail.Body = $@"
            <!DOCTYPE html>
            <html lang='pt-br'>
            <head>
                <meta charset='UTF-8'>
                <title>Solicitação de Alteração de Senha</title>
            </head>
            <body style='background-color:#f5f5f5;padding:20px;'>
                <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
                    <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
                        <div style='font-weight:bold;font-size:24px;'>NEWHORIZON</div>
                        <div style='font-size:12px;letter-spacing:2px;'>AGÊNCIA DE VIAGENS</div>
                    </div>
                    <div style='padding:30px;color:#333;'>
                        <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>Redefinição de Senha Solicitada</h1>
                        <p>Olá {username},</p>
                        <p>Recebemos uma solicitação para redefinir a sua senha de acesso. Para continuar com o processo, clique no botão abaixo:</p>
                        <div style='text-align:center;margin:25px 0;'>
                            <a href='{linkReset}' style='display:inline-block;background-color:#2563eb;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px 0;'>Redefinir Minha Senha</a>
                        </div>
                        <p>Se você não solicitou esta alteração, por favor ignore este e-mail ou entre em contato conosco imediatamente.</p>
                        <p>Este link expirará em 24 horas por motivos de segurança.</p>
                        <div style='margin-top:15px;font-size:14px;'>
                            <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                            <p><strong>Email:</strong> suporte@newhorizon.com</p>
                            <p><strong>Telefone:</strong> (11) 1234-5678</p>
                        </div>
                    </div>
                    <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
                        <p>&copy; 2025 NewHorizon Agência de Viagens. Todos os direitos reservados.</p>
                        <p>Endereço: Rua da hora, 123, Recife - PE</p>
                    </div>
                </div>
            </body>
            </html>
            ";
            mail.IsBodyHtml = true; // Define que o corpo do e-mail é HTML

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            // Envia o e-mail
            await smtp.SendMailAsync(mail);
            return new OkObjectResult(new { message = "E-mail de confirmação do PIX enviado com sucesso!" });
        }

        // NOVOS PARÂMETROS: basePrice, extrasValue, discount, finalPrice, optionalsList, packageFeatures
        public static async Task<IActionResult> SendPixPaymentConfirmation(User user, PaymentDTO payment, Packages packages, Booking booking, ILogger logger, decimal basePrice, decimal extrasValue, decimal discount, decimal finalPrice, List<string> optionalsList, string packageFeatures)
        {
            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(addresses: user.Email);
            mail.Subject = "✅ PIX Aprovado - Viagem Confirmada - NewHorizon";
            mail.Body = GetPixEmailBody(
                user.FirstName,
                user.LastName,
                user.CPFPassport,
                finalPrice,
                packages.Name,
                packages.Destination,
                packages.Origin,
                booking.TravelDate,
                booking.BookingDate,
                payment.TransactionId.ToString(),
                basePrice,
                extrasValue,
                discount,
                finalPrice,
                optionalsList,
                packageFeatures
            );
            mail.IsBodyHtml = true;
            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(mail);
            return new OkObjectResult(new { message = "E-mail de confirmação do PIX enviado com sucesso!" });
        }

        public static void SendBoletoEmail(string email, string FirstName, string LastName, string CPFPassport, decimal Amount, string NomePacotes, string Destino, string Origem, DateTime InicioViagem, DateTime FimViagem, decimal basePrice, decimal extrasValue, decimal discount, List<string> optionalsList, string packageFeatures)
        {
            // Gera dados simplificados do boleto (sem complexidade desnecessária)
            var codigoBarras = $"34191{Amount:00000000}{DateTime.UtcNow:yyyyMMdd}001234567890";
            var linhaDigitavel = $"34191.11111 11111.111111 11111.111111 1 {DateTime.UtcNow.AddDays(3):yyyyMMdd}";

            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(email);
            mail.Subject = "📋 Boleto Bancário - Pagamento Pendente - NewHorizon";
            mail.Body = $@"
<!DOCTYPE html>
<html lang='pt-br'>
<head>
    <meta charset='UTF-8'>
    <title>Boleto Bancário - Pagamento Pendente</title>
</head>
<body style='background-color:#f5f5f5;padding:20px;'>
    <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
            <div style='font-weight:bold;font-size:24px;'>NEWHORIZON</div>
            <div style='font-size:12px;letter-spacing:2px;'>AGÊNCIA DE VIAGENS</div>
        </div>
        <div style='padding:30px;color:#333;'>
            <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>📋 Boleto Bancário Gerado</h1>
            <p>Olá {FirstName} {LastName},</p>
            <p>Para confirmar sua viagem, realize o pagamento do boleto abaixo:</p>
            
            <div style='background-color:#fff3cd;border:1px solid #ffeaa7;border-radius:8px;padding:20px;margin:20px 0;'>
                <div style='text-align:center;margin-bottom:15px;'>
                    <span style='font-size:48px;'>⏳</span>
                    <h2 style='color:#856404;margin:10px 0;'>PAGAMENTO PENDENTE</h2>
                </div>
                <p style='color:#856404;text-align:center;font-weight:bold;'>Aguardando compensação do boleto bancário</p>
            </div>

            <div style='margin:20px 0;'>
                <h2 style='color:#2563eb;'>Detalhes da Viagem</h2>
                <div style='background-color:#f8f9fa;padding:15px;border-radius:5px;border-left:4px solid #2563eb;'>
                    <p><strong>Pacote:</strong> {NomePacotes}</p>
                    <p><strong>Origem:</strong> {Origem}</p>
                    <p><strong>Destino:</strong> {Destino}</p>
                    <p><strong>Data de Início:</strong> {InicioViagem:dd/MM/yyyy}</p>
                    <p><strong>Data de Término:</strong> {FimViagem:dd/MM/yyyy}</p>
                    <p><strong>Viajante:</strong> {FirstName} {LastName}</p>
                    <p><strong>Documento:</strong> {CPFPassport}</p>
                </div>
            </div>
                
            <!-- BOLETO BANCÁRIO SIMPLIFICADO -->
            <div style='border:2px solid #000;margin:20px 0;background:#fff;font-family:monospace;'>
                <div style='background:#f8f9fa;padding:10px;border-bottom:1px solid #000;text-align:center;'>
                    <strong>Avabank S.A. - 001</strong>
                </div>
                <div style='padding:15px;'>
                    <div style='margin-bottom:10px;'>
                        <strong>Beneficiário:</strong> NewHorizon Agência de Viagens LTDA<br>
                        <strong>CNPJ:</strong> 12.345.678/0001-90<br>
                        <strong>Endereço:</strong> Av. Boa Viagem, 456, Recife - PE
                    </div>
                    <div style='margin-bottom:10px;'>
                        <strong>Pagador:</strong> {FirstName} {LastName}<br>
                        <strong>CPF/CNPJ:</strong> {CPFPassport}<br>
                        <strong>Email:</strong> {email}
                    </div>
                    <div style='display:flex;justify-content:space-between;margin-bottom:15px;'>
                        <div><strong>Valor:</strong> R$ {Amount:N2}</div>
                        <div><strong>Vencimento:</strong> {DateTime.UtcNow.AddDays(3):dd/MM/yyyy}</div>
                    </div>
                    
                    <!-- CÓDIGO DE BARRAS VISUAL -->
                    <div style=""margin: 20px 0; text-align: center; font-family: Arial, sans-serif;"">
                        <div style=""background-color: #000; height: 60px; width: 100%; max-width: 400px; margin: 0 auto; display: flex; align-items: center; justify-content: center;"">
                            <span style=""color: #fff; font-size: 14px; font-weight: bold;"">CÓDIGO DE BARRAS</span>
                        </div>
                        <div style=""font-size: 13px; margin-top: 8px; letter-spacing: 1.5px;"">
                            {codigoBarras}
                        </div>
                    </div>
                    
                </div>
            </div>

            <div style='text-align:center;margin:25px 0;'>
                <a href='#' style='display:inline-block;background-color:#28a745;color:white !important;text-decoration:none;padding:12px 25px;border-radius:5px;font-weight:bold;margin:5px;'>🖨️ Imprimir Boleto</a>
                <a href='#' style='display:inline-block;background-color:#17a2b8;color:white !important;text-decoration:none;padding:12px 25px;border-radius:5px;font-weight:bold;margin:5px;'>📱 Copiar Linha Digitável</a>
            </div>

            <div style='background-color:#e3f2fd;border:1px solid #1976d2;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#1976d2;margin-top:0;'>💡 Como Pagar:</h3>
                <ul style='color:#1976d2;margin:5px 0;'>
                    <li>📱 <strong>App do Banco:</strong> Escaneie o código de barras</li>
                    <li>🏪 <strong>Casas Lotéricas:</strong> Apresente o boleto impresso</li>
                    <li>🏛️ <strong>Agências Bancárias:</strong> Qualquer banco até o vencimento</li>
                </ul>
            </div>

            <div style='background-color:#f8d7da;border:1px solid #f5c6cb;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#721c24;margin-top:0;'>⚠️ Importante:</h3>
                <ul style='color:#721c24;margin:10px 0;'>
                    <li><strong>Sua viagem só será confirmada após o pagamento</strong></li>
                    <li>O boleto pode demorar até 3 dias úteis para compensar</li>
                    <li>Após o vencimento, será necessário gerar um novo boleto</li>
                    <li>Guarde este comprovante até a confirmação do pagamento</li>
                </ul>
            </div>

            <div style='text-align:center;margin:25px 0;'>
                <a href='#' style='display:inline-block;background-color:#2563eb;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px;'>📱 Acompanhar Status</a>
                <a href='#' style='display:inline-block;background-color:#6c757d;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px;'>🧾 2ª Via do Boleto</a>
            </div>

            <p>Se você não reconhece esta reserva, por favor entre em contato conosco imediatamente.</p>
            
            <div style='margin-top:20px;font-size:14px;'>
                <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                <p><strong>Email:</strong> suporte@newhorizon.com</p>
                <p><strong>Telefone:</strong> (11) 1234-5678</p>
                <p><strong>WhatsApp:</strong> (11) 91234-5678</p>
            </div>
        </div>
        <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
            <p>&copy; 2024 NewHorizon Agência de Viagens. Todos os direitos reservados.</p>
            <p>Endereço: Av. Boa Viagem, 456, Recife - PE | CNPJ: 12.345.678/0001-90</p>
        </div>
    </div>
</body>
</html>

";
            mail.IsBodyHtml = true; // Define que o corpo do e-mail é HTML

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            // Envia o e-mail
            smtp.Send(mail);
        }

        // NOVOS PARÂMETROS: basePrice, extrasValue, discount, optionalsList, packageFeatures
        public static void SendCartaoEmail(string email, string FirstName, string LastName, string CPFPassport, decimal Amount, string NomePacotes, string Destino, string Origem, DateTime InicioViagem, DateTime FimViagem, int parcelas, string status, string transactionId, travel_agency_back.Third_party.PaymentGateway.PaymentGateway.CardData? cardData = null, decimal basePrice = 0, decimal extrasValue = 0, decimal discount = 0, List<string> optionalsList = null, string packageFeatures = "")
        {
            // Determina se é crédito ou débito baseado nas parcelas
            bool isCredito = parcelas > 1;
            string tipoCartao = isCredito ? "Crédito" : "Débito";

            // Usa dados reais do cartão se fornecidos, senão usa dados padrão mascarados
            var cartaoMascarado = !string.IsNullOrEmpty(cardData?.CardNumber) ?
                MaskCardNumber(cardData.CardNumber) : "**** **** **** 1234";
            var cvvMascarado = !string.IsNullOrEmpty(cardData?.CVV) ? "***" : "***";
            var validadeMascarada = !string.IsNullOrEmpty(cardData?.ExpiryDate) ? cardData.ExpiryDate : "12/28";
            var bandeira = !string.IsNullOrEmpty(cardData?.Brand) ? cardData.Brand : "Visa";
            var titularCartao = !string.IsNullOrEmpty(cardData?.CardHolderName) ?
                cardData.CardHolderName.ToUpper() : $"{FirstName?.ToUpper()} {LastName?.ToUpper()}";
            var nsuAutorizacao = new Random().Next(100000, 999999);

            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(email);

            string statusColor = status == "Aprovado" ? "#28a745" : "#dc3545";
            string statusIcon = status == "Aprovado" ? "✅" : "❌";
            string statusMessage = status == "Aprovado" ? "APROVADO" : "RECUSADO";

            mail.Subject = $"{statusIcon} Cartão de {tipoCartao} - {statusMessage} - NewHorizon";
            mail.Body = $@"
<!DOCTYPE html>
<html lang='pt-br'>
<head>
    <meta charset='UTF-8'>
    <title>Pagamento com Cartão de {tipoCartao} - {statusMessage}</title>
</head>
<body style='background-color:#f5f5f5;padding:20px;'>
    <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
            <div style='font-weight:bold;font-size:24px;'>NEWHORIZON</div>
            <div style='font-size:12px;letter-spacing:2px;'>AGÊNCIA DE VIAGENS</div>
        </div>
        <div style='padding:30px;color:#333;'>
            <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>💳 Pagamento com Cartão de {tipoCartao}</h1>
            <p>Olá {FirstName} {LastName},</p>
            
            <div style='background-color:{(status == "Aprovado" ? "#e8f5e8" : "#f8d7da")};border:2px solid {statusColor};border-radius:8px;padding:20px;margin:20px 0;'>
                <div style='text-align:center;margin-bottom:15px;'>
                    <span style='font-size:48px;'>{statusIcon}</span>
                    <h2 style='color:{statusColor};margin:10px 0;'>PAGAMENTO {statusMessage}</h2>
                </div>
                <p><strong>Valor Total:</strong> R$ {Amount:N2}</p>
                {(isCredito ? $"<p><strong>Parcelas:</strong> {parcelas}x de R$ {(Amount / parcelas):N2}</p>" : "<p><strong>Débito:</strong> Pagamento à vista</p>")}
                <p><strong>Data/Hora:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</p>
                <p><strong>ID da Transação:</strong> {transactionId}</p>
            </div>

            <!-- DADOS REAIS DO CARTÃO INFORMADOS PELO CLIENTE -->
            <div style='border:1px solid #ddd;border-radius:8px;padding:20px;margin:20px 0;background:#f8f9fa;'>
                <h3 style='color:#2563eb;margin-top:0;'>💳 Dados do Cartão Informados pelo Cliente</h3>
                
                <!-- Simulação do formulário com dados reais -->
                <div style='background:#fff;border:1px solid #ccc;border-radius:5px;padding:15px;margin-bottom:15px;'>
                    <div style='display:grid;grid-template-columns:1fr 1fr;gap:15px;margin-bottom:15px;'>
                        <div>
                            <label style='font-size:12px;color:#666;font-weight:bold;'>NÚMERO DO CARTÃO:</label>
                            <input type='text' value='{cartaoMascarado}' readonly style='width:100%;padding:8px;border:1px solid #ddd;border-radius:3px;background:#f9f9f9;font-family:monospace;'>
                        </div>
                        <div>
                            <label style='font-size:12px;color:#666;font-weight:bold;'>NOME DO TITULAR:</label>
                            <input type='text' value='{titularCartao}' readonly style='width:100%;padding:8px;border:1px solid #ddd;border-radius:3px;background:#f9f9f9;'>
                        </div>
                    </div>
                    <div style='display:grid;grid-template-columns:1fr 1fr 1fr;gap:15px;'>
                        <div>
                            <label style='font-size:12px;color:#666;font-weight:bold;'>VALIDADE (MM/AA):</label>
                            <input type='text' value='{validadeMascarada}' readonly style='width:100%;padding:8px;border:1px solid #ddd;border-radius:3px;background:#f9f9f9;font-family:monospace;'>
                        </div>
                        <div>
                            <label style='font-size:12px;color:#666;font-weight:bold;'>CVV:</label>
                            <input type='text' value='{cvvMascarado}' readonly style='width:100%;padding:8px;border:1px solid #ddd;border-radius:3px;background:#f9f9f9;font-family:monospace;'>
                        </div>
                        <div>
                            <label style='font-size:12px;color:#666;font-weight:bold;'>BANDEIRA:</label>
                            <input type='text' value='{bandeira}' readonly style='width:100%;padding:8px;border:1px solid #ddd;border-radius:3px;background:#f9f9f9;'>
                        </div>
                    </div>
                </div>
                
                <p style='font-size:12px;color:#666;margin-top:10px;'>ℹ️ Dados informados pelo cliente no formulário de pagamento</p>
                {(status == "Aprovado" ? $"<p style='color:#28a745;'><strong>✅ NSU Autorização:</strong> {nsuAutorizacao}</p>" : "")}
            </div>

            {(status == "Aprovado" ? $@"
            <div style='margin:20px 0;'>
                <h2 style='color:#2563eb;'>🎒 Detalhes da Viagem Confirmada</h2>
                <div style='background-color:#f8f9fa;padding:15px;border-radius:5px;border-left:4px solid #2563eb;'>
                    <p><strong>Pacote:</strong> {NomePacotes}</p>
                    <p><strong>Origem:</strong> {Origem}</p>
                    <p><strong>Destino:</strong> {Destino}</p>
                    <p><strong>Data de Início:</strong> {InicioViagem:dd/MM/yyyy}</p>
                    <p><strong>Data de Término:</strong> {FimViagem:dd/MM/yyyy}</p>
                    <p><strong>Viajante:</strong> {FirstName} {LastName}</p>
                    <p><strong>Documento:</strong> {CPFPassport}</p>
                </div>
            </div>

            {(isCredito ? $@"
            <div style='background-color:#fff3cd;border:1px solid #ffeaa7;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#856404;margin-top:0;'>💡 Informações do Parcelamento:</h3>
                <div style='display:grid;grid-template-columns:1fr 1fr;gap:10px;'>
                    <div>
                        <p style='color:#856404;margin:5px 0;'>• <strong>Total de Parcelas:</strong> {parcelas}x</p>
                        <p style='color:#856404;margin:5px 0;'>• <strong>Valor por Parcela:</strong> R$ {(Amount / parcelas):N2}</p>
                    </div>
                    <div>
                        <p style='color:#856404;margin:5px 0;'>• <strong>1ª Parcela:</strong> {DateTime.UtcNow.AddDays(30):dd/MM/yyyy}</p>
                        <p style='color:#856404;margin:5px 0;'>• <strong>Última Parcela:</strong> {DateTime.UtcNow.AddMonths(parcelas):dd/MM/yyyy}</p>
                    </div>
                </div>
                <p style='color:#856404;margin:10px 0;'>• As parcelas serão debitadas mensalmente no seu cartão</p>
                <p style='color:#856404;margin:5px 0;'>• Guarde este comprovante para controle financeiro</p>
                {(parcelas > 12 ? "<p style='color:#856404;margin:5px 0;'>• Taxa de 5% aplicada para parcelamento acima de 12x</p>" : "")}
            </div>" : @"
            <div style='background-color:#e3f2fd;border:1px solid #1976d2;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#1976d2;margin-top:0;'>💡 Informações do Débito:</h3>
                <p style='color:#1976d2;margin:5px 0;'>• Valor debitado integralmente da conta corrente</p>
                <p style='color:#1976d2;margin:5px 0;'>• Processamento em tempo real</p>
                <p style='color:#1976d2;margin:5px 0;'>• Comprovante válido como recibo</p>
            </div>")}

            <div style='text-align:center;margin:25px 0;'>
                <a href='#' style='display:inline-block;background-color:#2563eb;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px;'>📱 Ver Voucher Digital</a>
                <a href='#' style='display:inline-block;background-color:#6c757d;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px;'>🧾 {(isCredito ? "Fatura do Cartão" : "Extrato Bancário")}</a>
            </div>

            <p style='color:#28a745;font-weight:bold;text-align:center;font-size:18px;'>🎉 Boa viagem e obrigado por escolher a NewHorizon!</p>"
            :
            $@"
            <div style='background-color:#f8d7da;border:1px solid #f5c6cb;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#721c24;margin-top:0;'>❌ Pagamento Não Aprovado</h3>
                <p style='color:#721c24;margin:5px 0;'>Infelizmente, o pagamento com cartão de {tipoCartao.ToLower()} não foi processado com sucesso.</p>
                <p style='color:#721c24;margin:5px 0;'><strong>Dados utilizados na tentativa:</strong></p>
                <ul style='color:#721c24;margin:10px 0;'>
                    <li><strong>Cartão:</strong> {cartaoMascarado}</li>
                    <li><strong>Titular:</strong> {titularCartao}</li>
                    <li><strong>Validade:</strong> {validadeMascarada}</li>
                    <li><strong>Bandeira:</strong> {bandeira}</li>
                </ul>
                <p style='color:#721c24;margin:5px 0;'><strong>Possíveis motivos para recusa:</strong></p>
                <ul style='color:#721c24;margin:10px 0;'>
                    <li><strong>Dados incorretos:</strong> Verifique todas as informações digitadas</li>
                    <li><strong>CVV inválido:</strong> Confirme os 3 dígitos do verso do cartão</li>
                    <li><strong>Cartão expirado:</strong> Verifique se a data está correta</li>
                    <li><strong>Nome diferente:</strong> Digite exatamente como impresso no cartão</li>
                    {(isCredito ? "<li><strong>Limite insuficiente:</strong> Verifique seu limite disponível</li>" : "<li><strong>Saldo insuficiente:</strong> Verifique o saldo da conta corrente</li>")}
                    <li><strong>Cartão bloqueado:</strong> Entre em contato com seu banco</li>
                </ul>
            </div>

            <div style='border:1px solid #dc3545;border-radius:8px;padding:15px;margin:20px 0;background:#fff;'>
                <h3 style='color:#dc3545;margin-top:0;'>🔄 O que fazer agora?</h3>
                <div style='display:grid;grid-template-columns:1fr 1fr;gap:15px;'>
                    <div>
                        <p><strong>✅ Opções Imediatas:</strong></p>
                        <ul style='margin:5px 0;'>
                            <li>Verificar todos os dados do cartão</li>
                        </ul>
                    </div>
                    <div>
                        <p><strong>📞 Precisa de Ajuda?</strong></p>
                        <ul style='margin:5px 0;'>
                            <li>WhatsApp: (11) 91234-5678</li>
                            <li>Telefone: (11) 1234-5678</li>
                            <li>Email: suporte@newhorizon.com</li>
                            <li>Chat online: 24h disponível</li>
                        </ul>
                    </div>
                </div>
            </div>

            <div style='background-color:#fff3cd;border:1px solid #ffeaa7;border-radius:5px;padding:15px;margin:20px 0;'>
                <p style='color:#856404;margin:0;text-align:center;'><strong>⏰ Sua reserva será mantida por 2 horas para nova tentativa de pagamento</strong></p>
            </div>")}
            
            <div style='margin-top:20px;font-size:14px;'>
                <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                <p><strong>Email:</strong> suporte@newhorizon.com</p>
                <p><strong>Telefone:</strong> (11) 1234-5678</p>
                <p><strong>WhatsApp:</strong> (11) 91234-5678</p>
            </div>
        </div>
        <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
            <p>&copy; 2024 NewHorizon Agência de Viagens. Todos os direitos reservados.</p>
            <p>Endereço: Av. Boa Viagem, 456, Recife - PE | CNPJ: 12.345.678/0001-90</p>
        </div>
    </div>
</body>
</html>
";
            mail.IsBodyHtml = true;

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            // Envia o e-mail
            smtp.Send(mail);
        }

        /// <summary>
        /// Máscara o número do cartão para exibição segura
        /// Exemplo: "4111111111111111" -> "**** **** **** 1111"
        /// </summary>
        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return "**** **** **** ****";

            var lastFour = cardNumber.Substring(cardNumber.Length - 4);
            return $"**** **** **** {lastFour}";
        }
        public static async Task<IActionResult> SendRegistrationEmail(string firstName, string lastName, string email)
        {
            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(addresses: email);
            mail.Subject = "Bem Vindo - NewHorizon";
            mail.Body = GetEmailRegistration(firstName, lastName, email);
            mail.IsBodyHtml = true;

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
            return new OkObjectResult(new { message = "E-mail de confirmação de conta enviado com sucesso!" });
        }

        private static string GetPixEmailBody(string FirstName, string LastName, string CPFPassport, decimal Amount, string NomePacotes, string Destino, string Origem, DateTime InicioViagem, DateTime FimViagem, string pixCode, decimal basePrice, decimal extrasValue, decimal discount, decimal finalPrice, List<string> optionalsList, string packageFeatures)
        {
            // Monta HTML detalhado com características, opcionais, desconto e cálculo
            string optionalsHtml = optionalsList != null && optionalsList.Count > 0
                ? string.Join("<br>", optionalsList.Select(o => $"- {o} (+2%)"))
                : "Nenhum opcional selecionado";
            return $@"
<!DOCTYPE html>
<html lang='pt-br'>
<head>
    <meta charset='UTF-8'>
    <title>PIX Aprovado - Viagem Confirmada</title>
</head>
<body style='background-color:#f5f5f5;padding:20px;'>
    <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
            <div style='font-weight:bold;font-size:24px;'>NEWHORIZON</div>
            <div style='font-size:12px;letter-spacing:2px;'>AGÊNCIA DE VIAGENS</div>
        </div>
        <div style='padding:30px;color:#333;'>
            <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>🔥 PIX - Confirmação Instantânea</h1>
            <p>Olá {FirstName} {LastName},</p>
            <p>Sua reserva foi <strong style='color:#28a745;'>CONFIRMADA COM SUCESSO</strong> via PIX!</p>
            
            <div style='background-color:#e8f5e8;border:2px solid #28a745;border-radius:8px;padding:20px;margin:20px 0;'>
                <div style='text-align:center;margin-bottom:15px;'>
                    <span style='font-size:48px;'>✅</span>
                    <h2 style='color:#28a745;margin:10px 0;'>PAGAMENTO APROVADO</h2>
                </div>
                <p><strong>Valor Pago:</strong> R$ {finalPrice:N2}</p>
                <p><strong>Data/Hora:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</p>
                <p><strong>ID da Transação:</strong> {pixCode}</p>
                <p style='color:#28a745;text-align:center;font-weight:bold;'>✨ Processamento instantâneo via PIX ✨</p>
            </div>

            <div style='margin:20px 0;'>
                <h2 style='color:#2563eb;'>🎒 Detalhes da Viagem Confirmada</h2>
                <div style='background-color:#f8f9fa;padding:15px;border-radius:5px;border-left:4px solid #2563eb;'>
                    <p><strong>Pacote:</strong> {NomePacotes}</p>
                    <p><strong>Origem:</strong> {Origem}</p>
                    <p><strong>Destino:</strong> {Destino}</p>
                    <p><strong>Data de Início:</strong> {InicioViagem:dd/MM/yyyy}</p>
                    <p><strong>Data de Término:</strong> {FimViagem:dd/MM/yyyy}</p>
                    <p><strong>Viajante:</strong> {FirstName} {LastName}</p>
                    <p><strong>Documento:</strong> {CPFPassport}</p>
                    <p><strong>Características do Pacote:</strong><br>{packageFeatures}</p>
                </div>
            </div>

            <div style='margin:20px 0;'>
                <h2 style='color:#2563eb;'>💡 Opcionais Selecionados</h2>
                <div style='background-color:#f8f9fa;padding:10px;border-radius:5px;border-left:4px solid #2563eb;'>
                    {optionalsHtml}
                </div>
            </div>

            <div style='margin:20px 0;'>
                <h2 style='color:#2563eb;'>💰 Cálculo do Valor</h2>
                <div style='background-color:#f8f9fa;padding:10px;border-radius:5px;border-left:4px solid #2563eb;'>
                    <p>Valor Base: R$ {basePrice:N2}</p>
                    <p>Acréscimo Opcionais: R$ {extrasValue:N2}</p>
                    <p>Desconto: -R$ {discount:N2}</p>
                    <p><strong>Valor Final: R$ {finalPrice:N2}</strong></p>
                </div>
            </div>

            <div style='text-align:center;margin:25px 0;'>
                <a href='#' style='display:inline-block;background-color:#2563eb;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px;'>📱 Ver Voucher Digital</a>
                <a href='#' style='display:inline-block;background-color:#28a745;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px;'>📧 Enviar por WhatsApp</a>
            </div>

            <div style='background-color:#fff3cd;border:1px solid #ffeaa7;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#856404;margin-top:0;'>📋 Próximos Passos:</h3>
                <ul style='color:#856404;margin:0;'>
                    <li>✅ Guarde este e-mail como comprovante</li>
                    <li>📄 Prepare a documentação necessária para a viagem</li>
                    <li>📞 Fique atento aos nossos contatos próximo à data da viagem</li>
                    <li>🎯 Em caso de dúvidas, entre em contato conosco</li>
                </ul>
            </div>

            <p style='color:#28a745;font-weight:bold;text-align:center;font-size:18px;'>🎉 Boa viagem e obrigado por escolher a NewHorizon!</p>
            
            <div style='margin-top:20px;font-size:14px;'>
                <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                <p><strong>Email:</strong> suporte@newhorizon.com</p>
                <p><strong>Telefone:</strong> (11) 1234-5678</p>
                <p><strong>WhatsApp:</strong> (11) 91234-5678</p>
            </div>
        </div>
        <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
            <p>&copy; 2024 NewHorizon Agência de Viagens. Todos os direitos reservados.</p>
            <p>Endereço: Av. Boa Viagem, 456, Recife - PE | CNPJ: 12.345.678/0001-90</p>
        </div>
    </div>
</body>
</html>
    ";
        }
        private static string GetEmailRegistration(string FirstName, String LastName, string email)
        {
            return $@"
<!DOCTYPE html>
<html lang='pt-br'>
<head>
    <meta charset='UTF-8'>
    <title>Conta Criada com Sucesso - NewHorizon</title>
</head>
<body style='background-color:#f5f5f5;padding:20px;'>
    <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
            <div style='font-weight:bold;font-size:24px;'>NEWHORIZON</div>
            <div style='font-size:12px;letter-spacing:2px;'>AGÊNCIA DE VIAGENS</div>
        </div>
        <div style='padding:30px;color:#333;'>
            <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>✅ Confirmação de Conta</h1>
            <p>Olá {FirstName} {LastName},</p>
            <p>Sua conta na NewHorizon foi criada com sucesso e já está ativa!</p>

            <div style='background-color:#f8f9fa;padding:15px;border-radius:5px;border-left:4px solid #2563eb;'>
                <h2 style='color:#2563eb;'>📋 Detalhes da Conta</h2>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Nome:</strong> {FirstName} {LastName}</p>
            </div>

            <div style='background-color:#fff3cd;border:1px solid #ffeaa7;border-radius:5px;padding:15px;margin:20px 0;'>
                <h3 style='color:#856404;margin-top:0;'>📋 Próximos Passos:</h3>
                <ul style='color:#856404;margin:0;'>
                    <li>✅ Acesse sua conta e explore nossos pacotes</li>
                    <li>📄 Complete seu perfil para melhores recomendações</li>
                    <li>📞 Fique atento aos nossos contatos para novidades e promoções</li>
                </ul>
            </div>

            <p style='color:#28a745;font-weight:bold;text-align:center;font-size:18px;'>🎉 Bem-vindo à NewHorizon!</p>
            
            <div style='margin-top:20px;font-size:14px;'>
                <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                <p><strong>Email:</strong> suporte@newhorizon.com</p>
                <p><strong>Telefone:</strong> (11) 1234-5678</p>
                <p><strong>WhatsApp:</strong> (11) 91234-5678</p>
            </div>
        </div>
        <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
            <p>&copy; 2024 NewHorizon Agência de Viagens. Todos os direitos reservados.</p>
            <p>Endereço: Av. Boa Viagem, 456, Recife - PE | CNPJ: 12.345.678/0001-90</p>
        </div>
    </div>
</body>
</html>
";
        }
    }
}