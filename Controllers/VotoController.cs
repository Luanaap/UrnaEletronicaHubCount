using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrnaEletronica.Models;
using static UrnaEletronica.Models.ApiModels;
using static UrnaEletronica.Models.CandidatoViewModels;
using static UrnaEletronica.Models.VotoViewModels;

namespace UrnaEletronica.Controllers
{
    public class VotoController : Controller
    {
        public static HttpClient client = new HttpClient();
        public static int etapa = 0;
        public static int legenda = 0;
        //Etapa 1 = escolha do candidato | Etapa 2 =  confirmação da escolha | Etapa 3 = finalização

        public void Reiniciar()
        {
            //Reinicia variaveis para controle de sessão
            legenda = 0; etapa = 0;
        }
        public IActionResult Index()
        {
            Reiniciar();
            return PartialView("_TelaInicial");
        }
        public IActionResult BotaoBranco()
        {
            // Anular o voto funciona em qualquer etapa, e após, joga para a tela de CONFIRMAÇÃO DO VOTO

            etapa = 1; // Seta que a etapa/tela atual é a de CONFIRMAÇÃO DO VOTO
            legenda = 000; // Código do sistema utilizado para identificar um voto em branco.

            ViewBag.Legenda = legenda;
            return PartialView("_TelaConfirma");
        }
        public IActionResult BotaoCorrige()
        {
            Reiniciar();
            return RedirectToAction("Index", "Voto");
        }
        public async Task<IActionResult> BotaoConfirma(int data)
        {
            //Recebe o voto vindo do front atraves do AngularJS

            if (etapa == 0) //Caso esteja na fase inicial
            {
                // Busca o candidato e exibe na tela para confirmação
                HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/getcand/" + data);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Candidato>(result);
                    etapa = 1; // Define a proxima etapa, para confirmação do candidato que será exibido e finalizar o voto
                    legenda = obj.Legenda;

                    ViewBag.Legenda = obj.Legenda;
                    ViewBag.Nome = obj.NomeCompleto;
                    ViewBag.Vice = obj.Vice;

                    return PartialView("_TelaConfirma");
                }
                else
                {
                    return PartialView("_Info", "Houve algum erro, tente novamente!");
                }
            }
            else if (etapa == 1) // Caso já esteja no segundo passo
            {
                //Confirma o voto no candidato exibido e encaminha pra tela final
                var nVoto = new VotoViewModel
                {
                    Candidato = legenda
                };

                //Cria o modelo de req
                HttpRequestMessage request = new HttpRequestMessage
                {
                    //Converte o objeto para ser enviado
                    Content = new StringContent(JsonConvert.SerializeObject(nVoto), Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://" + this.Request.Host + "/api/vote")

                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    etapa = 2; // Define a etapa final para conclusão do processo de votação

                    ViewBag.Legenda = nVoto.Candidato;

                    var obj = JsonConvert.DeserializeObject<CandidatoViewModel>(result);
                    return PartialView("_TelaFinal", obj);
                }
                else
                {
                    return PartialView("_Info", "Houve algum erro, tente novamente!");
                }

            }
            else
            {
                Reiniciar();
                return PartialView("_TelaInicial");
            }
        }
        public async Task<IActionResult> Resultados()
        {
            HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/votes");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<ResultadosViewModel>>(result);
                return PartialView("_Resultados", obj);
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }
        }
        public async Task<IActionResult> ResetarBrancos()
        {
            // Envia a requisição de reset para a api
            HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/resetwhite/");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Resultados", "Voto");
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }

        }
    }
}
