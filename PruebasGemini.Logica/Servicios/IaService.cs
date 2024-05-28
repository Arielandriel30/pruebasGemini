using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using PruebasGemini.Entidades;

namespace PruebasGemini.Logica.Servicios
{

    public interface IIaService
    {
        Task<string[]> GenerateQuestions(Examen examen);
        Task<string> GetFeedback(Examen examen);
        public List<Examen> GetExamenes();
    }
    public class IaService : IIaService
    {
        private static List<Examen> _examenes = new List<Examen>();
        private readonly IApiService _apiService;
        private readonly ILogger<IaService> _logger;

        public IaService(IApiService apiService, ILogger<IaService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<string[]> GenerateQuestions(Examen examen)
        {
            try
            {
                string textoConPrompt = $"Generame 10 preguntas de este texto: {examen.TextoOriginal}";

                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = textoConPrompt
                            }
                        }
                    }
                }
                };

                var jsonResponse = await _apiService.PostAsync("v1/models/gemini-1.5-pro:generateContent", requestBody);
                var responseData = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var questions = new List<string>();

                if (responseData != null && responseData["candidates"] != null && responseData["candidates"][0] != null
                    && responseData["candidates"][0]["content"] != null && responseData["candidates"][0]["content"]["parts"] != null
                    && responseData["candidates"][0]["content"]["parts"][0] != null && responseData["candidates"][0]["content"]["parts"][0]["text"] != null)
                {
                    var text = responseData["candidates"][0]["content"]["parts"][0]["text"].ToString();
                    var lines = text.Split('\n');

                    foreach (var line in lines)
                    {
                        if (Regex.IsMatch(line, @"^\d+\.\s"))
                        {
                            questions.Add(line.Trim());
                        }
                    }

                    if (questions.Any())
                    {
                        // Agregar el examen a la lista
                        _examenes.Add(examen);

                        return questions.ToArray();
                    }
                    else
                    {
                        _logger.LogError("No se encontraron preguntas en el texto.");
                        return null;
                    }
                }
                else
                {
                    _logger.LogError("La estructura de la respuesta de la API no es la esperada.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Excepción al llamar a la API: {ex.Message}");
                return null;
            }
        }




        public async Task<string> GetFeedback(Examen examen)
        {
            try
            {
                var textoConRespuestas = "Quiero que me hagas una corrección de las siguientes preguntas y respuestas:\n\n";
                foreach (var pregunta in examen.Preguntas)
                {
                    textoConRespuestas += $"{pregunta.Texto}\nRespuesta: {pregunta.RespuestaUsuario}\n\n";
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = textoConRespuestas
                            }
                        }
                    }
                }
                };

                var jsonResponse = await _apiService.PostAsync("v1/models/gemini-1.5-pro:generateContent", requestBody);
                var responseData = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (responseData != null && responseData["candidates"] != null && responseData["candidates"][0] != null
                    && responseData["candidates"][0]["content"] != null && responseData["candidates"][0]["content"]["parts"] != null
                    && responseData["candidates"][0]["content"]["parts"][0] != null && responseData["candidates"][0]["content"]["parts"][0]["text"] != null)
                {
                    return responseData["candidates"][0]["content"]["parts"][0]["text"].ToString();
                }
                else
                {
                    _logger.LogError("La estructura de la respuesta de la API no es la esperada.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Excepción al llamar a la API: {ex.Message}");
                return null;
            }
        }

        public List<Examen> GetExamenes()
        {
            return _examenes;
        }

    }
    }
