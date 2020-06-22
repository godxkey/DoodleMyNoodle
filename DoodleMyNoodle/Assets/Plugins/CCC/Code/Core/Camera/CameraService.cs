using System;
using UnityEngine;
using UnityEngineX;

public class CameraService : MonoCoreService<CameraService>
{
    private class CameraEntry : IComparable
    {
        public CameraEntry(CameraSet cameraSet, int priority) { CameraSet = cameraSet; Priority = priority; }
        public int Priority { get; set; }
        public CameraSet CameraSet { get; set; }

        public int CompareTo(object obj)
        {
            CameraEntry other = (CameraEntry)obj;
            return Priority.CompareTo(other.Priority);
        }
    }

    [SerializeField] BackupCamera backupCameraPrefab;

    AutoSortedList<CameraEntry> cameras = new AutoSortedList<CameraEntry>();

    public CameraSet ActiveCameraSet { get; private set; }
    public AudioListener ActiveAudioListener { get { return ActiveCameraSet.AudioListener; } }
    public Camera ActiveCamera { get { return ActiveCameraSet.Camera; } }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        if (BackupCamera.Instance == null)
        {
            Instantiate(backupCameraPrefab.gameObject);
        }

        ReevaluateActiveCamera();

        onComplete(this);
    }

    public void AdjustCameraPriority(Camera camera, int priority)
    {
        int index = GetCameraEntryIndexFromCamera(camera);
        CameraEntry cameraEntry = index != -1 ? cameras[index] : null;
        if (cameraEntry != null)
        {
            // we must remove it and re-add it to the list if we want the priority to apply
            cameras.RemoveAt(index);
            cameraEntry.Priority = priority;
            cameras.Add(cameraEntry);
            ReevaluateActiveCamera();
        }
        else
        {
            Log.Error("Failed to adjust camera priority. It appears it was not added to the camera service (" + camera.gameObject.name + ")");
        }
    }

    public void AddCamera(Camera camera, AudioListener audioListener, CameraSet.DeactivateMode deactivateMode, int priority)
    {
        AddCamera(new CameraSet(camera, audioListener, deactivateMode), priority);
    }
    public void AddCamera(CameraSet cameraSet, int priority)
    {
        if (Debug.isDebugBuild) // no need for this verif in release
        {
            if (GetCameraEntryFromCamera(cameraSet.Camera) != null)
            {
                Log.Error("Tried to add the same camera twice in the CameraService (" + cameraSet.Camera.gameObject.name + ")");
                return;
            }
        }
        cameras.Add(new CameraEntry(cameraSet, priority));
        
        if(!ReevaluateActiveCamera())
        {
            // deactivate new camera set if it was not chosen to be the new active one
            cameraSet.Deactivate();
        }
    }

    public void RemoveCamera(Camera camera)
    {
        int index = GetCameraEntryIndexFromCamera(camera);
        if (index != -1)
        {
            cameras.RemoveAt(index);
        }
        ReevaluateActiveCamera();
    }

    bool ReevaluateActiveCamera()
    {
        CameraSet newActiveCameraSet = GetCameraThatShouldBeActive();
        if (newActiveCameraSet != ActiveCameraSet)
        {
            ActiveCameraSet?.Deactivate();
            newActiveCameraSet.Activate();
            ActiveCameraSet = newActiveCameraSet;
            return true;
        }
        return false;
    }

    CameraEntry GetCameraEntryFromCamera(Camera camera)
    {
        int index = GetCameraEntryIndexFromCamera(camera);

        if (index != -1)
        {
            return cameras[index];
        }
        else
        {
            return null;
        }
    }

    int GetCameraEntryIndexFromCamera(Camera camera)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i].CameraSet.Camera == camera)
                return i;
        }
        return -1;
    }

    CameraSet GetCameraThatShouldBeActive()
    {
        if (cameras.Count > 0)
        {
            return cameras[cameras.Count - 1].CameraSet;
        }
        else
        {
            return BackupCamera.Instance.CameraSet;
        }
    }
}
