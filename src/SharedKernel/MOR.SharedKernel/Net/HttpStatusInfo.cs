namespace System.Net
{
    public class HttpStatusInfo
    {
        public readonly string Code;
        public readonly string Message;
        public readonly string Description;

        private static Dictionary<string, string> _dic = new Dictionary<string, string>();


        static HttpStatusInfo()
        {
            _dic.Add("0", "Unknown\t");

            _dic.Add("100", "Continue\tClient can continue with its request");
            _dic.Add("101", "SwitchingProtocols\tThe protocol version or protocol is being changed");

            _dic.Add("200", "OK\tRequest succeeded and the requested information is in the response");
            _dic.Add("201", "Created\tRequest resulted in a new resource created before the response was sent");
            _dic.Add("202", "Accepted\tRequest has been accepted for further processing");
            _dic.Add("203", "NonAuthoritativeInformation\tReturned metainformation is from a cached copy instead of the origin server and therefore may be incorrect");
            _dic.Add("204", "NoContent\tRequest has been successfully processed and the response is intentionally blank");
            _dic.Add("205", "ResetContent\tClient should reset (not reload) the current resource");

            _dic.Add("300", "Ambiguous/Multiple Choices\tA link list. The user can select a link and go to that location. Maximum five addresses");
            _dic.Add("301", "Moved/Moved Permanently\tThe requested page has moved to a new URL");
            _dic.Add("302", "Redirect/Found\tThe requested page has moved temporarily to a new URL");
            _dic.Add("303", "RedirectMethod/See Other\tThe requested page can be found under a different URL");
            _dic.Add("304", "Not Modified\tIndicates the requested page has not been modified since last requested");
            _dic.Add("306", "Switch Proxy\tNo longer used");
            _dic.Add("307", "Temporary Redirect\tThe requested page has moved temporarily to a new URL");
            _dic.Add("308", "Resume Incomplete\tUsed in the resumable requests proposal to resume aborted PUT or POST requests");

            _dic.Add("400", "Bad Request\tThe request cannot be fulfilled due to bad syntax");
            _dic.Add("401", "Unauthorized\tThe requested resource requires user authorization");
            _dic.Add("402", "Payment Required\tReserved for future use");
            _dic.Add("403", "Forbidden\tThe request was a legal request, but the server is refusing to respond to it");
            _dic.Add("404", "Not Found\tThe requested page could not be found but may be available again in the future");
            _dic.Add("405", "Method Not Allowed\tA request was made of a page using a request method not supported by that page");
            _dic.Add("406", "Not Acceptable\tThe server can only generate a response that is not accepted by the client");
            _dic.Add("407", "Proxy Authentication Required\tThe client must first authenticate itself with the proxy");
            _dic.Add("408", "Request Timeout\tThe server timed out waiting for the request");
            _dic.Add("409", "Conflict\tThe request could not be completed because of a conflict in the request");
            _dic.Add("410", "Gone\tThe requested page is no longer available");
            _dic.Add("411", "Length Required\tThe \"Content-Length\" is not defined. The server will not accept the request without it ");
            _dic.Add("412", "Precondition Failed\tThe precondition given in the request evaluated to false by the server");
            _dic.Add("413", "Request Entity Too Large\tThe server will not accept the request, because the request entity is too large");
            _dic.Add("414", "Request-URI Too Long\tThe server will not accept the request, because the URL is too long. Occurs when you convert a POST request to a GET request with a long query information");
            _dic.Add("415", "Unsupported Media Type\tThe server will not accept the request, because the media type is not supported");
            _dic.Add("416", "Requested Range Not Satisfiable\tThe client has asked for a portion of the file, but the server cannot supply that portion");
            _dic.Add("417", "Expectation Failed\tThe server cannot meet the requirements of the Expect request-header field");
            _dic.Add("426", "Upgrade Required\tUpgrade Required");

            _dic.Add("500", "Internal Server Error\tAn internal server error has been occured");
            _dic.Add("501", "Not Implemented\tThe server either does not recognize the request method, or it lacks the ability to fulfill the request");
            _dic.Add("502", "Bad Gateway\tThe server was acting as a gateway or proxy and received an invalid response from the upstream server");
            _dic.Add("503", "Service Unavailable\tThe server is currently unavailable (overloaded or down)");
            _dic.Add("504", "Gateway Timeout\tThe server was acting as a gateway or proxy and did not receive a timely response from the upstream server");
            _dic.Add("505", "HTTP Version Not Supported\tThe server does not support the HTTP protocol version used in the request");
            _dic.Add("511", "Network Authentication Required\tThe client needs to authenticate to gain network access");
        }

        private HttpStatusInfo(string code, string message, string description)
        {
            Code = code;
            Message = message;
            Description = description;
        }


        public static HttpStatusInfo GetByStatusCode(int statusCode)
        {
            return GetByStatusCode(statusCode.ToString());
        }

        public static HttpStatusInfo GetByStatusCode(HttpStatusCode statusCode)
        {
            return GetByStatusCode(((int)statusCode).ToString());
        }

        public static HttpStatusInfo GetByStatusCode(string statusCode)
        {
            var data = string.Empty;

            if (!_dic.TryGetValue(statusCode, out data))
            {
                data = _dic["0"];
            }

            var split = data.Split('\t');
            var ret = new HttpStatusInfo(statusCode, split[0], split[1]);

            return ret;
        }
    }
}
