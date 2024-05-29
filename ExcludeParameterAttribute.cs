using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mexico_Utility
{
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class ExcludeParameterAttribute : Attribute
        {
        }
    
}
