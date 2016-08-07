﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mp4Parser;
using System.Net;
using System.Threading;

namespace mp4Parser
{
    public class Parser
    {
        public static void PrintHeader(string atomType, UInt32 size, UInt32 offset, int lvl)
        {
            string tab = "";
            for (int i = 0; i < lvl; i++)
            {
                tab += "\t";
            }
            Console.WriteLine(tab + "[" + atomType + ", size: " + size + ", offset: " + offset + "]");
        }

        public static string[] getTypes()
        {
            var types = new string[6];
            types[0] = "ftyp,moov,mdat";
            types[1] = "mvhd,trak,udta";
            types[2] = "tkhd,edts,mdia,meta,covr,©nam";
            types[3] = "mdhd,hdlr,minf";
            types[4] = "smhd,vmhd,dinf,stbl";
            types[5] = "stsd,stts,stss,ctts,stsc,stsz,stco";
            return types;
        }

        public void parserFunction(string path)
        {
            Console.WriteLine("Start");
            var types = getTypes();
            byte[] bytes = File.ReadAllBytes(path);
            UInt32 length = Convert.ToUInt32(bytes.Length);
            UInt32 offset = 0;

            while ((offset + 8) < length)
            {
                try
                {
                    UInt32 i = offset;
                    UInt32 atomSize = BitConverter.ToUInt32(new byte[] { bytes[i], bytes[++i], bytes[++i], bytes[++i] }.Reverse().ToArray(), 0);
                    string atomType = Encoding.ASCII.GetString(new byte[] { bytes[++i], bytes[++i], bytes[++i], bytes[++i] });
                    for (int lvl = 0; lvl < 6; lvl++)
                    {
                        if ((atomSize < length) && types[lvl].Contains(atomType))
                        {
                            PrintHeader(atomType, atomSize, offset, lvl);

                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                offset++;
            }
            Console.WriteLine("Parsing is finished");
        }

        public void fromUrlParserFunction(string path)
        {
            Console.WriteLine("Start");
            var types = getTypes();
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(path);
            UInt32 length = Convert.ToUInt32(wc.ResponseHeaders["Content-Length"]);
            var bytes = new byte[length];
            stream.Read(bytes, 0, Convert.ToInt32(length.ToString()));
            UInt32 offset = 0;
            while ((offset + 8) < length) { 
                try
                {
                    UInt32 i = offset;
                    UInt32 atomSize = BitConverter.ToUInt32(new byte[] { bytes[i], bytes[++i], bytes[++i], bytes[++i] }.Reverse().ToArray(), 0);
                    string atomType = Encoding.ASCII.GetString(new byte[] { bytes[++i], bytes[++i], bytes[++i], bytes[++i] });
                    for (int lvl = 0; lvl < 6; lvl++)
                    {
                        if ((atomSize < length) && types[lvl].Contains(atomType))
                        {
                            PrintHeader(atomType, atomSize, offset, lvl);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                offset++;
            }
            Console.WriteLine("Parsing is finished");
        }
    }
}
