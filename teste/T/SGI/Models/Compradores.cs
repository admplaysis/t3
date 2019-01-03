using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public static class Compradores
    {
        public static string GetEmailComprador(string usuario)
        {
            string email = "";
            switch (usuario)
            {
                case "000061":
                    email = "marcio.correa@jaepel.com.br";
                    break;
                case "000160":
                    email = "antonio.lucas@jaepel.com.br";
                    break;
                case "000235":
                    email = "tardely.marques@jaepel.com.br";
                    break;
                case "000250":
                    email = "alvenir.frigotto@jaepel.com.br";
                    break;
                case "000342":
                    email = "compras@jaepel.com.br";
                    break;
            }
            return email;
        }
    }
}