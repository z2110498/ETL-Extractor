using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Extractor.Extract
{
    internal static class Extension
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
            var v = string.Empty;
            var s = string.Empty;
            var x = new Regex(pattern);

            do
            {
                // Fix bug #15
                //
                s += stream.ReadStream(encoding);

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

                            // record value, set s to empty, match.Index = 0, buffer must only read once
                            //
                            v = match.Value;
                            var lastPart = s.Substring(match.Index + v.Length);
                            var offset1 = encoding.GetBytes(lastPart).LongLength;
                            stream.Seek(-offset1, SeekOrigin.Current);

                            s = string.Empty;
                        }
                        // second match
                        //
                        else
                        {
                            var offset = encoding.GetBytes(s.Substring(match.Index)).LongLength;
                            stream.Seek(-offset, SeekOrigin.Current);

                            s = s.Substring(0, match.Index);
                            break;
                        }
                    }
                }
                // not match
                //
                else
                {
                    // if more than 100M are read, and still not match, 
                    // thow directly to avoid OutOffMemory exception. 
                    //
                    if(encoding.GetByteCount(s) > 100000000)
                    {
                        throw new Exception("Could not match Pattern: " + pattern);
                    }
                }

            }
            while (!stream.EndOfStream());

            if (matchAsStartOrEnd == PatternMatchOption.MatchAsEnd)
            {
                return s + v;
            }

            return v + s;
        }

        /// <summary>
        /// Read stream and encoding the bytes to valid string.
        /// </summary>
        /// <param name="stream">stream to be read</param>
        /// <param name="encoding">Retruned string endoding</param>
        /// <returns></returns>
        public static string ReadStream(this Stream stream, Encoding encoding)
        {
            var s = string.Empty;
            var c = 0;
            var i = 1024;
            var buffer = new byte[i];
            c = stream.Read(buffer, 0, buffer.Length);
            s = encoding.GetString(buffer, 0, c);

            while(s[s.Length -1] == '�')
            {
                var b = stream.ReadByte();
                if(b == -1)
                {
                    break;
                }

                var temp = new byte[++i];
                buffer.CopyTo(temp, 0);
                temp[i - 1] = (byte)b;
                buffer = temp;

                s = encoding.GetString(buffer, 0, i);
            }

            return s;
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
