using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Test005 : MonoBehaviour
{
    GameObject bpm;
    GameObject config;
    GameObject tap;
    GameObject turn;
    GameObject tapLN;
    GameObject turnLN;
    GameObject SVLD;

    Test002 test002;

    NoteV1 note;
    public NoteV1 Note
    {
        get
        {
            return note;
        }
        set
        {
            note = value;
            if ((101 <= note.id && note.id <= 104) || (109 <= note.id && note.id <= 110))
            {
                Selected = SelectedEvent.Tap;
            }
            else if(113 <= note.id && note.id <= 114)
            {
                Selected = SelectedEvent.Turn;
            }
            else if((105 <= note.id && note.id <= 108) || (111 <= note.id && note.id <= 112))
            {
                Selected = SelectedEvent.TapLN;
            }
            else if(115 <= note.id && note.id <= 116)
            {
                Selected = SelectedEvent.TurnLN;
            }
        }
    }

    public enum SelectedEvent
    {
        None,
        BPM,
        Config,
        Tap,
        Turn,
        TapLN,
        TurnLN,
        SVLD
    }

    [SerializeField] SelectedEvent selected;
    public SelectedEvent Selected
    {
        get
        {
            return selected;
        }
        private set
        {
            selected = value;
            if(selected == SelectedEvent.None)
            {
                bpm.SetActive(false);
                config.SetActive(false);
                tap.SetActive(false);
                turn.SetActive(false);
                tapLN.SetActive(false);
                turnLN.SetActive(false);
                SVLD.SetActive(false);
            }
            else if(selected == SelectedEvent.BPM)
            {
                bpm.SetActive(true);
                config.SetActive(false);
                tap.SetActive(false);
                turn.SetActive(false);
                tapLN.SetActive(false);
                turnLN.SetActive(false);
                SVLD.SetActive(false);
                bpm.transform.Find("BPM").GetComponent<TMP_InputField>().text = note.args[0].ToString();
                bpm.transform.Find("Timing").GetComponent<TMP_InputField>().text = note.beat1_192.ToString() + ":" + note.adjust.ToString();
            }
            else if(selected == SelectedEvent.Config)
            {
                bpm.SetActive(false);
                config.SetActive(true);
                tap.SetActive(false);
                turn.SetActive(false);
                tapLN.SetActive(false);
                turnLN.SetActive(false);
                SVLD.SetActive(false);
                config.transform.Find("BPM").GetComponent<TMP_InputField>().text = test002.chart.bpm.ToString();
                config.transform.Find("Beat").GetComponent<TMP_InputField>().text = test002.chart.measure.ToString();
                config.transform.Find("Metronome").GetComponent<TMP_InputField>().text = test002.chart.metronome.ToString();
                config.transform.Find("MeasureLine").GetComponent<TMP_Dropdown>().value = Convert.ToInt32(test002.chart.measureLine);
                config.transform.Find("Title").GetComponent<TMP_InputField>().text = test002.chart.title;
                config.transform.Find("Artist").GetComponent<TMP_InputField>().text = test002.chart.artist;
                config.transform.Find("ND").GetComponent<TMP_InputField>().text = test002.chart.notesDesigner;
                config.transform.Find("SoundSource").GetComponent<TMP_InputField>().text = test002.chart.soundSource;
                config.transform.Find("Level").GetComponent<TMP_InputField>().text = test002.chart.level.ToString();
                config.transform.Find("LevelType").GetComponent<TMP_Dropdown>().value = test002.chart.levelType;
                config.transform.Find("Adjust").GetComponent<TMP_InputField>().text = test002.chart.adjust.ToString();
                config.transform.Find("StartTime").GetComponent<TMP_InputField>().text = test002.chart.startTime.ToString();
                config.transform.Find("BGType").GetComponent<TMP_InputField>().text = test002.chart.bgType.ToString();
            }
            else if(selected == SelectedEvent.Tap)
            {
                bpm.SetActive(false);
                config.SetActive(false);
                tap.SetActive(true);
                turn.SetActive(false);
                tapLN.SetActive(false);
                turnLN.SetActive(false);
                SVLD.SetActive(false);
                tap.transform.Find("Timing").GetComponent<TMP_InputField>().text = note.beat1_192.ToString() + ":" + note.adjust.ToString();
                tap.transform.Find("UniqueID").GetComponent<TMP_InputField>().text = note.uniqueId.ToString();
            }
            else if(selected == SelectedEvent.Turn)
            {
                bpm.SetActive(false);
                config.SetActive(false);
                tap.SetActive(false);
                turn.SetActive(true);
                tapLN.SetActive(false);
                turnLN.SetActive(false);
                SVLD.SetActive(false);
                turn.transform.Find("Timing").GetComponent<TMP_InputField>().text = note.beat1_192.ToString() + ":" + note.adjust.ToString();
                turn.transform.Find("Angle").GetComponent<TMP_InputField>().text = note.args[0].ToString();
                turn.transform.Find("UniqueID").GetComponent<TMP_InputField>().text = note.uniqueId.ToString();
            }
            else if(selected == SelectedEvent.TapLN)
            {
                bpm.SetActive(false);
                config.SetActive(false);
                tap.SetActive(false);
                turn.SetActive(false);
                tapLN.SetActive(true);
                turnLN.SetActive(false);
                SVLD.SetActive(false);
                tapLN.transform.Find("Timing").GetComponent<TMP_InputField>().text = note.beat1_192.ToString() + ":" + note.adjust.ToString();
                tapLN.transform.Find("Duration").GetComponent<TMP_InputField>().text = note.args[0].ToString() + ":" + note.args[1].ToString();
                tapLN.transform.Find("UniqueID").GetComponent<TMP_InputField>().text = note.uniqueId.ToString();
            }
            else if(selected == SelectedEvent.TurnLN)
            {
                bpm.SetActive(false);
                config.SetActive(false);
                tap.SetActive(false);
                turn.SetActive(false);
                tapLN.SetActive(false);
                turnLN.SetActive(true);
                SVLD.SetActive(false);
                turnLN.transform.Find("Timing").GetComponent<TMP_InputField>().text = note.beat1_192.ToString() + ":" + note.adjust.ToString();
                turnLN.transform.Find("Duration").GetComponent<TMP_InputField>().text = note.args[1].ToString() + ":" + note.args[2].ToString();
                turnLN.transform.Find("Angle").GetComponent<TMP_InputField>().text = note.args[0].ToString();
                turnLN.transform.Find("UniqueID").GetComponent<TMP_InputField>().text = note.uniqueId.ToString();
            }
            else if(selected == SelectedEvent.SVLD)
            {
                bpm.SetActive(false);
                config.SetActive(false);
                tap.SetActive(false);
                turn.SetActive(false);
                tapLN.SetActive(false);
                turnLN.SetActive(false);
                SVLD.SetActive(true);
            }
        }
    }

    public void EnterConfig()
    {
        Selected = SelectedEvent.Config;
    }

    public void SVLDReady()
    {
        Selected = SelectedEvent.SVLD;
    }

    public void SVLDChart(bool sv)
    {
        if(sv)
        {
            test002.SaveChart(SVLD.transform.Find("Filename").GetComponent<TMP_InputField>().text);
        }
        else
        {
            test002.LoadChart(SVLD.transform.Find("Filename").GetComponent<TMP_InputField>().text);
        }
        Selected = SelectedEvent.None;
    }

    public void ApplyConfig()
    {
        if(Selected == SelectedEvent.BPM)
        {
            /*
            string[] _timing = bpm.transform.Find("Timing").GetComponent<TMP_InputField>().text.Split(':');
            double _beat1_192 = Note.beat1_192;
            double _adjust = Note.adjust;
            double _bpm = Note.args[0];
            try
            {
                _beat1_192 = Convert.ToDouble(_timing[0]);
                _adjust = Convert.ToDouble(_timing[1]);
                _bpm = Convert.ToDouble(bpm.transform.Find("BPM").GetComponent<TMP_InputField>().text);
            }
            catch (System.Exception)
            {
                Debug.LogError("Invalid Input.");
                return;
            }
            Note.beat1_192 = _beat1_192;
            Note.adjust = _adjust;
            Note.args[0] = _bpm;
            */
            //‚¢‚Á‚½‚ñƒpƒX‚Å
        }
        else if(selected == SelectedEvent.Tap)
        {
            string[] _timing = tap.transform.Find("Timing").GetComponent<TMP_InputField>().text.Split(':');
            double _beat1_192 = Note.beat1_192;
            double _adjust = Note.adjust;

            try
            {
                _beat1_192 = double.Parse(_timing[0]);
                _adjust = double.Parse(_timing[1]);
            }
            catch(System.Exception)
            {
                Debug.LogError("Invalid Input.");
                return;
            }

            Note.beat1_192 = _beat1_192;
            Note.adjust = _adjust;

            var obj = GameObject.Find("NoteUI(" + Note.uniqueId.ToString() + ")");
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
                (float)note.beat1_192 * 2, obj.transform.localPosition.z);
        }
        else if(selected == SelectedEvent.Turn)
        {
            string[] _timing = turn.transform.Find("Timing").GetComponent<TMP_InputField>().text.Split(':');
            string _angleStr = turn.transform.Find("Angle").GetComponent<TMP_InputField>().text;
            double _beat1_192 = Note.beat1_192;
            double _adjust = Note.adjust;
            double _angle = Note.args[0];

            try
            {
                _beat1_192 = double.Parse(_timing[0]);
                _adjust = double.Parse(_timing[1]);
                _angle = double.Parse(_angleStr);
            }
            catch (System.Exception)
            {
                Debug.LogError("Invalid Input.");
                return;
            }

            Note.beat1_192 = _beat1_192;
            Note.adjust = _adjust;
            Note.args[0] = _angle;

            var obj = GameObject.Find("NoteUI(" + Note.uniqueId.ToString() + ")");
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
                (float)note.beat1_192 * 2, obj.transform.localPosition.z);
        }
        else if(selected == SelectedEvent.TapLN)
        {
            string[] _timing = tapLN.transform.Find("Timing").GetComponent<TMP_InputField>().text.Split(':');
            string[] _duration = tapLN.transform.Find("Duration").GetComponent<TMP_InputField>().text.Split(':');
            double _beat1_192 = Note.beat1_192;
            double _adjust = Note.adjust;
            double _dur1_192 = Note.args[0];
            double _duradj = Note.args[1];

            try
            {
                _beat1_192 = double.Parse(_timing[0]);
                _adjust = double.Parse(_timing[1]);
                _dur1_192 = double.Parse(_duration[0]);
                _duradj = double.Parse(_duration[1]);
                if (_dur1_192 <= 0)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Invalid Input.");
                return;
            }

            Note.beat1_192 = _beat1_192;
            Note.adjust = _adjust;
            Note.args[0] = _dur1_192;
            Note.args[1] = _duradj;

            var obj = GameObject.Find("NoteUI(" + Note.uniqueId.ToString() + ")");
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
                (float)note.beat1_192 * 2, obj.transform.localPosition.z);
            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)Note.args[0] * 2.0f);
        }
        else if(selected == SelectedEvent.TurnLN)
        {
            string[] _timing = turnLN.transform.Find("Timing").GetComponent<TMP_InputField>().text.Split(':');
            string[] _duration = turnLN.transform.Find("Duration").GetComponent<TMP_InputField>().text.Split(':');
            string _angleStr = turnLN.transform.Find("Angle").GetComponent<TMP_InputField>().text;
            double _beat1_192 = Note.beat1_192;
            double _adjust = Note.adjust;
            double _angle = Note.args[0];
            double _dur1_192 = Note.args[1];
            double _duradj = Note.args[2];

            try
            {
                _beat1_192 = double.Parse(_timing[0]);
                _adjust = double.Parse(_timing[1]);
                _angle = double.Parse(_angleStr);
                _dur1_192 = double.Parse(_duration[0]);
                _duradj = double.Parse(_duration[1]);

                if (_dur1_192 <= 0)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Invalid Input.");
                return;
            }

            Note.beat1_192 = _beat1_192;
            Note.adjust = _adjust;
            Note.args[0] = _angle;
            Note.args[1] = _dur1_192;
            Note.args[2] = _duradj;

            var obj = GameObject.Find("NoteUI(" + Note.uniqueId.ToString() + ")");
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
                (float)note.beat1_192 * 2, obj.transform.localPosition.z);

            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)Note.args[1] * 2.0f);
        }
        else if(Selected == SelectedEvent.Config)
        {
            string _title = config.transform.Find("Title").GetComponent<TMP_InputField>().text;
            string _artist = config.transform.Find("Artist").GetComponent<TMP_InputField>().text;
            string _notesDesigner = config.transform.Find("ND").GetComponent<TMP_InputField>().text;
            string _soundSource = config.transform.Find("SoundSource").GetComponent<TMP_InputField>().text;
            string _lvStr = config.transform.Find("Level").GetComponent<TMP_InputField>().text;
            short _lvType = (short)config.transform.Find("LevelType").GetComponent<TMP_Dropdown>().value;
            string _adjStr = config.transform.Find("Adjust").GetComponent<TMP_InputField>().text;
            string _startTimeStr = config.transform.Find("StartTime").GetComponent<TMP_InputField>().text;
            string _bgTypeStr = config.transform.Find("BGType").GetComponent<TMP_InputField>().text;
            string _bpmStr = config.transform.Find("BPM").GetComponent<TMP_InputField>().text;
            string _measureStr = config.transform.Find("Beat").GetComponent<TMP_InputField>().text;
            string _metroStr = config.transform.Find("Metronome").GetComponent<TMP_InputField>().text;
            bool _measureLine = Convert.ToBoolean(config.transform.Find("MeasureLine").GetComponent<TMP_Dropdown>().value);

            int _lv;
            double _adj, _startTime, _bpm, _measure, _metro;
            short _bgType;

            try
            {
                _lv = int.Parse(_lvStr);
                _adj = double.Parse(_adjStr);
                _startTime = double.Parse(_startTimeStr);
                _bpm = double.Parse(_bpmStr);
                _measure = double.Parse(_measureStr);
                _metro = double.Parse(_metroStr);
                _bgType = short.Parse(_bgTypeStr);
            }
            catch(System.Exception)
            {
                Debug.LogError("Invalid Input.");
                return;
            }

            test002.chart.title = _title;
            test002.chart.artist = _artist;
            test002.chart.notesDesigner = _notesDesigner;
            test002.chart.soundSource = _soundSource;
            test002.chart.level = _lv;
            test002.chart.levelType = _lvType;
            test002.chart.adjust = _adj;
            test002.chart.startTime = _startTime;
            test002.chart.bgType = _bgType;
            test002.chart.bpm = _bpm;
            test002.chart.measure = _measure;
            test002.chart.metronome = _metro;
            test002.chart.measureLine = _measureLine;
        }
    }

    void CancelConfig()
    {
        if (Selected != SelectedEvent.None && Selected != SelectedEvent.Config)
        {
            Note = Note;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bpm = GameObject.Find("EditorUI/Inspector/BPM");
        config = GameObject.Find("EditorUI/Inspector/Config");
        tap = GameObject.Find("EditorUI/Inspector/TapConfig");
        turn = GameObject.Find("EditorUI/Inspector/TurnConfig");
        tapLN = GameObject.Find("EditorUI/Inspector/LNConfig");
        turnLN = GameObject.Find("EditorUI/Inspector/LongTurnConfig");
        SVLD = GameObject.Find("EditorUI/Inspector/SVLD");
        Selected = SelectedEvent.None;
        test002 = GameObject.Find("EditorCore").GetComponent<Test002>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
