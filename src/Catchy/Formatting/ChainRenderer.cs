using System.Text;

namespace Catchy.Sdk
{
    public static class ChainRenderer
    {
        public static (string chain, IReadOnlyList<(string ph, string full)> truncations) Render(
            IReadOnlyList<string> links, int maxLen = 60)
        {
            if (maxLen < 20) maxLen = 20;

            var trunc = new List<(string, string)>();
            var sb = new StringBuilder();
            int n = 1;
            int side = Math.Max(6, (maxLen - 6) / 2);

            foreach (var link in links)
            {
                if (string.IsNullOrEmpty(link)) continue;
                if (link.Length > maxLen)
                {
                    var ph = "t" + n++;
                    trunc.Add((ph, link));

                    var start = link.Substring(0, Math.Min(side, link.Length));
                    var endLen = Math.Min(side, Math.Max(0, link.Length - start.Length));
                    var end = endLen > 0 ? link.Substring(link.Length - endLen) : string.Empty;

                    sb.Append(start).Append("{" + ph + "}").Append(end);
                }
                else
                {
                    sb.Append(link);
                }
            }

            return (sb.ToString(), trunc);
        }
    }
}
