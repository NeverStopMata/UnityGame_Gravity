﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class TimeBacker
{
    private bool isRewinding = false;//用来判断是否需要时光逆流
    private bool canRecord = true;
    private bool canRewind = true;
    private float recordTime = 600f;//时光逆流时间
    private Dictionary<Transform, List<PosRotInf>> PosRotTable;
    private List<Vector3> Gravities;
    private Transform battleTransform;
    public TimeBacker(List<Transform> _movableGOs, float _recordTime, Transform _battleTransform)
    {
        PosRotTable = new Dictionary<Transform, List<PosRotInf>>();
        recordTime = _recordTime;
        foreach (var go in _movableGOs)
        {
            PosRotTable.Add(go, new List<PosRotInf>());
        }
        Gravities = new List<Vector3>();
        battleTransform = _battleTransform;
    }

    public void StartRewind()
    {
        isRewinding = true;
        foreach (var go in PosRotTable.Keys)
        {
            if (go.gameObject.GetComponent<Rigidbody>()!= null)
            {
                go.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            
        }

    }

    public void clearRecord()
    {
        foreach (var v in PosRotTable.Values)
        {
            v.Clear();
        }
    }

    /// <summary>
    /// 停止时光逆流
    /// </summary>
    public void StopRewind()
    {
        isRewinding = false;
        foreach (var go in PosRotTable.Keys)
        {
            if (go.gameObject.GetComponent<Rigidbody>() != null)
            {
                go.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
    public void Execution()
    {
        if (isRewinding)
            Rewind();
        else
            Record();
    }

    private void Rewind()
    {
        //记录点数量大于0时才可以倒流
        if (PosRotTable.Count > 0 && PosRotTable.First().Value.Count > 0)
        {
            foreach (var kv in PosRotTable)
            {
                PosRotInf currentPosRot = kv.Value[0];
                kv.Key.SetPositionAndRotation(currentPosRot.position, currentPosRot.rotation);
                kv.Value.RemoveAt(0);
            }
        }
        if (Gravities.Count > 0)
        {


            //Vector3 GravityDrctInBattleWorld = GravityController.getPlayerDrct(battleTransform.InverseTransformDirection(Gravities[0]));
            //Physics.gravity = battleTransform.TransformDirection(GravityDrctInBattleWorld);//
            Physics.gravity = Gravities[0];//
            Gravities.RemoveAt(0);
        }

    }

    /// <summary>
    /// 记录物体的信息
    /// </summary>
    private void Record()
    {
        if (canRecord)
        {
            foreach (var kv in PosRotTable)
            {
                if (kv.Value.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
                {
                    kv.Value.RemoveAt(kv.Value.Count - 1);
                }
                kv.Value.Insert(0, new PosRotInf(kv.Key.position, kv.Key.rotation));
            }
            if (Gravities.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
            {
                Gravities.RemoveAt(Gravities.Count - 1);
            }
            Gravities.Insert(0, Physics.gravity);
        }


    }
}
public class PosRotInf
{
    public Vector3 position;
    public Quaternion rotation;
    public PosRotInf(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}

