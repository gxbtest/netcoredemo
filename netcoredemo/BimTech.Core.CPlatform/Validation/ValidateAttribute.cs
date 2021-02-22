using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class ValidateAttribute : Attribute
    {
        public ValidateAttribute()
        {

        }
    }
}
