using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class NoteV1
{
    //小節数(実装見送り)
    //public int bar { get; set; }
    //拍数(実装見送り)
    //public int beat { get; set; }
    //192分単位(小数点以下対応)
    public double beat1_192 { get; set; }
    //微調整(s)
    public double adjust { get; set; }
    //ノーツ・イベント種類
    public short id { get; set; }
    //固有ID
    public int uniqueId { get; set; }
    //ノーツ・イベント引数
    public List<double> args { get; set; }

    public byte[] ToBytes()
    {
        byte[] ret = { };
        //ret.Concat(BitConverter.GetBytes(bar)).ToArray();
        //ret.Concat(BitConverter.GetBytes(beat)).ToArray();
        ret = ret.Concat(BitConverter.GetBytes(beat1_192)).ToArray();
        ret = ret.Concat(BitConverter.GetBytes(adjust)).ToArray();
        ret = ret.Concat(BitConverter.GetBytes(id)).ToArray();
        ret = ret.Concat(BitConverter.GetBytes(uniqueId)).ToArray();
        double[] args_ = args.ToArray();
        ret = ret.Concat(BitConverter.GetBytes(args_.Length)).ToArray();
        foreach (double arg in args_)
        {
            ret = ret.Concat(BitConverter.GetBytes(arg)).ToArray();
        }
        return ret;
    }

    //デフォルトコンストラクタ
    public NoteV1() { }
    //コンストラクタ
    public NoteV1(double _beat1_192, double _adjust, short _id, int _uniqueId, List<double> _args)
    {
        beat1_192 = _beat1_192;
        adjust = _adjust;
        id = _id;
        uniqueId = _uniqueId;
        args = _args;
    }
    //コピーコンストラクタ
    public NoteV1(NoteV1 _note)
    {
        beat1_192 = _note.beat1_192;
        adjust = _note.adjust;
        id = _note.id;
        uniqueId = _note.uniqueId;
        args = _note.args;
    }
}

public class ChartV1
{
    public short version { get; set; } = 1;
    public string title { get; set; } = "Title";
    public string artist { get; set; } = "Artist";
    public string notesDesigner { get; set; } = "ND";
    public string soundSource { get; set; } = "SoundSource";
    public int level { get; set; } = 0;
    public short levelType { get; set; } = 0;
    public double adjust { get; set; } = 0;
    public double startTime { get; set; } = 0;
    public double bpm { get; set; } = 180;
    public double measure { get; set; } = 4;
    public double metronome { get; set; } = 0;//メトロノームのタイミング(ms)、-1で無効化
    public bool measureLine { get; set; } = true;//小節線の有無
    public short bgType { get; set; } = 0;
    public List<NoteV1> notes { get; set; } = new List<NoteV1>();

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
        bytes = bytes.Concat(BitConverter.GetBytes(bpm)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(measure)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(metronome)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(measureLine)).ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(bgType)).ToArray();
        NoteV1[] notes_ = notes.ToArray();
        bytes = bytes.Concat(BitConverter.GetBytes(notes_.Length)).ToArray();
        foreach (NoteV1 note in notes_)
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
        if (version == 1)
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
            bpm = BitConverter.ToDouble(buffer, currentByte);
            currentByte += 8;
            measure = BitConverter.ToDouble(buffer, currentByte);
            currentByte += 8;
            metronome = BitConverter.ToDouble(buffer, currentByte);
            currentByte += 8;
            measureLine = BitConverter.ToBoolean(buffer, currentByte);
            currentByte += 1;
            bgType = BitConverter.ToInt16(buffer, currentByte);
            currentByte += 2;
            int notesLength = BitConverter.ToInt32(buffer, currentByte);
            currentByte += 4;
            for (int i = 0; i < notesLength; ++i)
            {
                /*
                int bar = BitConverter.ToInt32(buffer, currentByte);
                currentByte += 4;
                int beat = BitConverter.ToInt32(buffer, currentByte);
                currentByte += 4;
                */
                double beat1_192 = BitConverter.ToDouble(buffer, currentByte);
                currentByte += 8;
                double noteAdjust = BitConverter.ToDouble(buffer, currentByte);
                currentByte += 8;
                short id_ = BitConverter.ToInt16(buffer, currentByte);
                currentByte += 2;
                int uniqueId_ = BitConverter.ToInt32(buffer, currentByte);
                currentByte += 4;
                int argsLength = BitConverter.ToInt32(buffer, currentByte);
                currentByte += 4;
                List<double> args_ = new List<double>();
                for (int j = 0; j < argsLength; ++j)
                {
                    args_.Add(BitConverter.ToDouble(buffer, currentByte));
                    currentByte += 8;
                }
                notes.Add(new NoteV1(beat1_192, noteAdjust, id_, uniqueId_, args_));
            }
        }
    }
}

//Todo: ノーツをクリックした時の動作(select,Inspector)、LNなど

public class Test002 : MonoBehaviour
{
    public ChartV1 chart { get; private set; }
    GameObject noteGameObject;
    GameObject scoreBase;
    GameObject scoreEditor;
    RectTransform rectScoreEditor;
    GameObject snapGrid16;
    GameObject snapGridRoot;
    RectTransform rectSnapGridRoot;
    TMP_Dropdown dropdown;
    GameObject inspector;
    Test005 test005;
    ScrollRect scrollRect;
    GameObject snap12, snap16, snap24, snap32, snap48, snap64;
    GameObject LNtemp;
    RectTransform LNtempRect;
    Color BTColor, FXColor, BTLNColor, FXLNColor, LRColor, LRLColor;
    TMP_Text editorModeText;
    TMP_Text isLNText;

    public enum EditorMode
    {
        AddBT,
        AddFX,
        EraseBT,
        EraseFX,
        Select
    }

    public enum LNState
    {
        Begin,
        End
    }

    [SerializeField] EditorMode _editorMode;
    public EditorMode editorMode
    {
        get
        {
            return _editorMode;
        }
        set
        {
            _editorMode = value;
            editorModeText.text = _editorMode.ToString();
        }
    }

    [SerializeField] bool _isLN;
    public bool isLN
    {
        get
        {
            return _isLN;
        }
        set
        {
            _isLN = value;
            isLNText.text = _isLN.ToString();
        }
    }
    [SerializeField] LNState LNstate = LNState.Begin;

    int uniqueIdCount = 0;

    //とりあえず16分でyを24動かす感じで

    void Initialize()
    {
        chart = new ChartV1();
        editorMode = EditorMode.Select;
        isLN = false;
        uniqueIdCount = 0;
        foreach (Transform c in scoreBase.transform)
        {
            Destroy(c.gameObject);
        }
    }

    public void SelectNote(int _uniqueId)
    {
        test005.Note = chart.notes.Where(n => n.uniqueId == _uniqueId).First();
    }

    public void EraseNote(int _uniqueId)
    {
        var target = chart.notes.Where(n => n.uniqueId == _uniqueId).First();
        chart.notes.Remove(target);
    }

    public void ChangeLNMode()
    {
        isLN = !isLN;
    }

    public void SaveChart(string filename)
    {
        //var path = EditorUtility.SaveFilePanel("Save Chart", "", "MyChart", "n2rf");
        var path = Application.dataPath + "/" + filename +".n2rf";
        if (!string.IsNullOrEmpty(path))
        {
            chart.WriteChart(path);
        }
    }

    public void LoadChart(string filename)
    {
        //var path = EditorUtility.OpenFilePanel("Load Chart", "", "n2rf");
        var path = Application.dataPath + "/" + filename + ".n2rf";
        if(string.IsNullOrEmpty(path))
        {
            return;
        }

        chart = new ChartV1();
        //todo save&loadの改善、loadに関しては初期化とuniqueIdCountの反映が未実装
        Initialize();
        chart.ReadChart(path);
        
        foreach(var note in chart.notes)
        {
            uniqueIdCount = Math.Max(uniqueIdCount, note.uniqueId);
            var obj = Instantiate(noteGameObject, scoreBase.transform);
            obj.name = "NoteUI(" + note.uniqueId.ToString() + ")";
            obj.GetComponent<Test004>().uniqueId = note.uniqueId;
            if (101 <= note.id && note.id <= 104)
            {
                obj.transform.localPosition = new Vector3(70 * (note.id - 100), (float)note.beat1_192 * 2.0f, 0);
                obj.GetComponent<Image>().color = BTColor;
            }
            else if(113 <= note.id && note.id <= 114)
            {
                obj.transform.localPosition = new Vector3(350 * (note.id - 113), (float)note.beat1_192 * 2.0f, 0);
                obj.GetComponent<Image>().color = LRColor;
            }
            else if(109 <= note.id && note.id <= 110)
            {
                obj.transform.localPosition = new Vector3(70 + 140 * (note.id - 109), (float)note.beat1_192 * 2.0f, 0);
                var rect = obj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(140, rect.sizeDelta.y);
                obj.GetComponent<Image>().color = FXColor;
            }
            else if(105 <= note.id && note.id <= 108)
            {
                obj.transform.localPosition = new Vector3(70 * (note.id - 104), (float)note.beat1_192 * 2.0f, 0);
                var rect = obj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)note.args[0] * 2.0f);
                obj.GetComponent<Image>().color = BTLNColor;
            }
            else if(115 <= note.id && note.id <= 116)
            {
                obj.transform.localPosition = new Vector3(350 * (note.id - 115), (float)note.beat1_192 * 2.0f, 0);
                var rect = obj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)note.args[1] * 2.0f);
                obj.GetComponent<Image>().color = LRLColor;
            }
            else if(111 <= note.id && note.id <= 112)
            {
                obj.transform.localPosition = new Vector3(70 + (note.id - 111) * 140, (float)note.beat1_192 * 2.0f, 0);
                var rect = obj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(140, (float)note.args[0] * 2.0f);
                obj.GetComponent<Image>().color = FXLNColor;
            }
        }
        uniqueIdCount++;
    }

    public void ChangeEditorMode(int mode)
    {
        if (mode == 0)
        {
            if (editorMode == EditorMode.AddBT)
            {
                editorMode = EditorMode.AddFX;
            }
            else
            {
                editorMode = EditorMode.AddBT;
            }
        }
        else if (mode == 1)
        {
            if (editorMode != EditorMode.Select)
            {
                editorMode = EditorMode.Select;
            }
            else
            {
                editorMode = EditorMode.AddBT;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        snapGrid16 = (GameObject)Resources.Load("Prefabs/1_16SnapGrid");
        noteGameObject = (GameObject)Resources.Load("Prefabs/NoteUI");
        chart = new ChartV1();
        scoreBase = GameObject.Find("ScoreBase");
        dropdown = GameObject.Find("SnapSetting").GetComponent<TMP_Dropdown>();
        scoreEditor = GameObject.Find("ScoreEditor");
        rectScoreEditor = scoreEditor.GetComponent<RectTransform>();
        snapGridRoot = GameObject.Find("SnapGridRoot");
        rectSnapGridRoot = snapGridRoot.GetComponent<RectTransform>();
        snap12 = GameObject.Find("Snap12");
        snap16 = GameObject.Find("Snap16");
        snap24 = GameObject.Find("Snap24");
        snap32 = GameObject.Find("Snap32");
        snap48 = GameObject.Find("Snap48");
        snap64 = GameObject.Find("Snap64");
        inspector = GameObject.Find("Inspector");
        test005 = inspector.GetComponent<Test005>();
        scrollRect = GameObject.Find("ScoreEditor").GetComponent<ScrollRect>();
        editorModeText = GameObject.Find("EditorMode").GetComponent<TMP_Text>();
        isLNText = GameObject.Find("TapLN").GetComponent<TMP_Text>();

        editorMode = EditorMode.Select;
        isLN = false;

        BTColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        BTLNColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        FXColor = new Color(0.7f, 0.4f, 0.2f, 0.7f);
        FXLNColor = new Color(0.5f, 0.2f, 0.1f, 0.7f);
        LRColor = new Color(0.0f, 0.7f, 0.0f, 0.7f);
        LRLColor = new Color(0.0f, 0.4f, 0.0f, 0.7f);

        for (int y = -16; y <= rectSnapGridRoot.rect.height / 6; y++)
        {
            GameObject obj;
            GameObject obj12;

            if ((y + 16) % 16 == 0)
            {
                obj = Instantiate(snapGrid16, snap16.transform);
                obj12 = Instantiate(snapGrid16, snap12.transform);
                obj.GetComponent<Image>().color = new Color(1, 1, 1);
                obj12.GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else if ((y + 16) % 8 == 0)
            {
                obj = Instantiate(snapGrid16, snap16.transform);
                obj12 = Instantiate(snapGrid16, snap12.transform);
                obj.GetComponent<Image>().color = new Color(1, 0, 0);
                obj12.GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else if ((y + 16) % 4 == 0)
            {
                obj = Instantiate(snapGrid16, snap16.transform);
                obj12 = Instantiate(snapGrid16, snap12.transform);
                obj.GetComponent<Image>().color = new Color(0, 1, 0);
                obj12.GetComponent<Image>().color = new Color(0, 1, 0);
            }
            else if ((y + 16) % 2 == 0)
            {
                obj = Instantiate(snapGrid16, snap32.transform);
                obj12 = Instantiate(snapGrid16, snap24.transform);
                obj.GetComponent<Image>().color = new Color(1, 1, 0);
                obj12.GetComponent<Image>().color = new Color(1, 1, 0);
                //obj.SetActive(false);
            }
            else
            {
                obj = Instantiate(snapGrid16, snap64.transform);
                obj12 = Instantiate(snapGrid16, snap48.transform);
                obj.GetComponent<Image>().color = new Color(0, 1, 1);
                obj12.GetComponent<Image>().color = new Color(0, 1, 1);
                //obj.SetActive(false);
            }
            obj.transform.localPosition = new Vector3(0, y * 6 - rectSnapGridRoot.rect.height / 2, 0);
            obj12.transform.localPosition = new Vector3(0, y * 8 - rectSnapGridRoot.rect.height / 2, 0);
            snap32.SetActive(false);
            snap64.SetActive(false);
            snap12.SetActive(false);
            snap24.SetActive(false);
            snap48.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dropdown.value%2==0)
        {
            scrollRect.scrollSensitivity = 32.0f / Mathf.Pow(2, dropdown.value / 2);
        }
        else
        {
            scrollRect.scrollSensitivity = 24.0f / Mathf.Pow(2, dropdown.value / 2);
        }
        rectSnapGridRoot.transform.localPosition = new Vector3(0, scoreBase.transform.position.y - Mathf.Floor(scoreBase.transform.position.y / (scrollRect.scrollSensitivity * 4 * Mathf.Pow(2, dropdown.value / 2))) * (scrollRect.scrollSensitivity * 4 * Mathf.Pow(2, dropdown.value / 2)), 0);

        if(LNstate == LNState.End)
        {
            var tmp = Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity
                                - LNtemp.transform.localPosition.y;
            if(tmp>0)
            {
                LNtempRect.sizeDelta = new Vector2(LNtempRect.sizeDelta.x, tmp);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(220 <= Input.mousePosition.x && Input.mousePosition.x <= 640)
            {
                if (editorMode == EditorMode.AddBT)
                {
                    if(isLN)
                    {
                        if(LNstate == LNState.Begin)
                        {
                            LNtemp = Instantiate(noteGameObject, scoreBase.transform);
                            LNtemp.name = "NoteUI(" + uniqueIdCount.ToString() + ")";
                            LNtemp.transform.localPosition = new Vector3(Mathf.Floor((Input.mousePosition.x - 220.0f) / 70.0f) * 70,
                            Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity, 0);

                            LNtemp.GetComponent<Test004>().uniqueId = uniqueIdCount;
                            var x = LNtemp.transform.localPosition.x;
                            if(70 <= x && x < 350)
                            {
                                LNtemp.GetComponent<Image>().color = BTLNColor;
                            }
                            else
                            {
                                LNtemp.GetComponent<Image>().color = LRLColor;
                            }
                            LNtempRect = LNtemp.GetComponent<RectTransform>();
                            LNstate = LNState.End;
                        }
                        else
                        {
                            var tmp = Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity
                                - LNtemp.transform.localPosition.y;
                            if (tmp > 0)
                            {
                                LNtempRect.sizeDelta = new Vector2(LNtempRect.sizeDelta.x, tmp);
                                var x = (int)LNtemp.transform.localPosition.x;
                                if (70 <= x && x < 350)
                                {
                                    chart.notes.Add(new NoteV1(LNtemp.transform.localPosition.y / 2.0, 0.0, (short)(104 + x / 70), uniqueIdCount, new List<double>() { LNtempRect.rect.height / 2.0, 0.0 }));//BT-A,B,C,D
                                }
                                else
                                {
                                    chart.notes.Add(new NoteV1(LNtemp.transform.localPosition.y / 2.0, 0.0, (short)(115 + x / 350), uniqueIdCount, new List<double>() { 90, LNtempRect.rect.height / 2.0, 0.0 }));//TURN_L,R
                                }
                                uniqueIdCount++;
                            }
                            else
                            {
                                Destroy(LNtemp);
                            }
                            LNstate = LNState.Begin;
                        }
                    }
                    else
                    {
                        var obj = Instantiate(noteGameObject, scoreBase.transform);
                        obj.name = "NoteUI(" + uniqueIdCount.ToString() + ")";
                        obj.transform.localPosition = new Vector3(Mathf.Floor((Input.mousePosition.x - 220.0f) / 70.0f) * 70,
                            Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity, 0);

                        obj.GetComponent<Test004>().uniqueId = uniqueIdCount;
                        var x = (int)obj.transform.localPosition.x;
                        if (70 <= x && x < 350)
                        {
                            obj.GetComponent<Image>().color = BTColor;
                            chart.notes.Add(new NoteV1(obj.transform.localPosition.y / 2.0, 0.0, (short)(100 + x / 70), uniqueIdCount, new List<double>()));//BT-A,B,C,D
                        }
                        else
                        {
                            obj.GetComponent<Image>().color = LRColor;
                            chart.notes.Add(new NoteV1(obj.transform.localPosition.y / 2.0, 0.0, (short)(113 + x / 350), uniqueIdCount, new List<double>() { 90 }));//TURN_L,R
                        }
                        uniqueIdCount++;

                        var forDebug = chart.notes.Last();
                        Debug.Log("1/192: " + forDebug.beat1_192.ToString() +
                            ", adjust: " + forDebug.adjust.ToString() +
                            ", id: " + forDebug.id.ToString() +
                            ", unique_id: " + forDebug.uniqueId.ToString() +
                            ", args: " + forDebug.args.ToString());
                    }
                }
                else if(editorMode == EditorMode.AddFX)
                {
                    if (290 <= Input.mousePosition.x && Input.mousePosition.x <= 570)
                    {
                        if (isLN)
                        {
                            if (LNstate == LNState.Begin)
                            {
                                LNtemp = Instantiate(noteGameObject, scoreBase.transform);
                                LNtemp.name = "NoteUI(" + uniqueIdCount.ToString() + ")";
                                LNtemp.transform.localPosition = new Vector3(Mathf.Floor((Input.mousePosition.x - 290.0f) / 140.0f) * 140 + 70,
                                Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity, 0);

                                LNtemp.GetComponent<Test004>().uniqueId = uniqueIdCount;
                                LNtemp.GetComponent<Image>().color = FXLNColor;
                                LNtempRect = LNtemp.GetComponent<RectTransform>();
                                LNtempRect.sizeDelta = new Vector2(140, LNtempRect.sizeDelta.y);
                                LNstate = LNState.End;
                            }
                            else
                            {
                                var tmp = Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity
                                    - LNtemp.transform.localPosition.y;
                                if (tmp > 0)
                                {
                                    LNtempRect.sizeDelta = new Vector2(LNtempRect.sizeDelta.x, tmp);
                                    var x = (int)LNtemp.transform.localPosition.x;
                                    if (70 <= x && x < 210)
                                    {
                                        chart.notes.Add(new NoteV1(LNtemp.transform.localPosition.y / 2.0, 0.0, 112, uniqueIdCount, new List<double>() { LNtempRect.rect.height / 2.0, 0.0 }));//break
                                    }
                                    else
                                    {
                                        chart.notes.Add(new NoteV1(LNtemp.transform.localPosition.y / 2.0, 0.0, 111, uniqueIdCount, new List<double>() { LNtempRect.rect.height / 2.0, 0.0 }));//accel
                                    }
                                    uniqueIdCount++;
                                }
                                else
                                {
                                    Destroy(LNtemp);
                                }
                                LNstate = LNState.Begin;
                            }
                        }
                        else
                        {
                            var obj = Instantiate(noteGameObject, scoreBase.transform);
                            obj.name = "NoteUI(" + uniqueIdCount.ToString() + ")";
                            obj.transform.localPosition = new Vector3(Mathf.Floor((Input.mousePosition.x - 290.0f) / 140.0f) * 140 + 70,
                                Mathf.Floor((Input.mousePosition.y - scoreBase.transform.position.y) / scrollRect.scrollSensitivity) * scrollRect.scrollSensitivity, 0);

                            obj.GetComponent<Image>().color = FXColor;
                            obj.GetComponent<Test004>().uniqueId = uniqueIdCount;
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            rect.sizeDelta = new Vector2(140, rect.sizeDelta.y);
                            var x = (int)obj.transform.localPosition.x;
                            if (70 <= x && x < 210)
                            {
                                chart.notes.Add(new NoteV1(obj.transform.localPosition.y / 2.0, 0.0, 110, uniqueIdCount, new List<double>()));//break
                            }
                            else
                            {
                                chart.notes.Add(new NoteV1(obj.transform.localPosition.y / 2.0, 0.0, 109, uniqueIdCount, new List<double>()));//accel
                            }
                            uniqueIdCount++;

                            var forDebug = chart.notes.Last();
                            Debug.Log("1/192: " + forDebug.beat1_192.ToString() +
                                ", adjust: " + forDebug.adjust.ToString() +
                                ", id: " + forDebug.id.ToString() +
                                ", unique_id: " + forDebug.uniqueId.ToString() +
                                ", args: " + forDebug.args.ToString());
                        }
                    }
                }
            }
        }
    }
}
