using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class Converter : MonoBehaviour
{
    private string dir = Path.Combine("Assets", "Audio", "Songs");
    private List<float> timeTable = new List<float>();
    private List<int> drumLines = new List<int>();
    private List<(int, int)> syncLines = new List<(int, int)>();
    private List<int> drumTime = new List<int>();
    private string diffDrum;
    void Awake()
    {

        foreach (string path in Directory.GetDirectories(dir))
        {
            string difficulty = Path.GetFileName(path);
            diffDrum = $"[{difficulty}Drums]";
            foreach (string song in Directory.GetDirectories(path))
            {
                string chart = Path.Combine(song, "notes.chart");
                if (File.Exists(chart))
                {
                    ReadChartFile(chart);
                    File.Delete(chart);
                    createCSV(song);
                }

            }
        }
    }

    private void ReadChartFile(string filePath)
    {
        string line;
        string[] parts;
        float noBPM;
        drumTime.Add(0);
        //in song.ini there is the preview start time, i don't know what it does

        using (StreamReader sr = new StreamReader(filePath))
        {
            //find the resolution
            while ((line = sr.ReadLine()) != null && !line.StartsWith("  Resolution")) { }
            // i divide by 60 (seconds/minute) and by 1000 (because the bpm is *1000)
            noBPM = 60000f / float.Parse(line.Split(' ')[4]);

            //find the sync track
            while (sr.ReadLine() != "[SyncTrack]") { }
            //skip '{'
            sr.ReadLine();
            //read until the end of the sync track
            while ((line = sr.ReadLine()) != "}")
            {
                //parts[4] = letter, parts[2] = tick, parts[5] = bpm
                //there is also 'TS' but i don't know what it does
                if (line.Contains(" B ", StringComparison.Ordinal))
                {
                    parts = line.Split(' ');
                    //different bpm at different ticks
                    syncLines.Add((int.Parse(parts[2]), int.Parse(parts[5])));
                }
            }
            //find the drum track
            while (sr.ReadLine() != diffDrum) { }
            //skip '{'
            sr.ReadLine();
            //read until the end of the drum track
            while ((line = sr.ReadLine()) != "}")
            {
                parts = line.Split(' ');
                //parts[2] = tick, parts[4] = letter, parts[5] = int, parts[6] = int
                //there is also 'S' but i don't know what it does
                //there is also part[6] but i don't know what it does and it is always 0
                if (line.Contains(" N ", StringComparison.Ordinal))
                {
                    parts = line.Split(' ');
                    drumTime.Add(int.Parse(parts[2]));
                    //type of drum
                    drumLines.Add(int.Parse(parts[5]));
                }
            }
            //add end of song
            syncLines.Add((drumTime[drumTime.Count - 1], 0));
        }

        //not to efficent but i don't think it will be a problem
        createTimeTable(drumTime, syncLines, noBPM);
        syncLines.Clear();
        drumTime.Clear();
    }
    //create the time teable that will be used to wait the time between the notes
    private void createTimeTable(List<int> drumTime, List<(int, int)> syncLines, float noBPM)
    {
        int len1 = drumTime.Count - 1;
        int len2 = syncLines.Count;
        float totalTime;
        int prevTick = drumTime[0];
        int endTick;

        for (int i = 0; i < len1; i++)
        {
            endTick = drumTime[i + 1];
            totalTime = 0;
            for (int x = 0; x < len2; x++)
            {
                if (syncLines[x].Item1 > prevTick)
                {
                    if (syncLines[x].Item1 >= endTick)
                    {
                        totalTime += (endTick - prevTick) * (noBPM / syncLines[x - 1].Item2);
                        break;
                    }
                    totalTime += (syncLines[x].Item1 - prevTick) * (noBPM / syncLines[x - 1].Item2);
                    prevTick = syncLines[x].Item1;
                }
            }
            timeTable.Add(totalTime);
            prevTick = endTick;
        }
    }

    private void createCSV(string path)
    {
        path = Path.Combine(path, "time_notes.txt");
        using (StreamWriter sw = new StreamWriter(path))
        {
            int len = timeTable.Count;
            for (int i = 0; i < len; i++)
            {
                sw.WriteLine($"{timeTable[i]}/{drumLines[i]}");
            }
        }
    }

}
