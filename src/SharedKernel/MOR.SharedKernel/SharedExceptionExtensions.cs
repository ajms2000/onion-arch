namespace System
{
    public static class SharedExceptionExtensions
    {
        public static string GetMessagesDeep(this Exception ex)
        {
            var exs = default(Exception[]);

            if (ex is AggregateException)
            {
                var aex = (AggregateException)ex;
                exs = aex.InnerExceptions.ToArray();
            }
            else
            {
                exs = new[] { ex };
            }

            var list = new List<string>();

            for (int i = 0; i < exs.Length; i++)
            {
                var inList = new List<string>();

                if (exs.Length > 1)
                {
                    // If more than one message is there, then number it.
                    inList.Add($"#{i + 1}");
                }

                var terr = exs[i];

                while (terr != null)
                {
                    if (terr.Message.NotNullOrWhiteSpace())
                    {
                        inList.Add(terr.Message.Trim());
                    }

                    terr = terr.InnerException;
                }

                list.Add(string.Join(" ", inList));
            }

            var ret = string.Join(" ", list);
            return ret;
        }
    }
}
