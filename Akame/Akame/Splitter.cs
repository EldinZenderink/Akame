using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using DirectShowLib.DES;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace Akame
{
    class Splitter
    {

        public Splitter(String file, int parts)
        {
            // TODO: Complete member initialization

            String name = fileName(file);

            double videoLength = getVideoLength(file);

            String timeCodes = timeCodeGenerator(videoLength, parts);

            Console.WriteLine("File: " + name);
            Console.WriteLine("Video Length: " + videoLength);
            Console.WriteLine("Timecodes: " + timeCodes);
            Console.WriteLine("Running splitter with arguments: ");
            Console.WriteLine("-o \"" + file + "\" --split \"timecodes:" + timeCodes + ".00\" \"" + file + "\"");



            ProcessStartInfo startInfo = new ProcessStartInfo(@"mkvtoolnix\mkvmerge.exe");

            if(!Directory.Exists("Parts")){
                Directory.CreateDirectory("Parts");
            }

            startInfo.Arguments = "-o \"Parts\\" + name + "\" --split \"timecodes:" + timeCodes + "\" \"" + file + "\"";
            Process.Start(startInfo);


        }

        private String fileName(String file)
        {
            String[] parts = file.Split('\\');

            return parts[parts.Length - 1];
        }

        private String getTimeStamp(double seconds)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", (int)(seconds / 3600), (int)((seconds / 60) % 60), (int)(seconds % 60)); 
        }

        private String timeCodeGenerator(double time, int parts)
        {


            String[] timeStamps = new String[parts];

            double firstPart = time / parts;

            for (int i = 1; i < (parts); i++)
            {

                timeStamps[i] = getTimeStamp(firstPart * i) + ".00";
                
            }

            String timeCodes = "";

            foreach (String timeCode in timeStamps)
            {
                timeCodes = timeCodes + "," + timeCode;
            }

            timeCodes = timeCodes.Substring(2);

            return timeCodes;

        }

        private double getVideoLength(String file) 
        {
            var mediaDet = (IMediaDet)new MediaDet();
            DsError.ThrowExceptionForHR(mediaDet.put_Filename(file));

            var type = Guid.Empty;
            for (int index = 0; index < 1000 && type != MediaType.Video; index++)
            {
                mediaDet.put_CurrentStream(index);
                mediaDet.get_StreamType(out type);
            }

            double frameRate;
            mediaDet.get_FrameRate(out frameRate);
            double mediaLength;
            mediaDet.get_StreamLength(out mediaLength);

            return mediaLength;


        }
    }

    

}
