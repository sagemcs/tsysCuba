using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for MulticonsultingException
/// </summary>
public class MulticonsultingException : Exception
{
    public MulticonsultingException()
    {
    }

    public MulticonsultingException(string message) : base(message)
    {
    }

    public MulticonsultingException(string message, Exception innerException) : base(message, innerException)
    {
    }    
}