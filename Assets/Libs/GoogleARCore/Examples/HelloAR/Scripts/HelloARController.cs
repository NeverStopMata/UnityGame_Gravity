﻿//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.HelloAR
{
    using System;
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityStandardAssets.CrossPlatformInput;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        enum DisplayState { start, playing, succeed, fail };
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject TrackedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyAndroidPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        //public GameObject SearchingForPlaneUI;
        public GameObject InitUI;
        public GameObject FailUI;
        public GameObject SucceedUI;
        public GameObject TouchControls;
        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;
        private bool m_hasCreateBattle = false;
        private GameObject battleGO;
        private DisplayState _displaystate;
        /// <summary>
        /// The Unity Update() method.
        /// 
        /// </summary>
        private void Start()
        {
            SetUIActive(DisplayState.start);
        }
        private void SetUIActive(DisplayState distate)
        {
            _displaystate = distate;
            if (distate == DisplayState.playing)
            {
                //TouchControls.SetActive(false);
                InitUI.SetActive(false);
                //SucceedUI.SetActive(false);
                FailUI.SetActive(false);
            }
            else if (distate == DisplayState.start)
            {
                //TouchControls.SetActive(false);
                InitUI.SetActive(true);
                //SucceedUI.SetActive(false);
                //FailUI.SetActive(false);
            }
            else if (distate == DisplayState.fail)
            {
                //TouchControls.SetActive(false);
                //InitUI.SetActive(false);
                //SucceedUI.SetActive(false);
                FailUI.SetActive(true);
            }
            else if (distate == DisplayState.succeed)
            {
                TouchControls.SetActive(false);
                //InitUI.SetActive(false);
                SucceedUI.SetActive(true);
                //FailUI.SetActive(false);
            }

        }
        public void beginPlaying()
        {
            m_hasCreateBattle = false;
            if (battleGO != null)
            {
                DestroyImmediate(battleGO);
            }
            SetUIActive(DisplayState.playing);
        }
        public void Update()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (CrossPlatformInputManager.GetButtonDown("Restart"))
            {
                //Application.Quit();
                m_hasCreateBattle = false;
                TouchControls.SetActive(false);
                if (battleGO != null)
                {
                    DestroyImmediate(battleGO);
                }
            }
            _QuitOnConnectionErrors();

            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)//mata mod
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
                if (!m_IsQuitting && Session.Status.IsValid())
                {
                    //SearchingForPlaneUI.SetActive(true);
                }

                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            Session.GetTrackables<TrackedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity,
                    transform);
                planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
            }

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<TrackedPlane>(m_AllPlanes);
            bool showSearchingUI = true;//mata mod
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            //SearchingForPlaneUI.SetActive(showSearchingUI);

            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;


            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && !m_hasCreateBattle)
            {
                battleGO = Instantiate(AndyAndroidPrefab, hit.Pose.position, hit.Pose.rotation);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Andy should look at the camera but still be flush with the plane.
                if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
                {
                    // Get the camera position and match the y-component with the hit position.
                    Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                    cameraPositionSameY.y = hit.Pose.position.y;//相机在地板上的投影点。
                    var lookAtPos = battleGO.transform.position - (cameraPositionSameY - battleGO.transform.position);
                    // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
                    //andyObject.transform.LookAt(andyObject.transform.position + Vector3.forward, anchor.transform.up);
                    battleGO.transform.LookAt(lookAtPos, anchor.transform.up);
                }

                // Make Andy model a child of the anchor.
                battleGO.transform.parent = anchor.transform;
                m_hasCreateBattle = true;
                TouchControls.SetActive(true);

                //dualTouchControls = (GameObject)Instantiate(Resources.Load("Standard_Assets/CrossPlatformInput/Prefabs/DualTouchControls"));

            }

        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }

        public void FinishGame(bool isSucceed)
        {
            if (isSucceed)
            {
                SetUIActive(DisplayState.succeed);
            }
            else
            {
                SetUIActive(DisplayState.fail);
            }
        }
    }
}
