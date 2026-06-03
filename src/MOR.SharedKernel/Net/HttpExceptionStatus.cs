namespace System.Net
{
    public static class HttpExceptionStatus
    {
        private const string DEF_ERR_MSG = "Inernal Error";


        internal static Dictionary<Type, HttpStatusCode> MapExceptionHttpStatus = new Dictionary<Type, HttpStatusCode>();
        private static Type TypeHttpServiceException = typeof(HttpServiceException);


        static HttpExceptionStatus()
        {
            MapExceptionHttpStatus.TryAdd(typeof(NotFoundException), HttpStatusCode.NotFound);
            MapExceptionHttpStatus.TryAdd(typeof(UnauthorizedAccessException), HttpStatusCode.Forbidden);
            MapExceptionHttpStatus.TryAdd(typeof(System.Security.SecurityException), HttpStatusCode.Forbidden);
            MapExceptionHttpStatus.TryAdd(typeof(ConflictException), HttpStatusCode.Conflict);
            MapExceptionHttpStatus.TryAdd(typeof(InvalidParameterException), HttpStatusCode.BadRequest);
            MapExceptionHttpStatus.TryAdd(typeof(InvalidDataException), HttpStatusCode.BadRequest);
            MapExceptionHttpStatus.TryAdd(typeof(NotImplementedException), HttpStatusCode.NotImplemented);
            MapExceptionHttpStatus.TryAdd(typeof(NotSupportedException), HttpStatusCode.NotImplemented);
        }


        public static void Register<TException>(HttpStatusCode statusCode)
            where TException : Exception
        {
            MapExceptionHttpStatus.TryAdd(typeof(TException), statusCode);
        }

        public static void Unregister<TException>()
            where TException : Exception
        {
            var type = typeof(TException);

            if (MapExceptionHttpStatus.ContainsKey(TypeHttpServiceException))
            {
                MapExceptionHttpStatus.Remove(type);
            }
        }

        public static KeyValuePair<HttpStatusCode, string> GetStatusAndMessage(Exception ex, string defaultErrorMessage = DEF_ERR_MSG)
        {
            var isHttpSvcException = TypeHttpServiceException.IsInstanceOfType(ex);

            if (isHttpSvcException)
            {
                var hex = (HttpServiceException)ex;
                return new KeyValuePair<HttpStatusCode, string>(hex.HttpStatus, hex.Message);
            }
            else
            {
                var exType = MapExceptionHttpStatus.Keys.SingleOrDefault(t => t.IsInstanceOfType(ex));

                if (exType != null)
                {
                    var stat = MapExceptionHttpStatus[exType];
                    return new KeyValuePair<HttpStatusCode, string>(stat, ex.Message);
                }
            }

            if (defaultErrorMessage.NullOrWhiteSpace())
            {
                defaultErrorMessage = DEF_ERR_MSG;
            }

            return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.InternalServerError, defaultErrorMessage);
        }
    }
}
