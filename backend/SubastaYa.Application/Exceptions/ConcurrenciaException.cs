using System;

namespace SubastaYa.Application.Exceptions
{
    public class ConcurrenciaException : Exception
    {
        public ConcurrenciaException(string mensaje) : base(mensaje)
        {
        }
    }
}
