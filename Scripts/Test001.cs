using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//ñvÉfÅ[É^
public class Note
{
    public double timing { get; set; }
    public short id { get; set; }
    public short uniqueId { get; set; }
    public List<double> args { get; set; }
    
    public byte[] ToBytes()
    {
        byte[] ret = { };
        ret.Concat(BitConverter.GetBytes(timing)).ToArray();
        ret.Concat(BitConverter.GetBytes(id)).ToArray();
        ret.Concat(BitConverter.GetBytes(uniqueId)).ToArray();
        double[] args_ = args.ToArray();
        ret.Concat(BitConverter.GetBytes(args_.Length)).ToArray();
        foreach(double arg in args_)
        {
            ret.Concat(BitConverter.GetBytes(arg)).ToArray();
        }
        return ret;
    }

    public Note() { }
    public Note(double timing_, short id_, short uniqueId_, List<double> args_)
    {
        timing = timing_;
        id = id_;
        uniqueId = uniqueId_;
        args = args_;
    }
}

public class Chart
{
    public short version { get; set; } = 0;
    public string title { get; set; } = "Title";
    public string artist { get; set; } = "Artist";
    public string notesDesigner { get; set; } = "ND";
    public string soundSource { get; set; } = "SoundSource";
    public int level { get; set; } = 0;
    public short levelType { get; set; } = 0;
    public double adjust { get; set; } = 0;
    public double startTime { get; set; } = 0;
    public short bgType { get; set; } = 0;
    public List<Note> notes { get; set; } = new List<Note>();

    public void WriteChart(string filePath)
    {
        byte[] bytes = { };
        bytes = bytes.Concat(BitConverter.GetBytes(version)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(title.Length)).ToArray();
        bytes = bytes.Concat(Encoding.Unicode.GetBytes(title)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(artist.Length)).ToArray();
        bytes = bytes.Concat(Encoding.Unicode.GetBytes(artist)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(notesDesigner.Length)).ToArray();
        bytes = bytes.Concat(Encoding.Unicode.GetBytes(notesDesigner)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(soundSource.Length)).ToArray();
        bytes = bytes.Concat(Encoding.Unicode.GetBytes(soundSource)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(level)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(levelType)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(adjust)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(startTime)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(bgType)).ToArray();
        Note[] notes_ = notes.ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(notes_.Length)).ToArray();
        foreach (Note note in notes_)
        {
            bytes = bytes.Concat(note.ToBytes()).ToArray();
        }

        File.WriteAllBytes(filePath, bytes);
    }

    public void ReadChart(string filePath)
    {
        byte[] buffer = File.ReadAllBytes(filePath);
        int currentByte = 0;
        version = BitConverter.ToInt16(buffer, currentByte);
        currentByte += 2;
        if (version == 0)
        {
            int titleLength = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            title = Encoding.Unicode.GetString(buffer, currentByte, 2 * titleLength);
            currentByte += 2 * titleLength;
            int artistLength = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            artist = Encoding.Unicode.GetString(buffer, currentByte, 2 * artistLength);
            currentByte += 2 * artistLength;
            int ndLength = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            notesDesigner = Encoding.Unicode.GetString(buffer, currentByte, 2 * ndLength);
            currentByte += 2 * ndLength;
            int ssLength = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            soundSource = Encoding.Unicode.GetString(buffer, currentByte, 2 * ssLength);
            currentByte += 2 * ssLength;
            level = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            levelType = BitConverter.ToInt16(buffer, currentByte);
            currentByte += 2;
            adjust = BitConverter.ToDouble(buffer, currentByte);
            currentByte += 8;
            startTime = BitConverter.ToDouble(buffer, currentByte);
            currentByte += 8;
            bgType = BitConverter.ToInt16(buffer, currentByte);
            currentByte += 2;
            int notesLength = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            for (int i = 0; i < notesLength; ++i)
            {
                double timing_ = BitConverter.ToDouble(buffer, currentByte);
                currentByte += 8;
                short id_ = BitConverter.ToInt16(buffer, currentByte);
                currentByte += 2;
                short uniqueId_ = BitConverter.ToInt16(buffer, currentByte);
                currentByte += 2;
                int argsLength = BitConverter.ToInt32(buffer, currentByte);
                currentByte += 4;
                List<double> args_ = new List<double>();
                for (int j = 0; j < argsLength; ++j)
                {
                    args_.Add(BitConverter.ToDouble(buffer, currentByte));
                    currentByte += 8;
                }
                notes.Add(new Note(timing_, id_, uniqueId_, args_));
            }
        }
    }
}

public class Test001 : MonoBehaviour
{

    GameObject noteGameObject;
    TextMeshProUGUI bpmText;
    TextMeshProUGUI scoreText;
    Chart chart;
    GameObject cam;

    [SerializeField] float speed = 180.0f;
    [SerializeField] float hiSpeed
    {
        get
        {
            return hiSpeed;
        }
        set
        {
            hiSpeed = value;
            speed = (float)bpm * scrollSpeed * hiSpeed;
        }
    }
    [SerializeField] float scrollSpeed
    {
        get
        {
            return scrollSpeed;
        }
        set
        {
            scrollSpeed = value;
            speed = (float)bpm * scrollSpeed * hiSpeed;
        }
    }
    [SerializeField] double bpm
    {
        get
        {
            return bpm;
        }
        set
        {
            bpm = value;
            speed = (float)bpm * scrollSpeed * hiSpeed;
        }
    }
    [SerializeField] int measureTop = 4;
    [SerializeField] int measureBottom = 4;
    [SerializeField] double score = 0.0;

    Vector3 direction = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        chart = new Chart();
        chart.ReadChart("C:/Users/N1rat/New_Project_20220623/a.bin");
        //chart.WriteChart("C:/Users/N1rat/New_Project_20220623/a.bin");
        noteGameObject = (GameObject)Resources.Load("prefabs/Cube");
        bpmText = GameObject.Find("BPM").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("SCORE").GetComponent<TextMeshProUGUI>();
        cam = GameObject.Find("Main Camera");

        Vector3 currentPos = Vector3.zero;
        double currentTime = 0.0;
        Quaternion currentAngle = Quaternion.Euler(Vector3.zero);

        foreach (var ev in chart.notes)
        {
            if(ev.timing!=-1)
            {
                float delta = (float)(ev.timing - currentTime);
                currentPos += Vector3.Scale(direction, new Vector3(delta * speed, delta * speed, delta * speed));
                currentTime = ev.timing;
            }
            if(ev.id == 1)
            {
                bpm = ev.args[0];
            }
            if(ev.id==2)
            {
                measureTop = (int)ev.args[0];
                measureBottom = (int)ev.args[1];
            }
            if(ev.id==101)
            {
                Instantiate(noteGameObject, currentPos, currentAngle);
            }
            if(ev.id==114)
            {
                direction = Quaternion.Euler(0, (float)ev.args[0], 0) * direction;
                currentAngle *= Quaternion.Euler(0, (float)ev.args[0], 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position += Vector3.forward * (speed * Time.deltaTime);
        scoreText.text = "SCORE: " + score.ToString("0000000");
        bpmText.text = "BPM: " + bpm.ToString("0");
    }
}
