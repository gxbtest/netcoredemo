using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BimTech.Core.CPlatform.Validation
{
    public interface IValidationProcessor
    {
        void Validate(ParameterInfo parameterInfo, object value);
    }
}
