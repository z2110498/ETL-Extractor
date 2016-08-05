using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Extractor.Extract
{
    public static class Extension
    {
        /// <summary>
        /// Read the next specified unit whcih split by pattern
        /// </summary>
        /// <param name="stream">Reading stream</param>
        /// <param name="pattern">split pattern</param>
        /// <param name="encoding">string encoding</param>
        /// <param name="matchAsStartOrEnd">the pattern as start/end of the unit</param>
        /// <returns>the unit which is first met or others when on such unit</returns>
        public static string ReadUnit(this Stream stream, string pattern, Encoding encoding, PatternMatchOption matchAsStartOrEnd)
        {
            var c = 0;
            var v = string.Empty;
            var s = string.Empty;
            var x = new Regex(pattern);

            do
            {
                var buffer = new byte[1024];
                c = stream.Read(buffer, 0, buffer.Length);
                s += encoding.GetString(buffer, 0, c);

                if (x.IsMatch(s))
                {
                    var match = x.Match(s);
                    if (matchAsStartOrEnd == PatternMatchOption.MatchAsEnd)
                    {
                        // TODO: I am not sure whether the match index will be larger
                        // than Int32.
                        //
                        v = match.Value;
                        var lastPart = s.Substring(match.Index + v.Length);
                        var offset = encoding.GetBytes(lastPart).LongLength;

                        stream.Seek(-offset, SeekOrigin.Current);

                        s = s.Substring(0, match.Index);
                        break;
                    }
                    else
                    {
                        // first match
                        //
                        if (v == string.Empty)
                        {
                            // if not start with match value, return previous string of the first match
                            //
                            if (match.Index != 0)
                            {
                                var offset = encoding.GetBytes(s.Substring(match.Index)).LongLength;
                                stream.Seek(-offset, SeekOrigin.Current);

                                s = s.Substring(0, match.Index);
                                break;
                            }

                            // record value, remove value from s
                            //
                            v = match.Value;
                            s = s.Substring(v.Length);
                        }
                        // second match
                        //
                        else
                        {
                            var substring = s.Substring(1000);
                            var offset = encoding.GetBytes(s.Substring(match.Index)).LongLength;
                            stream.Seek(-offset, SeekOrigin.Current);

                            s = s.Substring(0, match.Index);
                            break;
                        }
                    }
                }

                // when s is not match the pattern, I don't want the match method cost too much
                // the cost is input too large.
                // the cost may be split pattern not good enough
                // TODO: But I don't know how to do.....

            }
            // if c==0, this is the end of stream
            //
            while (c != 0);

            if (matchAsStartOrEnd == PatternMatchOption.MatchAsEnd)
            {
                return s + v;
            }

            return v + s;
        }

        public static bool EndOfStream(this Stream stream)
        {
            var res = stream.ReadByte() == -1;

            if (!res)
            {
                stream.Seek(-1, SeekOrigin.Current);
            }

            return res;
        }
    }
}
