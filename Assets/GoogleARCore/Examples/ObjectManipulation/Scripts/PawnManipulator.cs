//-----------------------------------------------------------------------
// <copyright file="PawnManipulator.cs" company="Google">
//
// Copyright 2019 Google LLC. All Rights Reserved.
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

namespace GoogleARCore.Examples.ObjectManipulation
{
    using GoogleARCore;
    using UnityEngine;

    /// <summary>
    /// Controls the placement of objects via a tap gesture.
    /// </summary>
    public class PawnManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a plane.
        /// </summary>
        

        /// <summary>
        /// Manipulator prefab to attach placed objects to.
        /// </summary>
        public GameObject ManipulatorPrefab;


        /* NAMAN started to change script to add 2 buttons & change the model based on the bitton click
         */
        public GameObject obj1_prefab;
        public GameObject obj2_prefab;
        private const float k_ModelRotation = 0f;

        private bool firstBtnClicked = true;


        public void firstButtonClick()
        {
            firstBtnClicked = true;
        }

        public void secondButtonClick()
        {
            firstBtnClicked = false;
        }

        /* NAMAN ended to change script to add 2 buttons & change the model based on the bitton click
         */


        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (Frame.Raycast(
                gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    /* NAMAN started to change script to add 2 buttons & change the model based on the bitton click
                       */

                    GameObject obj = null;
                    if (firstBtnClicked == true)
                    {
                        // Instantiate first Asset model at the hit pose.
                        obj = Instantiate(obj1_prefab, hit.Pose.position, hit.Pose.rotation);
                    }
                    else
                    {
                        // Second button clicked: Instantiate second Asset model at the hit pose.
                        obj = Instantiate(obj2_prefab, hit.Pose.position, hit.Pose.rotation);
                    }

                    // Compensate for the hitPose rotation facing towards from the raycast (i.e. camera).
                    obj.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
                    obj.name = "myobject";
                    // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                    // world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    
                    // Instantiate manipulator.
                    var manipulator =
                        Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);


                    
                    obj1_prefab.transform.parent = manipulator.transform;
                    obj2_prefab.transform.parent = manipulator.transform;
                    /* NAMAN ended to change script to add 2 buttons & change the model based on the bitton click
                       */
                    // Make game object a child of the manipulator.
                    obj.transform.parent = manipulator.transform;

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    // Make manipulator a child of the anchor.
                    manipulator.transform.parent = anchor.transform;

                    // Select the placed object.
                    manipulator.GetComponent<Manipulator>().Select();
                }
            }
        }
    }
}
