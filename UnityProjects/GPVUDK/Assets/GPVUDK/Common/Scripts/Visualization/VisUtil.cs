﻿using UnityEngine;
using System.Collections.Generic;

namespace GPVUDK
{
    /// <summary>
    /// General scene utilities
    /// </summary>
    public class VisUtil
    {
        /// <summary>
        /// Set the given game object as visible or not, enabling or disabling
        /// all its mesh renderers (children included)
        /// </summary>
        /// <param name="targetObject">The main game object to show/hide</param>
        /// <param name="visible">if true enable mesh renderers, if false disable them</param>
        public static void SetVisible(GameObject targetObject, bool visible)
        {
            foreach (MeshRenderer mr in targetObject.GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = visible;
            }
        }

        /// <summary>
        /// Store a map of renderers and their materials into a dictionary
        /// </summary>
        /// <param name="targetObject">Game object to scan</param>
        /// <param name="materialsBackup">Dictionary used to map renderers to their materials</param>
        public static void BackupMaterials(GameObject targetObject, Dictionary<Renderer, Material> materialsBackup)
        {
            foreach (Renderer mr in targetObject.GetComponentsInChildren<Renderer>(true))
            {
                materialsBackup[mr] = mr.material;
            }
        }

        /// <summary>
        /// Replace the materials of a game onbject using a previously stored map (see <see cref="BackupMaterials"/>)
        /// </summary>
        /// <param name="targetObject">Game object to scan</param>
        /// <param name="materialsBackup">Dictionary that maps renderers to their materials</param>
        /// <remarks>If the given dictionary does not match the renderers the behaviour is undefined.</remarks>
        public static void ReplaceMaterials(GameObject targetObject, Dictionary<Renderer, Material> materialsBackup)
        {
            foreach (Renderer mr in targetObject.GetComponentsInChildren<Renderer>(true))
            {
                if (materialsBackup.ContainsKey(mr)) mr.material = materialsBackup[mr];
            }
        }

        /// <summary>
        /// Swap the materials of the renderers of a game object with the given set of materials (see <see cref="BackupMaterials"/>)
        /// </summary>
        /// <param name="targetObject">Game object to scan</param>
        /// <param name="materialsBackup">Dictionary that maps renderers to their materials</param>
        public static void SwapMaterials(GameObject targetObject, Dictionary<Renderer, Material> materialsBackup)
        {
            foreach (Renderer mr in targetObject.GetComponentsInChildren<Renderer>(true))
            {
                if (materialsBackup.ContainsKey(mr)) mr.material = materialsBackup[mr];
            }
        }

        /// <summary>
        /// Replace the materials in a game object with a copy of each one of them.
        /// Both the original materials and the cloned materials are stored in the given dictionaries.
        /// </summary>
        /// <param name="targetObject">Game object to scan</param>
        /// <param name="originalMaterials">Dictionary that maps renderers to their original materials</param>
        /// <param name="newMaterials">Dictionary that maps renderers to their cloned materials</param>
        /// <remarks>The given dictionaries are cleaned before they are filled.</remarks>
        public static void CloneMaterials(GameObject targetObject, ref Dictionary<Renderer, Material> originalMaterials, ref Dictionary<Renderer, Material> newMaterials)
        {
            originalMaterials.Clear();
            newMaterials.Clear();
            foreach (Renderer mr in targetObject.GetComponentsInChildren<Renderer>(true))
            {
                originalMaterials[mr] = mr.material;
                mr.material = Material.Instantiate(mr.material);
                newMaterials[mr] = mr.material;
            }
        }

        /// <summary>
        /// Change the color (also albedo and emission) of each material of a game object.
        /// </summary>
        /// <param name="targetObject">Game object to which colors are changed</param>
        /// <param name="newColor">Color to set for each material</param>
        /// <remarks>Original materials are changed here, not their copies</remarks>
        public static void ChangeColor(GameObject targetObject, Color newColor)
        {
            foreach (Renderer mr in targetObject.GetComponentsInChildren<Renderer>(true))
            {
                ChangeColor(mr.material, newColor);
            }
        }

        /// <summary>
        /// Change the color (also albedo and emission) of a material.
        /// </summary>
        /// <param name="targetMaterial">Material to which colors are changed</param>
        /// <param name="newColor">Color to set for each material</param>
        public static void ChangeColor(Material targetMaterial, Color newColor)
        {
            targetMaterial.color = newColor;
            if (targetMaterial.HasProperty("_Color"))
            {
                targetMaterial.SetColor("_Color", newColor);
            }
            if (targetMaterial.HasProperty("_EmissionColor"))
            {
                targetMaterial.SetColor("_EmissionColor", newColor);
            }
        }

        /// <summary>
        /// Change the color (also albedo and emission) of each material of a game object.
        /// Materials are cloned and their copies are set to the renderes (see <see cref="CloneMaterials"/>).
        /// </summary>
        /// <param name="targetObject">Game object to scan</param>
        /// <param name="newColor">Color to set for each material</param>
        /// <param name="originalMaterials">Dictionary that maps renderers to their original materials</param>
        /// <param name="newMaterials">Dictionary that maps renderers to their cloned materials</param>
        public static void ChangeColor(GameObject targetObject, Color newColor, ref Dictionary<Renderer, Material> originalMaterials, ref Dictionary<Renderer, Material> newMaterials)
        {
            CloneMaterials(targetObject, ref originalMaterials, ref newMaterials);
            ChangeColor(targetObject, newColor);
        }

        /// <summary>
        /// Move an object to the given position and (optionally) rotation.
        /// </summary>
        /// <param name="movedObject">Game object to be moved</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="targetRot">Target rotation (ignored if null)</param>
        public static void MoveObject(GameObject movedObject, Vector3 targetPos, Quaternion? targetRot = null)
        {
            if (movedObject)
            {
                movedObject.transform.position = targetPos;
                if (targetRot.HasValue)
                {
                    movedObject.transform.rotation = targetRot.Value;
                }
            }
        }

        /// <summary>
        /// Push the rigid body of a game object towards the given position and (optionally) rotation.
        /// </summary>
        /// <param name="movedObject">Game object to be moved (its rigid body will be affected)</param>
        /// <param name="targetPos">Target position</param>
        /// <param name="targetRot">Target rotation (ignored if null)</param>
        /// <param name="maxG">Maximum G-force applied to the rigid body (0 means no limit)</param>
        /// <remarks>Setting a maximum applied G-force slows down the movements, but grants a better stability, mainly when synchronizing objects across the network.</remarks>
        public static void MoveRigidbody(GameObject movedObject, Vector3 targetPos, Quaternion? targetRot = null, float maxG = 20f)
        {
            if (movedObject == null)
            {
                return;
            }
            Rigidbody rigidBody = movedObject.GetComponent<Rigidbody>();
            if (rigidBody)
            {
                float t = Mathf.Max(Time.smoothDeltaTime, Time.deltaTime);
                Vector3 shift = targetPos - rigidBody.position;
                Vector3 velocity = shift / t;
                Vector3 acceleration = velocity;
                rigidBody.velocity = Vector3.zero;
                Vector3 force = rigidBody.mass * acceleration;
                Vector3 gForce = rigidBody.mass * Physics.gravity;// Vector3.down * 9.81f;

                // limit accelerations for a better stability
                if (maxG > 0 && force.magnitude > gForce.magnitude * maxG)
                {
                    force = force.normalized * gForce.magnitude * maxG;
                }
                // compensate gravity
                if (rigidBody.useGravity)
                {
                    force -= gForce;
                }
                rigidBody.AddForce(force);

                if (targetRot != null)
                {
                    //float degPerSec = 60f;
                    //rigidBody.rotation = Quaternion.RotateTowards(movedObject.transform.rotation, targetRot.Value, degPerSec/t);
                    rigidBody.rotation = targetRot.Value;
                }
            }
            else
            {
                Debug.LogWarning("MoveRigidbody() - missing rigidbody in object " + movedObject.name);
            }
        }
    }
}