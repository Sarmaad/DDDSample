using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class DuplicateEmailException:Exception
    {
        public string Email { get; private set; }

        public DuplicateEmailException(string email)
        {
            Email = email;
        }
    }
}
