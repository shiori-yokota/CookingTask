using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using SIGVerse.Common;
using System.Threading;
using System.Collections;

public class PlayBackMotions : MonoBehaviour
{

    private ShowRecipe sr;
    private PlaybackerCommon pbc;

    private float elapsedTime = 0.0f;
    private bool isPlaying = false;

    private List<string> MotionsData = new List<string>();

    private Dictionary<string, Transform> targetObjectsPathMap = new Dictionary<string, Transform>();
    private List<UpdatingTransformList> playingTransformList = new List<UpdatingTransformList>();
    private int playingTransformIndex;


    // Use this for initialization
    void Start()
    {
        sr = FindObjectOfType<ShowRecipe>();
        pbc = FindObjectOfType<PlaybackerCommon>();
    }

    private void Update()
    {
        this.elapsedTime += Time.deltaTime;
        if (this.isPlaying)
        {
            this.PlayMotions();
        }
    }

    public void OnClick()
    {
        try
        {
            if (!this.isPlaying)
            {
                this.StartPlaying();
            }
            else
            {
                this.StopPlaying();
            }
        }
        catch (Exception ex)
        {
            SIGVerseLogger.Error(ex.Message);
            SIGVerseLogger.Error(ex.StackTrace);
        }
    }

    private void PlayMotions()
    {
        // Stop playing when reached the end of the list
        if (this.playingTransformIndex >= this.playingTransformList.Count)
        {
            this.StopPlaying();
            return;
        }

        UpdatingTransformList updatingTransformList = null;

        // Increase the list index until the elapsed time of the list reaches the actual elapsed time
        while (this.elapsedTime >= this.playingTransformList[this.playingTransformIndex].ElapsedTime)
        {
            updatingTransformList = this.playingTransformList[this.playingTransformIndex];

            this.playingTransformIndex++;

            if (this.playingTransformIndex >= this.playingTransformList.Count) { break; }
        }

        if (updatingTransformList == null) { return; }

        // Play
        foreach (UpdatingTransformData updatingTransformData in updatingTransformList.GetUpdatingTransformList())
        {
            updatingTransformData.UpdateTransform();
        }
    }

    private void StartPlaying()
    {
        SIGVerseLogger.Info("Player : Initialise");

        this.playingTransformList = sr.GetPlayingTransformList();
        this.targetObjectsPathMap = sr.GetTargetObjectsPathMap();

        this.playingTransformIndex = 0;
        this.elapsedTime = 0.0f;
        this.isPlaying = true;

    }

    private void StopPlaying()
    {
        this.isPlaying = false;

        SIGVerseLogger.Info("Player : playing finished.");
        pbc.ResetObjects();
    }
}
