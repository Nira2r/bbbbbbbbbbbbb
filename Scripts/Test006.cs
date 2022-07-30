using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class LaneInfo
{
    public double timing { get; private set; } = 0.0;
    public Quaternion angle { get; private set; } = Quaternion.Euler(Vector3.zero);
    public Vector3 pivot { get; private set; } = Vector3.zero;
    public Vector3 direction { get; private set; } = Vector3.forward;
    public Vector3 normal { get; private set; } = Vector3.right;

    public LaneInfo() { }
    public LaneInfo(double _timing, Quaternion _angle, Vector3 _pivot)
    {
        timing = _timing;
        angle = _angle;
        pivot = _pivot;
        direction = angle * direction;
        normal = angle * normal;
    }
}


[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class Test006 : MonoBehaviour
{

    ChartV1 chart;
    GameObject cam;
    Mesh mesh;
    MeshFilter meshFilter;
    float speed;// 0.50 * scrspd (* hisp) ���Ƃ�beat1/192�������邾��
    float width;//���[���̕�(2���[����)
    float resolution = 5.0f;//LongTurn�̕���\(deg)

    GameObject tapGameObject;
    GameObject fxGameObject;

    List<LaneInfo> laneInfos;

    // Start is called before the first frame update
    void Start()
    {
        laneInfos = new List<LaneInfo>();
        tapGameObject = (GameObject)Resources.Load("Prefabs/TapPivot");
        fxGameObject = (GameObject)Resources.Load("Prefabs/FXTapPivot");
        chart = new ChartV1();
        chart.ReadChart(Application.dataPath + "/test.n2rf");
        cam = GameObject.Find("Main Camera");

        Vector3 currentPos = Vector3.zero;
        double currentBeat = 0.0;
        Vector3 directon = Vector3.forward;
        Vector3 normal = Vector3.right;
        Quaternion currentAngle = Quaternion.Euler(Vector3.zero);


        var evs = chart.notes.OrderBy(x => x.beat1_192);
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int confirmedVerts = 0;

        speed = 0.5f * 0.01f;
        width = 25.0f * 0.01f;
        laneInfos.Add(new LaneInfo(0, currentAngle, currentPos));

        //Todo: �Ƃ肠�������[�����������A�m�[�c�z�u�A�J�����̋���(�}�b�s���O)�A�e�N�X�`���A���ʂ̏I�[����

        var endBeat = 0.0;

        //���[�������A���[�����擾
        foreach(var ev in evs)
        {

            if ((101 <= ev.id && ev.id <= 104) || (109 <= ev.id && ev.id <= 110) || (113 <= ev.id && ev.id <= 114))
            {
                endBeat = Math.Max(endBeat, ev.beat1_192);
            }
            else if ((105 <= ev.id && ev.id <= 108) || (111 <= ev.id && ev.id <= 112))
            {
                endBeat = Math.Max(endBeat, ev.beat1_192 + ev.args[0]);
            }
            else if (115 <= ev.id && ev.id <= 116)
            {
                endBeat = Math.Max(endBeat, ev.beat1_192 + ev.args[1]);
            }

            //�E�J�[�u(Tap)
            if (ev.id == 114)
            {
                //��������(�J�[�u�O)���m�肳����
                vertices.Add(currentPos + width * normal);
                vertices.Add(currentPos - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon + width * normal);
                triangles.AddRange(new int[] {
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts,confirmedVerts+2,confirmedVerts+3 });
                confirmedVerts += 4;

                currentPos += speed * (float)(ev.beat1_192 - currentBeat) * directon;
                laneInfos.Add(new LaneInfo(ev.beat1_192, currentAngle, currentPos));

                var dirAfter = Quaternion.Euler(0, (float)ev.args[0], 0) * directon;
                var normAfter = Quaternion.Euler(0, (float)ev.args[0], 0) * normal;

                //�p�̈ʒu�x�N�g�������߂�

                var t = 2 * width * Vector3.Magnitude(normal - normAfter) / Vector3.Magnitude(directon + dirAfter);
                vertices.Add(currentPos + width * normal);
                vertices.Add(currentPos - width * normal);
                vertices.Add(currentPos - width * normal + t * directon);
                vertices.Add(currentPos + width * normal - 2 * width * normAfter);
                triangles.AddRange(new int[]{
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts,confirmedVerts+2,confirmedVerts+3 });
                confirmedVerts += 4;

                currentPos += width * normal - width * normAfter;
                currentBeat = ev.beat1_192;
                directon = dirAfter;
                normal = normAfter;
                currentAngle *= Quaternion.Euler(0, (float)ev.args[0], 0);
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
            }
            else if(ev.id == 113)//���J�[�u(tap)
            {
                //��������(�J�[�u�O)���m�肳����
                vertices.Add(currentPos + width * normal);
                vertices.Add(currentPos - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon + width * normal);
                triangles.AddRange(new int[] {
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts,confirmedVerts+2,confirmedVerts+3 });
                confirmedVerts += 4;

                currentPos += speed * (float)(ev.beat1_192 - currentBeat) * directon;
                laneInfos.Add(new LaneInfo(ev.beat1_192, currentAngle, currentPos));

                var dirAfter = Quaternion.Euler(0, (float)-ev.args[0], 0) * directon;
                var normAfter = Quaternion.Euler(0, (float)-ev.args[0], 0) * normal;

                //�p�̈ʒu�x�N�g�������߂�

                var t = 2 * width * Vector3.Magnitude(normAfter - normal) / Vector3.Magnitude(directon + dirAfter);
                vertices.Add(currentPos + width * normal);
                vertices.Add(currentPos - width * normal);
                vertices.Add(currentPos + width * normal + t * directon);
                vertices.Add(currentPos - width * normal + 2 * width * normAfter);
                triangles.AddRange(new int[]{
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts+2,confirmedVerts+1,confirmedVerts+3 });
                confirmedVerts += 4;

                currentPos += width * normAfter - width * normal;
                currentBeat = ev.beat1_192;
                directon = dirAfter;
                normal = normAfter;
                currentAngle *= Quaternion.Euler(0, (float)-ev.args[0], 0);
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
            }
            else if(ev.id==116)//�E�J�[�u(LN)
            {
                var angleLeft = (float)ev.args[0];
                var duration = ev.args[1];//LN�̒���(1/192beat)
                var r = (float)(speed * duration / (angleLeft * Mathf.Deg2Rad));//�����̔��a

                //�܂��͒����������m�肳����
                vertices.Add(currentPos + width * normal);
                vertices.Add(currentPos - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon + width * normal);
                triangles.AddRange(new int[] {
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts,confirmedVerts+2,confirmedVerts+3 });
                confirmedVerts += 4;

                currentPos += speed * (float)(ev.beat1_192 - currentBeat) * directon;
                currentBeat = ev.beat1_192;
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));

                //angleLeft������\�������܂ŕ���\�������Ă�
                
                while (angleLeft >= resolution)
                {
                    var dirAfter = Quaternion.Euler(0, resolution, 0) * directon;
                    var normAfter = Quaternion.Euler(0, resolution, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        - 2 * width * normAfter);//�O�����_
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon);//�������_

                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;

                    currentPos += (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        - width * normAfter;//currentPos�͏�Ƀ��[���̒���
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, resolution, 0);
                    angleLeft -= resolution;
                    currentBeat += ev.args[1] * resolution / ev.args[0];
                    laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
                }
                //���܂�̏���
                if(angleLeft>0)
                {
                    var dirAfter = Quaternion.Euler(0, angleLeft, 0) * directon;
                    var normAfter = Quaternion.Euler(0, angleLeft, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        - 2 * width * normAfter);//�O�����_
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon);//�������_
                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;
                    currentPos += (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        - width * normAfter;//currentPos�͏�Ƀ��[���̒���
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, angleLeft, 0);
                }
                currentBeat = ev.beat1_192 + ev.args[1];//LN�̏I�_�܂Ő��������̂ł�����currentBeat��u��
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
            }
            else if (ev.id == 115)//���J�[�u(LN)
            {
                var angleLeft = (float)ev.args[0];
                var duration = ev.args[1];//LN�̒���(1/192beat)
                var r = (float)(speed * duration / (angleLeft * Mathf.Deg2Rad));//�����̔��a

                //�܂��͒����������m�肳����
                vertices.Add(currentPos + width * normal);
                vertices.Add(currentPos - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon - width * normal);
                vertices.Add(currentPos + speed * (float)(ev.beat1_192 - currentBeat) * directon + width * normal);
                triangles.AddRange(new int[] {
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts,confirmedVerts+2,confirmedVerts+3 });
                confirmedVerts += 4;

                currentPos += speed * (float)(ev.beat1_192 - currentBeat) * directon;
                currentBeat = ev.beat1_192;
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));

                //angleLeft������\�������܂ŕ���\�������Ă�
                while (angleLeft >= resolution)
                {
                    var dirAfter = Quaternion.Euler(0, -resolution, 0) * directon;
                    var normAfter = Quaternion.Euler(0, -resolution, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon);//�������_
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        + 2 * width * normAfter);//�O�����_

                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;

                    currentPos += -(width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        + width * normAfter;//currentPos�͏�Ƀ��[���̒���
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, -resolution, 0);
                    angleLeft -= resolution;
                    currentBeat += ev.args[1] * resolution / ev.args[0];
                    laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
                }
                //���܂�̏���
                if (angleLeft > 0)
                {
                    var dirAfter = Quaternion.Euler(0, -angleLeft, 0) * directon;
                    var normAfter = Quaternion.Euler(0, -angleLeft, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon);//�������_
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        + 2 * width * normAfter);//�O�����_
                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;
                    currentPos += -(width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        + width * normAfter;//currentPos�͏�Ƀ��[���̒���
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, -angleLeft, 0);
                }
                currentBeat = ev.beat1_192 + ev.args[1];//LN�̏I�_�܂Ő��������̂ł�����currentBeat��u��
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
            }
        }

        //�I�[����
        endBeat += 192.0;//1���ߐ���
        vertices.Add(currentPos + width * normal);
        vertices.Add(currentPos - width * normal);
        vertices.Add(currentPos + speed * (float)(endBeat - currentBeat) * directon - width * normal);
        vertices.Add(currentPos + speed * (float)(endBeat - currentBeat) * directon + width * normal);
        triangles.AddRange(new int[] {
                    confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                    confirmedVerts,confirmedVerts+2,confirmedVerts+3 });
        confirmedVerts += 4;

        currentPos += speed * (float)(endBeat - currentBeat) * directon;
        laneInfos.Add(new LaneInfo(endBeat, currentAngle, currentPos));

        //LaneInfo�̂ǂ̋�Ԃɂ��邩(idx)
        int currentLane = -1;

        float[] dx = new float[] { -0.1875f, -0.064f, 0.064f, 0.1875f };//delta local X�p

        //���[���g�z�u
        {
            float[] thickness = new float[] { 0.0025f, 0.0035f, 0.0080f, 0.0035f, 0.0025f };
            float[] _dx = new float[5];
            for (int i = 0; i < 5; ++i)//������i-1�{�ڂ̐�
            {
                if (i == 0)
                {
                    _dx[i] = -width + thickness[i] * 0.5f;
                }
                else
                {
                    _dx[i] = _dx[i - 1] + 0.12f + (thickness[i - 1] + thickness[i]) / 2;
                }
                var obj = new GameObject("LaneLine(" + i.ToString() + ")");
                obj.transform.parent = this.transform;
                var _meshFilter = obj.AddComponent<MeshFilter>();
                var _meshRenderer = obj.AddComponent<MeshRenderer>();
                Mesh _mesh = new Mesh();
                List<Vector3> _vertices = new List<Vector3>();
                List<int> _triangles = new List<int>();
                var _confirmedVerts = 0;
                var _dy = new Vector3(0, 0.0001f, 0);
                for(int j=0;j<laneInfos.Count-1;++j)//j�Ԗڂ̋��
                {
                    _vertices.Add(laneInfos[j].pivot + (_dx[i] + thickness[i] * 0.5f) * laneInfos[j].normal + _dy);
                    _vertices.Add(laneInfos[j].pivot + (_dx[i] - thickness[i] * 0.5f) * laneInfos[j].normal + _dy);
                    _vertices.Add(laneInfos[j + 1].pivot + (_dx[i] - thickness[i] * 0.5f) * laneInfos[j + 1].normal + _dy);
                    _vertices.Add(laneInfos[j + 1].pivot + (_dx[i] + thickness[i] * 0.5f) * laneInfos[j + 1].normal + _dy);
                    _triangles.AddRange(new int[]
                    {
                        _confirmedVerts,_confirmedVerts+1,_confirmedVerts+2,
                        _confirmedVerts,_confirmedVerts+2,_confirmedVerts+3
                    });
                    _confirmedVerts += 4;
                }
                _mesh.vertices = _vertices.ToArray();
                _mesh.triangles = _triangles.ToArray();
                _mesh.RecalculateBounds();
                _meshFilter.sharedMesh = _mesh;
            }
        }

        //�m�[�c�z�u�BLN�̔z�u�ɂ��Ă͂���while�u���b�N�I�Ȃ̂�����ŉ񂵂Ȃ��烌�[�������̗v�̂�mesh�𐶐����銴����
        foreach (var ev in evs)
        {
            while (ev.beat1_192 >= laneInfos[currentLane+1].timing)
            {
                currentLane++;
                //IndexOutOfRange�ɂ͂Ȃ�Ȃ��͂�
            }
            if(101 <= ev.id && ev.id <= 104)//�ʏ�Tap
            {
                var obj = Instantiate(tapGameObject, this.transform);
                obj.name = "Note(" + ev.uniqueId.ToString() + ")";
                var deltaBeat = ev.beat1_192 - laneInfos[currentLane].timing;
                var _direction = (laneInfos[currentLane].direction + laneInfos[currentLane + 1].direction) * 0.5f;//���ς����
                var _normal = (laneInfos[currentLane].normal + laneInfos[currentLane + 1].normal) * 0.5f;//���ς��Ƃ�
                obj.transform.localPosition = laneInfos[currentLane].pivot//tap�����݂����Ԃ̋N�_���W
                    + _normal * dx[ev.id-101]//delta local X
                    + _direction * (float)(speed * deltaBeat)//delta local Z
                    + new Vector3(0f, 0.0004f, 0f);//delta Y(kari)delta local y�p�̕ϐ��p�ӂ�����
                obj.transform.localRotation = Quaternion.Euler((laneInfos[currentLane].angle.eulerAngles
                    + laneInfos[currentLane + 1].angle.eulerAngles) * 0.5f);//��](�I�C���[�p)�̕��ς����
            }
            if(109 <= ev.id && ev.id <= 110)//accel,break Tap
            {
                var obj = Instantiate(fxGameObject, this.transform);
                obj.name = "Note(" + ev.uniqueId.ToString() + ")";
                var deltaBeat = ev.beat1_192 - laneInfos[currentLane].timing;
                var _direction = (laneInfos[currentLane].direction + laneInfos[currentLane + 1].direction) * 0.5f;//���ς����
                var _normal = (laneInfos[currentLane].normal + laneInfos[currentLane + 1].normal) * 0.5f;//���ς��Ƃ�
                obj.transform.localPosition = laneInfos[currentLane].pivot//tap�����݂����Ԃ̋N�_���W
                    + _normal * (0.12575f + (ev.id - 110) * 0.2515f)//delta local X
                    + _direction * (float)(speed * deltaBeat)//delta local Z
                    + new Vector3(0f, 0.0003f, 0f);//delta Y(kari)
                obj.transform.localRotation = Quaternion.Euler((laneInfos[currentLane].angle.eulerAngles
                    + laneInfos[currentLane + 1].angle.eulerAngles) * 0.5f);//��](�I�C���[�p)�̕��ς����
            }
            if(105 <= ev.id && ev.id <= 108)//�ʏ�LN
            {
                var obj = new GameObject("Note(" + ev.uniqueId.ToString() + ")");
                obj.transform.parent = this.transform;
                var _meshFilter = obj.AddComponent<MeshFilter>();
                var _meshRenderer = obj.AddComponent<MeshRenderer>();
                Mesh _mesh = new Mesh();
                List<Vector3> _vertices = new List<Vector3>();
                List<int> _triangles = new List<int>();
                var _currentBeat = ev.beat1_192;
                var _currentLane = currentLane;
                var _confirmedVerts = 0;
                while(ev.beat1_192 + ev.args[0] - _currentBeat > 0)
                {
                    if(laneInfos[_currentLane+1].timing >= ev.beat1_192 + ev.args[0])//�Ō�̒���
                    {
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane].normal//�n�_�E
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id-105] - 0.06f) * laneInfos[_currentLane].normal//�n�_��
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] - 0.06f) * laneInfos[_currentLane].normal//�I�_��
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane].normal//�I�_�E
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _triangles.AddRange(new int[]
                        {
                            _confirmedVerts,_confirmedVerts+1,_confirmedVerts+2,
                            _confirmedVerts,_confirmedVerts+2,_confirmedVerts+3
                        });
                        _confirmedVerts += 4;
                        _currentBeat = ev.beat1_192 + ev.args[0];
                    }
                    else
                    {
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane].normal//�n�_�E
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] - 0.06f) * laneInfos[_currentLane].normal//�n�_��
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane+1].pivot
                            + (dx[ev.id - 105] - 0.06f) * laneInfos[_currentLane+1].normal//�I�_��
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane+1].pivot
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane+1].normal//�I�_�E
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _triangles.AddRange(new int[]
                        {
                            _confirmedVerts,_confirmedVerts+1,_confirmedVerts+2,
                            _confirmedVerts,_confirmedVerts+2,_confirmedVerts+3
                        });
                        _confirmedVerts += 4;
                        _currentBeat = laneInfos[_currentLane+1].timing;
                        _currentLane++;
                    }
                }
                _mesh.vertices = _vertices.ToArray();
                _mesh.triangles = _triangles.ToArray();
                _mesh.RecalculateBounds();
                _meshFilter.sharedMesh = _mesh;
            }
            if (111 <= ev.id && ev.id <= 112)
            {
                var obj = new GameObject("Note(" + ev.uniqueId.ToString() + ")");
                obj.transform.parent = this.transform;
                var _meshFilter = obj.AddComponent<MeshFilter>();
                var _meshRenderer = obj.AddComponent<MeshRenderer>();
                Mesh _mesh = new Mesh();
                List<Vector3> _vertices = new List<Vector3>();
                List<int> _triangles = new List<int>();
                var _currentBeat = ev.beat1_192;
                var _currentLane = currentLane;
                var _confirmedVerts = 0;
                var _dx = 0.12575f + (ev.id - 112) * 0.2515f;
                while (ev.beat1_192 + ev.args[0] - _currentBeat > 0)
                {
                    if (laneInfos[_currentLane + 1].timing >= ev.beat1_192 + ev.args[0])//�Ō�̒���
                    {
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx+0.12175f) * laneInfos[_currentLane].normal//�n�_�E
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx-0.12175f) * laneInfos[_currentLane].normal//�n�_��
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (_dx-0.12175f) * laneInfos[_currentLane].normal//�I�_��
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (_dx+0.12175f) * laneInfos[_currentLane].normal//�I�_�E
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _triangles.AddRange(new int[]
                        {
                            _confirmedVerts,_confirmedVerts+1,_confirmedVerts+2,
                            _confirmedVerts,_confirmedVerts+2,_confirmedVerts+3
                        });
                        _confirmedVerts += 4;
                        _currentBeat = ev.beat1_192 + ev.args[0];
                    }
                    else
                    {
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx + 0.12175f) * laneInfos[_currentLane].normal//�n�_�E
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx - 0.12175f) * laneInfos[_currentLane].normal//�n�_��
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane + 1].pivot
                            + (_dx - 0.12175f) * laneInfos[_currentLane + 1].normal//�I�_��
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane + 1].pivot
                            + (_dx + 0.12175f) * laneInfos[_currentLane + 1].normal//�I�_�E
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _triangles.AddRange(new int[]
                        {
                            _confirmedVerts,_confirmedVerts+1,_confirmedVerts+2,
                            _confirmedVerts,_confirmedVerts+2,_confirmedVerts+3
                        });
                        _confirmedVerts += 4;
                        _currentBeat = laneInfos[_currentLane + 1].timing;
                        _currentLane++;
                    }
                }
                _mesh.vertices = _vertices.ToArray();
                _mesh.triangles = _triangles.ToArray();
                _mesh.RecalculateBounds();
                _meshFilter.sharedMesh = _mesh;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position += 60.0f * speed * Time.deltaTime * Vector3.forward;
    }
}
