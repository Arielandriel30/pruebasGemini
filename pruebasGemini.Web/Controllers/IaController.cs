using Microsoft.AspNetCore.Mvc;
using PruebasGemini.Logica.Servicios;
using PruebasGemini.Web.Models;

namespace pruebasGemini.Web.Controllers
{
    public class IaController : Controller
    {

        private readonly IIaService _iaService;

        public IaController(IIaService iaService)
        {
            _iaService = iaService;
        }

        public IActionResult VistaPreguntas()
        {
            return View(new ExamenModel());
        }

        [HttpPost]
        public async Task<IActionResult> GenerateQuestions(ExamenModel examenModel)
        {
            try
            {
                var examenEntidad = examenModel.MapearAEntidad();
                var preguntasGeneradas = await _iaService.GenerateQuestions(examenEntidad);

                if (preguntasGeneradas != null && preguntasGeneradas.Any())
                {
                    examenModel.Preguntas = preguntasGeneradas.Select(p =>
                    {
                        var preguntaModel = new PreguntaModel();
                        preguntaModel.ParsearPregunta(p);
                        return preguntaModel;
                    }).ToList();

                    return View("MostrarPreguntas", examenModel);
                }
                else
                {
                    ViewBag.Error = "No se pudieron generar preguntas.";
                    return View("VistaPreguntas", examenModel);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al generar preguntas: " + ex.Message;
                return View("VistaPreguntas", examenModel);
            }
        }

        public IActionResult MostrarPreguntas(ExamenModel examenModel)
        {
            return View(examenModel);
        }

        [HttpPost]
        public async Task<IActionResult> CorregirRespuestas(ExamenModel examenModel)
        {
            try
            {
                var examenEntidad = examenModel.MapearAEntidad();
                var feedback = await _iaService.GetFeedback(examenEntidad);

                if (!string.IsNullOrEmpty(feedback))
                {
                    examenModel.Feedback = feedback;
                    return View("MostrarFeedback", examenModel);
                }
                else
                {
                    ViewBag.Error = "No se pudo obtener el feedback.";
                    return View("MostrarPreguntas", examenModel);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener el feedback: " + ex.Message;
                return View("MostrarPreguntas", examenModel);
            }
        }

        public IActionResult MostrarFeedback(ExamenModel examenModel)
        {
            return View(examenModel);
        }
    }
    
    }
