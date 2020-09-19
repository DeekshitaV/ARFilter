using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;

/// <summary>
/// This component uses the ARCoreFaceSubsystem to query for face regions, special
/// regions detected within a face, such as the nose tip. Each region has a pose
/// associated with it. This component instantiates <see cref="regionPrefab"/>
/// at each detected region.
/// </summary>
[RequireComponent(typeof(ARFaceManager))]
[RequireComponent(typeof(ARSessionOrigin))]
public class FaceRegions : MonoBehaviour
{
    public GameObject eyebrow;
    public GameObject nose;

    ARFaceManager m_FaceManager;
    ARSessionOrigin m_SessionOrigin;

    NativeArray<ARCoreFaceRegionData> m_FaceRegions;
    Dictionary<TrackableId, Dictionary<ARCoreFaceRegion, GameObject>> m_InstantiatedPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        m_FaceManager = GetComponent<ARFaceManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_InstantiatedPrefabs = new Dictionary<TrackableId, Dictionary<ARCoreFaceRegion, GameObject>>();
    } 


    // Update is called once per frame
    void Update()
    {
        var subsystem = (ARCoreFaceSubsystem)m_FaceManager.subsystem;
        if (subsystem == null)
            return;

        foreach (var face in m_FaceManager.trackables)
        {
            Dictionary<ARCoreFaceRegion, GameObject> regionGos;
            if (!m_InstantiatedPrefabs.TryGetValue(face.trackableId, out regionGos))
            {
                regionGos = new Dictionary<ARCoreFaceRegion, GameObject>();
                m_InstantiatedPrefabs.Add(face.trackableId, regionGos);
            }

            subsystem.GetRegionPoses(face.trackableId, Allocator.Persistent, ref m_FaceRegions);
            for (int i = 0; i < m_FaceRegions.Length; ++i)
            {
                var regionType = m_FaceRegions[i].region;

                GameObject go;
                if (!regionGos.TryGetValue(regionType, out go))
                {
                    if ((int)regionType == 1 || (int)regionType == 2)
                    {
                        go = Instantiate(eyebrow, m_SessionOrigin.trackablesParent);
                    }
                    else
                    {
                        go = Instantiate(nose, m_SessionOrigin.trackablesParent);
                    }
                    
                    regionGos.Add(regionType, go);
                }

                go.transform.localPosition = m_FaceRegions[i].pose.position;
                go.transform.localRotation = m_FaceRegions[i].pose.rotation;
            }
        }
    }

    void OnDestroy()
    {
        if (m_FaceRegions.IsCreated)
            m_FaceRegions.Dispose();
    }
}
