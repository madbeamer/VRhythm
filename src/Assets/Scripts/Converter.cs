using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Globalization;
using System.Linq;

public class Converter : MonoBehaviour
{
    private string dir = Path.Combine("Assets", "Audio", "Songs");
    private List<float> timeTable = new List<float>();
    private List<int> drumLines = new List<int>();
    private List<(int, float)> syncLines = new List<(int, float)>();
    private List<int> drumTime = new List<int>();
    private string diffDrum;
    private Dictionary<int, int> Drums = new Dictionary<int, int>
    {
        //for more info: https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/.chart/Drums.md
        {0, 4},
        {1, 3},
        {2, 7},
        {3, 5},
        {4, 6}, //when 4 lanes?
        {5, 0},
        {32, 4},
        {34, 3},
        {35, 7},
        {36, 5},
        {37, 6},
        {38, 0},
        {40, 3},
        {41, 7},
        {42, 5},
        {43, 6},
        {44, 0},
        {66, 7},
        {67, 5},
        {68, 0}
    };

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
                    drumLines.Clear();
                    syncLines.Clear();
                    drumTime.Clear();
                    timeTable.Clear();
                }

            }
        }
    }

    private void ReadChartFile(string filePath)
    {
        string line;
        string[] parts;

        using (StreamReader sr = new StreamReader(filePath))
        {
            //find the resolution
            while ((line = sr.ReadLine()) != null && !line.StartsWith("  Resolution")) { }
            // i divide by 60 (seconds/minute) and by 1000 (because the bpm is *1000)
            //line.Split(' ')[4] = resolution
            float noBPM = 60000f / float.Parse(line.Split(' ')[4]);

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
                    syncLines.Add((int.Parse(parts[2]), noBPM / int.Parse(parts[5])));
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
                    drumLines.Add(Drums[int.Parse(parts[5])]);
                }
            }
        }

        //not to efficent but i don't think it will be a problem
        createTimeTable();
    }
    //create the time teable that will be used to wait the time between the notes
    private void createTimeTable()
    {
        syncLines.Add((int.MaxValue, 0.0f));
        int len = syncLines.Count;
        float prevSec = 0.0f;
        foreach (int tick in drumTime)
        {
            float totalTime = 0.0f;
            for (int i = 1; i < len; i++)
            {
                if (tick > syncLines[i].Item1)
                {
                    totalTime += (syncLines[i].Item1 - syncLines[i - 1].Item1) * syncLines[i - 1].Item2;
                }
                else
                {
                    totalTime += (tick - syncLines[i - 1].Item1) * syncLines[i - 1].Item2;
                    break;
                }
            }
            timeTable.Add(totalTime - prevSec);
            prevSec = totalTime;
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
                sw.WriteLine($"{timeTable[i].ToString(CultureInfo.InvariantCulture)}/{drumLines[i]}");
            }
        }
    }
}
