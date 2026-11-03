using System;

namespace SubastaYa.Application.Exceptions
{
    public class SaldoInsuficienteException : DominioException
    {
        public SaldoInsuficienteException(string mensaje) : base(mensaje)
        {
        }
    }
}
