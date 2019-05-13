﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadScript : MonoBehaviour
{
    public static LoadScript loader;
    public int checkpointNumber = 0;
    public float playerWeight = 0;
    public int playerCharge = 0;
    public Vector3[] checkpointPos;
    public Camera[] checkpointCamera;
    private string savePath;
    private SaveData save;
    public GameObject weightPrefab;
    public GameObject batteryPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) { Save(); }
        if (Input.GetKeyDown(KeyCode.KeypadPeriod)) { Load(); }
    }

    private void Awake()
    {
        loader = this;
        savePath = Application.persistentDataPath + "/RoboHeist.dat";
    }

    private void Start()
    {
        if (!DataPackage.NewGame) { Load(); }
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            save = (SaveData)bf.Deserialize(file);
            file.Close();
            checkpointNumber = save.checkpoint;
            PlayerMove.player.gameObject.transform.position = checkpointPos[checkpointNumber];
            CameraMenu.camMenu.ChangeCamera(checkpointCamera[checkpointNumber]);
            for(int i = 0; i <= save.charge; i++)
            {
                GameObject temp = Instantiate(batteryPrefab);
                temp.GetComponent<ScoopableObject>().Scoop();
            }
            for(int i = (int)PlayerInv.playerInv.weight; i <= save.weight; i++)
            {
                GameObject temp = Instantiate(weightPrefab);
                temp.GetComponent<ScoopableObject>().Scoop();
            }
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if(File.Exists(savePath)) { file = File.Open(savePath, FileMode.Open); }
        else { file = File.Create(savePath); }
        save = new SaveData(playerWeight, playerCharge, checkpointNumber);
        bf.Serialize(file, save);
        file.Close();
        DataPackage.NewGame = true;
    }
}

[System.Serializable]
public class SaveData
{
    public float weight;
    public int charge;
    public int checkpoint;

    public SaveData(float weight, int charge, int checkpoint)
    {
        this.weight = weight;
        this.charge = charge;
        this.checkpoint = checkpoint;
    }
}