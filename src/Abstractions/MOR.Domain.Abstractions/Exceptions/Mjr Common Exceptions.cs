namespace MOR.Exceptions
{
    public class MjrSystemException : MjrException
    {
        protected MjrSystemException(MjrExceptionCategory errorCategory, string errorCode)
            : base(MjrExceptionCategory.System, errorCode)
        {
        }

        protected MjrSystemException(MjrExceptionCategory errorCategory, string errorCode, Exception innerException)
            : base(MjrExceptionCategory.System, errorCode, innerException)
        {
        }
    }

    public class MjrGeneralException : MjrException
    {
        protected MjrGeneralException(MjrExceptionCategory errorCategory, string errorCode)
            : base(errorCategory, errorCode)
        {
        }

        protected MjrGeneralException(MjrExceptionCategory errorCategory, string errorCode, Exception innerException)
            : base(errorCategory, errorCode, innerException)
        {
        }
    }
}
