namespace PruebasGemini.Entidades
{
    public class Examen
    {
        public int Id { get; set; }
        public string TextoOriginal { get; set; }
        public List<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
        public string Feedback { get; set; }
    }

    public class Pregunta
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string RespuestaCorrecta { get; set; }
        public string RespuestaUsuario { get; set; }
    }
}
