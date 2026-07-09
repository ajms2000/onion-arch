using MOR.Services;

namespace MOR.Exceptions
{
    public abstract class MjrException : AppException
    {
        private string? _ProcessedMessage;
        private string[]? _Messages;


        protected MjrException(MjrExceptionCategory errorCategory, string errorCode)
            : base(errorCode)
        {
            ErrorCategory = errorCategory;

            if (IsKnownErrorCode(errorCode))
            {
                ErrorCode = errorCode;
            }
            else
            {
                ErrorCode = MjrCodes.DefaultError;
            }
        }

        protected MjrException(MjrExceptionCategory errorCategory, string errorCode, Exception innerException)
            : base(errorCode, innerException)
        {
            ErrorCategory = errorCategory;

            if (IsKnownErrorCode(errorCode))
            {
                ErrorCode = errorCode;
            }
            else
            {
                ErrorCode = MjrCodes.DefaultError;
            }
        }


        public string ErrorCode { get; private set; }

        public virtual MjrExceptionCategory ErrorCategory { get; private set; }

        public string[] Messages
        {
            get
            {
                if (_Messages.AnyAndNotNull())
                {
                    return _Messages ?? [];
                }

                return Message.WrapInArray();
            }
        }

        public override string Message
        {
            get
            {
                if (_ProcessedMessage.NotNullOrWhiteSpace())
                {
                    return _ProcessedMessage ?? ErrorCode;
                }

                return base.Message;
            }
        }


        public bool HasCustomMessage()
        {
            return Messages.AnyAndNotNull();
        }

        public static bool IsKnownErrorCode(string errorCode)
        {
            var ret = false;

            if (errorCode.NotNullOrWhiteSpace())
            {
                ret = System.Text.RegularExpressions.Regex.IsMatch(errorCode, MjrCodes.RegexCode, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return ret;
        }


        internal void FinalizeMessages(ICodeMessageProvider? messageProvider = null, string[]? customMessages = null, object[]? formatParams = null)
        {
            if (customMessages.AnyAndNotNull())
            {
                // Formatting not applicable for custom messages.
                _Messages = customMessages;
                _ProcessedMessage = string.Join(Environment.NewLine, customMessages ?? []);
            }
            else if (messageProvider != null)
            {
                _ProcessedMessage = messageProvider.GetMessage(ErrorCode, ErrorCode);

                if (formatParams.AnyAndNotNull())
                {
                    _ProcessedMessage = string.Format(_ProcessedMessage!, formatParams ?? []);
                }
            }
            else
            {
                _ProcessedMessage = ErrorCode;
            }
        }
    }
}
