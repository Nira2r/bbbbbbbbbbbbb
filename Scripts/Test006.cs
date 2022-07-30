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
    float speed;// 0.50 * scrspd (* hisp) あとはbeat1/192をかけるだけ
    float width;//レーンの幅(2レーン分)
    float resolution = 5.0f;//LongTurnの分解能(deg)

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

        //Todo: とりあえずレーン生成を作る、ノーツ配置、カメラの挙動(マッピング)、テクスチャ、譜面の終端処理

        var endBeat = 0.0;

        //レーン生成、レーン情報取得
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

            //右カーブ(Tap)
            if (ev.id == 114)
            {
                //直線部分(カーブ前)を確定させる
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

                //角の位置ベクトルを求める

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
            else if(ev.id == 113)//左カーブ(tap)
            {
                //直線部分(カーブ前)を確定させる
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

                //角の位置ベクトルを求める

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
            else if(ev.id==116)//右カーブ(LN)
            {
                var angleLeft = (float)ev.args[0];
                var duration = ev.args[1];//LNの長さ(1/192beat)
                var r = (float)(speed * duration / (angleLeft * Mathf.Deg2Rad));//内周の半径

                //まずは直線部分を確定させる
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

                //angleLeftが分解能を下回るまで分解能ずつつくってく
                
                while (angleLeft >= resolution)
                {
                    var dirAfter = Quaternion.Euler(0, resolution, 0) * directon;
                    var normAfter = Quaternion.Euler(0, resolution, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        - 2 * width * normAfter);//外周頂点
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon);//内周頂点

                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;

                    currentPos += (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        - width * normAfter;//currentPosは常にレーンの中央
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, resolution, 0);
                    angleLeft -= resolution;
                    currentBeat += ev.args[1] * resolution / ev.args[0];
                    laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
                }
                //あまりの処理
                if(angleLeft>0)
                {
                    var dirAfter = Quaternion.Euler(0, angleLeft, 0) * directon;
                    var normAfter = Quaternion.Euler(0, angleLeft, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        - 2 * width * normAfter);//外周頂点
                    vertices.Add(currentPos
                        + (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon);//内周頂点
                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;
                    currentPos += (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        - width * normAfter;//currentPosは常にレーンの中央
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, angleLeft, 0);
                }
                currentBeat = ev.beat1_192 + ev.args[1];//LNの終点まで生成したのでそこにcurrentBeatを置く
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
            }
            else if (ev.id == 115)//左カーブ(LN)
            {
                var angleLeft = (float)ev.args[0];
                var duration = ev.args[1];//LNの長さ(1/192beat)
                var r = (float)(speed * duration / (angleLeft * Mathf.Deg2Rad));//内周の半径

                //まずは直線部分を確定させる
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

                //angleLeftが分解能を下回るまで分解能ずつつくってく
                while (angleLeft >= resolution)
                {
                    var dirAfter = Quaternion.Euler(0, -resolution, 0) * directon;
                    var normAfter = Quaternion.Euler(0, -resolution, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon);//内周頂点
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        + 2 * width * normAfter);//外周頂点

                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;

                    currentPos += -(width + r * (1 - Mathf.Cos(resolution * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(resolution * Mathf.Deg2Rad) * directon
                        + width * normAfter;//currentPosは常にレーンの中央
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, -resolution, 0);
                    angleLeft -= resolution;
                    currentBeat += ev.args[1] * resolution / ev.args[0];
                    laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
                }
                //あまりの処理
                if (angleLeft > 0)
                {
                    var dirAfter = Quaternion.Euler(0, -angleLeft, 0) * directon;
                    var normAfter = Quaternion.Euler(0, -angleLeft, 0) * normal;

                    vertices.Add(currentPos + width * normal);
                    vertices.Add(currentPos - width * normal);
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon);//内周頂点
                    vertices.Add(currentPos
                        - (width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        + 2 * width * normAfter);//外周頂点
                    triangles.AddRange(new int[]
                    {
                        confirmedVerts,confirmedVerts+1,confirmedVerts+2,
                        confirmedVerts,confirmedVerts+2,confirmedVerts+3
                    });
                    confirmedVerts += 4;
                    currentPos += -(width + r * (1 - Mathf.Cos(angleLeft * Mathf.Deg2Rad))) * normal
                        + r * Mathf.Sin(angleLeft * Mathf.Deg2Rad) * directon
                        + width * normAfter;//currentPosは常にレーンの中央
                    directon = dirAfter;
                    normal = normAfter;
                    currentAngle *= Quaternion.Euler(0, -angleLeft, 0);
                }
                currentBeat = ev.beat1_192 + ev.args[1];//LNの終点まで生成したのでそこにcurrentBeatを置く
                laneInfos.Add(new LaneInfo(currentBeat, currentAngle, currentPos));
            }
        }

        //終端処理
        endBeat += 192.0;//1小節盛る
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

        //LaneInfoのどの区間にいるか(idx)
        int currentLane = -1;

        float[] dx = new float[] { -0.1875f, -0.064f, 0.064f, 0.1875f };//delta local X用

        //レーン枠配置
        {
            float[] thickness = new float[] { 0.0025f, 0.0035f, 0.0080f, 0.0035f, 0.0025f };
            float[] _dx = new float[5];
            for (int i = 0; i < 5; ++i)//左からi-1本目の線
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
                for(int j=0;j<laneInfos.Count-1;++j)//j番目の区間
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

        //ノーツ配置。LNの配置についてはこのwhileブロック的なのを内部で回しながらレーン生成の要領でmeshを生成する感じで
        foreach (var ev in evs)
        {
            while (ev.beat1_192 >= laneInfos[currentLane+1].timing)
            {
                currentLane++;
                //IndexOutOfRangeにはならないはず
            }
            if(101 <= ev.id && ev.id <= 104)//通常Tap
            {
                var obj = Instantiate(tapGameObject, this.transform);
                obj.name = "Note(" + ev.uniqueId.ToString() + ")";
                var deltaBeat = ev.beat1_192 - laneInfos[currentLane].timing;
                var _direction = (laneInfos[currentLane].direction + laneInfos[currentLane + 1].direction) * 0.5f;//平均を取る
                var _normal = (laneInfos[currentLane].normal + laneInfos[currentLane + 1].normal) * 0.5f;//平均をとる
                obj.transform.localPosition = laneInfos[currentLane].pivot//tapが存在する区間の起点座標
                    + _normal * dx[ev.id-101]//delta local X
                    + _direction * (float)(speed * deltaBeat)//delta local Z
                    + new Vector3(0f, 0.0004f, 0f);//delta Y(kari)delta local y用の変数用意したい
                obj.transform.localRotation = Quaternion.Euler((laneInfos[currentLane].angle.eulerAngles
                    + laneInfos[currentLane + 1].angle.eulerAngles) * 0.5f);//回転(オイラー角)の平均を取る
            }
            if(109 <= ev.id && ev.id <= 110)//accel,break Tap
            {
                var obj = Instantiate(fxGameObject, this.transform);
                obj.name = "Note(" + ev.uniqueId.ToString() + ")";
                var deltaBeat = ev.beat1_192 - laneInfos[currentLane].timing;
                var _direction = (laneInfos[currentLane].direction + laneInfos[currentLane + 1].direction) * 0.5f;//平均を取る
                var _normal = (laneInfos[currentLane].normal + laneInfos[currentLane + 1].normal) * 0.5f;//平均をとる
                obj.transform.localPosition = laneInfos[currentLane].pivot//tapが存在する区間の起点座標
                    + _normal * (0.12575f + (ev.id - 110) * 0.2515f)//delta local X
                    + _direction * (float)(speed * deltaBeat)//delta local Z
                    + new Vector3(0f, 0.0003f, 0f);//delta Y(kari)
                obj.transform.localRotation = Quaternion.Euler((laneInfos[currentLane].angle.eulerAngles
                    + laneInfos[currentLane + 1].angle.eulerAngles) * 0.5f);//回転(オイラー角)の平均を取る
            }
            if(105 <= ev.id && ev.id <= 108)//通常LN
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
                    if(laneInfos[_currentLane+1].timing >= ev.beat1_192 + ev.args[0])//最後の直線
                    {
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane].normal//始点右
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id-105] - 0.06f) * laneInfos[_currentLane].normal//始点左
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] - 0.06f) * laneInfos[_currentLane].normal//終点左
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane].normal//終点右
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
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane].normal//始点右
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (dx[ev.id - 105] - 0.06f) * laneInfos[_currentLane].normal//始点左
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane+1].pivot
                            + (dx[ev.id - 105] - 0.06f) * laneInfos[_currentLane+1].normal//終点左
                            + new Vector3(0, 0.0002f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane+1].pivot
                            + (dx[ev.id - 105] + 0.06f) * laneInfos[_currentLane+1].normal//終点右
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
                    if (laneInfos[_currentLane + 1].timing >= ev.beat1_192 + ev.args[0])//最後の直線
                    {
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx+0.12175f) * laneInfos[_currentLane].normal//始点右
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx-0.12175f) * laneInfos[_currentLane].normal//始点左
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (_dx-0.12175f) * laneInfos[_currentLane].normal//終点左
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (ev.beat1_192 + ev.args[0] - laneInfos[_currentLane].timing))
                            + (_dx+0.12175f) * laneInfos[_currentLane].normal//終点右
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
                            + (_dx + 0.12175f) * laneInfos[_currentLane].normal//始点右
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane].pivot
                            + laneInfos[_currentLane].direction * (float)(speed * (_currentBeat - laneInfos[_currentLane].timing))
                            + (_dx - 0.12175f) * laneInfos[_currentLane].normal//始点左
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane + 1].pivot
                            + (_dx - 0.12175f) * laneInfos[_currentLane + 1].normal//終点左
                            + new Vector3(0, 0.0001f, 0));//delta y(kari)
                        _vertices.Add(laneInfos[_currentLane + 1].pivot
                            + (_dx + 0.12175f) * laneInfos[_currentLane + 1].normal//終点右
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
