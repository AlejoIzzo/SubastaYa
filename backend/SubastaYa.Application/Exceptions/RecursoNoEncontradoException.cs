using System;

namespace SubastaYa.Application.Exceptions
{
    public class RecursoNoEncontradoException : DominioException
    {
        public RecursoNoEncontradoException(string mensaje) : base(mensaje)
        {
        }
    }
}
