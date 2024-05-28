
    using global::PruebasGemini.Entidades;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace PruebasGemini.Web.Models;

        public class ExamenModel
        {
       

      
            public string Resumen { get; set; }
            public string Feedback { get; set; }

            public List<PreguntaModel> Preguntas { get; set; } = new List<PreguntaModel>();

            public ExamenModel() { }

            public ExamenModel(Examen examen)
            {
         
                Resumen = examen.TextoOriginal;
                Preguntas = PreguntaModel.ParsearLista(examen.Preguntas);
                Feedback = examen.Feedback;
            }

            public Examen MapearAEntidad()
            {
                return new Examen
                {
            
                    TextoOriginal = Resumen,
                    Preguntas = PreguntaModel.ParsearListaAEntidad(Preguntas),
                    Feedback = Feedback
                };
            }

            public static List<ExamenModel> MapearLista(List<Examen> examenes)
            {
                return examenes.ConvertAll(e => new ExamenModel(e));
            }
        }

        public class PreguntaModel
        {
      

       
            public string Texto { get; set; }

    
            public string RespuestaUsuario { get; set; }

            public static List<PreguntaModel> ParsearLista(List<Pregunta> preguntas)
            {
                var listaPreguntas = new List<PreguntaModel>();
                foreach (var pregunta in preguntas)
                {
                    var preguntaModel = new PreguntaModel();
                    preguntaModel.ParsearPregunta(pregunta.Texto);
                    listaPreguntas.Add(preguntaModel);
                }
                return listaPreguntas;
            }

    public void ParsearPregunta(string pregunta)
    {
        var partes = pregunta.Split('.');
        if (partes.Length > 1)
        {
            Texto = partes[1].Trim();
        }
        else
        {
            Texto = pregunta.Trim();
        }
    }

    public Pregunta MapearAEntidad()
            {
                return new Pregunta
                {
            
                    Texto = Texto,
                    RespuestaUsuario = RespuestaUsuario
                };
            }

            public static List<Pregunta> ParsearListaAEntidad(List<PreguntaModel> preguntas)
            {
                return preguntas.ConvertAll(p => p.MapearAEntidad());
            }
        }

    