using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class Demo_Script : MonoBehaviour
{

    [Header("Textures:")]
    Texture[] ninjaTextures;
    Texture[] ninjaNormals;
    Texture[] ninjaSpeculars;
    Texture[] swordTextures;
    Texture[] swordNormals;
    Texture[] swordSpeculars;


    public enum NinjaColor
    {
        Blue, Green, Red
    };
    public enum SwordColor
    {
        Blue, Green, Red
    };

    //Color Inspector DropDownWindow 
    [Header("Color:")]
    public NinjaColor ninjaColor;
    public SwordColor swordColor;

    NinjaColor currentNinjaColor;
    SwordColor currentSwordColor;

    void Start()
    {
        InitializeTextures();
        //Set initial values
        GameObject.Find("Ninja_Mesh").GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", ninjaTextures[0]);
        GameObject.Find("Ninja_Mesh").GetComponent<Renderer>().sharedMaterial.SetTexture("_BumpMap", ninjaNormals[0]);
        GameObject.Find("Ninja_Mesh").GetComponent<Renderer>().sharedMaterial.SetTexture("_SpecGlossMap", ninjaSpeculars[0]);

        ChangeTexture("Scabbard_Mesh", swordTextures[0]);
        ChangeTexture("Sword_Mesh", swordTextures[0]);
    }

    private void InitializeTextures()
    {
        #region Ninja
        string[] ninjaTexturesGUID = AssetDatabase.FindAssets("Kunoichi_Color_");
        ninjaTextures = new Texture[ninjaTexturesGUID.Length];
        string[] ninjaNormalGUID = AssetDatabase.FindAssets("Kunoichi_Normal");
        ninjaNormals = new Texture[ninjaNormalGUID.Length];
        string[] ninjaSpecGUID = AssetDatabase.FindAssets("Kunoichi_Spec_Smoothness");
        ninjaSpeculars = new Texture[ninjaSpecGUID.Length];

        for (int i = 0; i< ninjaTexturesGUID.Length; i++)
        {
            ninjaTextures[i] = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ninjaTexturesGUID[i]), typeof(Texture)); 
        }

        ninjaNormals[0] = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ninjaNormalGUID[0]), typeof(Texture));
        ninjaSpeculars[0] = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ninjaSpecGUID[0]), typeof(Texture));
        #endregion

        #region Sword
        string[] swordTexturesGUID = AssetDatabase.FindAssets("Sword_Color_");
        swordTextures = new Texture[swordTexturesGUID.Length];
        string[] swordNormalGUID = AssetDatabase.FindAssets("Sword_Normal");
        swordNormals= new Texture[swordNormalGUID.Length];
        string[] swordSpecGUID = AssetDatabase.FindAssets("Sword_Spec_Smoothness");
        swordSpeculars = new Texture[swordSpecGUID.Length];

        for (int i = 0; i < swordTexturesGUID.Length; i++)
        {
            swordTextures[i] = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(swordTexturesGUID[i]), typeof(Texture));
        }

        swordNormals[0] = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(swordNormalGUID[0]), typeof(Texture));
        swordSpeculars[0] = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(swordSpecGUID[0]), typeof(Texture));
        #endregion

    }

    void Update()
    {
        if (currentNinjaColor != ninjaColor)
        {
            switch (ninjaColor)
            {
                case NinjaColor.Blue:
                    ChangeTexture("Ninja_Mesh", ninjaTextures[0]);
                    break;
                case NinjaColor.Green:
                    ChangeTexture("Ninja_Mesh", ninjaTextures[1]);
                    break;
                case NinjaColor.Red:
                    ChangeTexture("Ninja_Mesh", ninjaTextures[2]);
                    break;
                default:
                    break;
            }
            currentNinjaColor = ninjaColor;
        }

        if (currentSwordColor != swordColor)
        {
            switch (swordColor)
            {
                case SwordColor.Blue:
                    ChangeTexture("Scabbard_Mesh", swordTextures[0]);
                    ChangeTexture("Sword_Mesh", swordTextures[0]);
                    break;
                case SwordColor.Green:
                    ChangeTexture("Scabbard_Mesh", swordTextures[1]);
                    ChangeTexture("Sword_Mesh", swordTextures[1]);
                    break;
                case SwordColor.Red:
                    ChangeTexture("Scabbard_Mesh", swordTextures[2]);
                    ChangeTexture("Sword_Mesh", swordTextures[2]);
                    break;
                default:
                    break;
            }
            currentSwordColor = swordColor;
        }
    }

    void ChangeTexture(string meshName, Texture texture)
    {
        GameObject.Find(meshName).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", texture);
    }
}
