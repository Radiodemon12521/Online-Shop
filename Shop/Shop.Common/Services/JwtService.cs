using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Common.Services
{
    public class JwtService:IJwtService
    {
        public string Token { get => _token; set => _token = value; }
        private string _token;

    }
    public interface IJwtService
    {
        string Token { get; set; }
    }
}
