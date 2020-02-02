using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJector
{
    class Program
    {
        static string InsertBeforeIndex(string src, string data, int index)
        {
            string p1 = src.Substring(0, index);
            string p2 = "\n" + data + "\n";
            string p3 = src.Substring(index);

            return p1+p2+p3;
        }
        static int GetLocOfData(string jData, string pathToFind)
        {
            bool ALL_GOOD = false;
            while (!ALL_GOOD)
            {
                int p1Loc = jData.IndexOf(pathToFind);
                int loc = p1Loc - 1;
                ALL_GOOD = true;
                while (loc > 0)
                {
                    string s = jData.Substring(loc, 1);

                    if (s == "\n")
                    {
                        Console.WriteLine("[V] - Found globals!");
                        return p1Loc;
                    }
                    if (s == "\t" || s == " ")
                    {
                        loc--;
                        continue;
                    }
                    ALL_GOOD = false;
                    break;
                }
                if (!ALL_GOOD)
                {
                    Console.WriteLine("[!] - This is the wrong globals - trying again");
                    jData = jData.Substring(0,p1Loc) + "|||||||" + jData.Substring(p1Loc + 7);
                }
            }
            Console.WriteLine("[X] - Could not find globals!");
            return -1;
        }
        static void InsertCP(string jFilePath, string outputFile)
        {
            string jData = File.ReadAllText(jFilePath);
            //step #1a - find the "globals" line. 
            int loc = GetLocOfData(jData, "globals");
            //step 1b - post the part1 of the cheatpack.
            string cp = File.ReadAllText("CheatPacks//jjp1.txt");
            jData = InsertBeforeIndex(jData, cp, loc + 8);
            
            //jData = jData.Substring(0, loc + 7) + "\n" + cp + jData.Substring(loc + 7);
            //step #2a - find "function main" 
            loc = GetLocOfData(jData, "function main takes nothing returns nothing");
            //step #2b - post part2 above "function main".
            cp = File.ReadAllText("CheatPacks//jjp2.txt");
            jData = InsertBeforeIndex(jData, cp, loc);
            //step #3a - find the last local variable of function main 
            loc = GetLocOfData(jData, "function main takes nothing returns nothing");
            string temp = jData.Substring(loc+43);
            int placeHolder = 0;
            int buffer = GetLocOfData(temp, "endfunction");
            temp = temp.Substring(0, buffer);
            while (temp.Length>10)
            {
                buffer = temp.IndexOf("local");
                if (buffer == -1)
                {
                    break;
                }
                buffer = temp.IndexOf("\n");
                placeHolder += buffer+1;
                temp = temp.Substring(buffer + 1);
            }
            loc += placeHolder + 43;
            //step #3b - post part3 beneath it. (assuming there are any locals)
            cp = File.ReadAllText("CheatPacks//jjp3.txt");
            jData = InsertBeforeIndex(jData, cp, loc);

            //step #4 - write it all to a new .j file
            File.WriteAllText(outputFile, jData);
        }

        static void CheatMap(string mapPath)
        {
            Console.WriteLine("[!] Now cheating: {0}", mapPath);
            //step #1 - open the map with MPQLib 
            int hMPQ = 0;
            bool status;
            //if (MPQLib.MPQCreateArchive(map, MPQLib.OPEN_EXISTING, 0,ref phMPQ))

            status = MPQLib.MPQCreateArchive(mapPath, MPQLib.OPEN_EXISTING, 0, ref hMPQ);
            //bool status = MPQLib.MPQOpenArchive(mapPath, 0, 0, ref hMPQ, true);
            if (!status)
            {
                Console.WriteLine("[X] - Could not open map {0}", mapPath);
                MPQLib.MPQCloseArchive(hMPQ);
                return;
            }
            //step #2 - extract the war3map.j file
            status = MPQLib.MPQExtractFile(hMPQ, "war3map.j", "temp//temp.j");
            if (!status)
            {
                status = MPQLib.MPQExtractFile(hMPQ, "scripts\\war3map.j", "temp//temp.j");
                if (!status)
                {
                    Console.WriteLine("[X] - Could not extract the war3map.j file from the map {0}", mapPath);
                    MPQLib.MPQCloseArchive(hMPQ);
                    return;
                }
            }
            //step #3 - add a cheatpack
            InsertCP("temp//temp.j", "temp//cheated.j");
            //step #4 - re-insert the edited war3map.j file.
            bool refMe = false;
            //if (   MPQLib.MPQAddFile(hMPQ, "temp//cheated.j", fileToAdd, MPQLib.FILE_COMPRESS_PKWARE, ref pbReplaced))
            status = MPQLib.MPQAddFile(hMPQ, "temp//cheated.j", "war3map.j", MPQLib.FILE_COMPRESS_PKWARE, ref refMe);
            if (!status)
            {
                Console.WriteLine("[X] - Could not add the war3map.j file to the map {0}", mapPath);
                MPQLib.MPQCloseArchive(hMPQ);
                return;
            }
            //step #5a - remove the temp files.
            File.Delete("temp//cheated.j");
            File.Delete("temp//temp.j");
            //step #5b OPT - compact flush?
            MPQLib.MPQCompactArchive(hMPQ, null, 0, 0);
            //step #6 - close the archive
            MPQLib.MPQCloseArchive(hMPQ);
            //step #7 - Move the temo file to the output folder.
            string cheatedMap = "output//" + mapPath.Substring(mapPath.IndexOf("temp//")+6);
            if (File.Exists(cheatedMap))
                File.Delete(cheatedMap);
            //mapPaht = "temp//name.w3x"

            File.Move(mapPath, cheatedMap);
        }
        static void Main(string[] args)
        {
            //input folder. *.w3x *.w3m
            //output folder. 

            //#TODO orgnise this later.
            Directory.CreateDirectory("temp");
            Directory.CreateDirectory("output");
            //step #1 - scan the input folder.

            DirectoryInfo di = new DirectoryInfo("input");
            FileInfo[] maps = di.GetFiles("*.w3x");
            foreach (FileInfo map in maps)
            {
                //make a copy of the map.
                string filePath = "temp//" + map.Name;
                File.Copy(map.FullName, filePath,true);
                //cheat the map.
                CheatMap(filePath);
            }
            Directory.Delete("temp");
            //#DEBUG - pause the program.
            Console.ReadKey();
        }
    }
}
