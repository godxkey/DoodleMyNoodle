using CCC.Fix2D;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngineX;

public class TrajectoryDisplaySystem : GamePresentationSystem<TrajectoryDisplaySystem>
{
    public struct TrajectoryHandle
    {
        private TrajectoryDisplaySystem _owner;
        private Trajectory _trajectory;

        public TrajectoryHandle(TrajectoryDisplaySystem owner, Trajectory trajectory)
        {
            _owner = owner;
            _trajectory = trajectory;
        }

        public bool Displayed { get => _trajectory.Displayed; set => _trajectory.Displayed = value; }
        public Vector2 StartPoint { get => _trajectory.StartPoint; set => _trajectory.StartPoint = value; }
        public Vector2 Velocity { get => _trajectory.Velocity; set => _trajectory.Velocity = value; }
        public float Length { get => _trajectory.Length; set => _trajectory.Length = value; }

        public void Dispose()
        {
            if (_owner != null)
                _owner.DestroyTrajectory(_trajectory);
            _owner = null;
        }
    }

    public class Trajectory
    {
        public Vector2 StartPoint { get; set; }
        public Vector2 Velocity { get; set; }
        public float Length { get; set; }
        public bool Displayed { get; set; }

        public float CalculateTravelDuration(float traveledDistance, Vector2 gravity) => mathX.Trajectory.TravelDurationApprox(Velocity, gravity, traveledDistance);
        public Vector2 CalculatePosition(float time, Vector2 gravity) => mathX.Trajectory.Position(StartPoint, Velocity, gravity, time);
    }

    private class PointManager
    {
        private readonly TrajectoryDisplayPoint[] _prefabs;
        private readonly Transform _container;
        private readonly int _pointCap;
        private readonly List<Transform> _pointTransforms = new List<Transform>();
        private readonly List<TrajectoryDisplayPoint> _points = new List<TrajectoryDisplayPoint>();

        private int _iterator = 0;
        private int _prefabIterator = 0;

        public bool ReachedLimit => _iterator >= _pointCap;

        public PointManager(TrajectoryDisplayPoint[] prefabs, Transform container, int pointCap)
        {
            _prefabs = prefabs;
            _container = container;
            _pointCap = pointCap;
        }

        public void BeginPoints()
        {
            _iterator = 0;
        }

        public void SetPoint(Vector2 position, float alpha)
        {
            if (ReachedLimit)
                return;

            if (_iterator >= _points.Count)
            {
                SpawnPoint();
            }

            _pointTransforms[_iterator].position = position;
            _points[_iterator].SetAlpha(alpha);
            _points[_iterator].gameObject.SetActive(true);

            _iterator++;
        }

        private void SpawnPoint()
        {
            TrajectoryDisplayPoint prefab = _prefabs[_prefabIterator++ % _prefabs.Length];
            TrajectoryDisplayPoint instance = Instantiate(prefab, _container);
            Transform transform = instance.transform;

            // randomize rotation
            transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));

            _points.Add(instance);
            _pointTransforms.Add(transform);
        }

        public void EndPoints()
        {
            while (_iterator < _points.Count)
            {
                _points[_iterator].gameObject.SetActive(false);
                _iterator++;
            }
        }
    }

    [SerializeField] private TrajectoryDisplayPoint[] _pointSpritePrefabs;
    [SerializeField] private Transform _pointContainer;
    [SerializeField] private float _pointBaseFrequency = 10;
    [SerializeField] private float _pointFrequencyMultiplier = 10;
    [SerializeField] private int _pointCap = 400;
    [SerializeField] private List<Trajectory> _trajectories = new List<Trajectory>();

    private PointManager _pointManager;

    protected override void Awake()
    {
        base.Awake();

        _pointManager = new PointManager(_pointSpritePrefabs, _pointContainer, _pointCap);
    }

    protected override void OnGamePresentationUpdate()
    {
        Vector2 gravity = Vector2.down * 9.8f;

        if (SimWorld.HasSingleton<PhysicsStepSettings>())
        {
            gravity = SimWorld.GetSingleton<PhysicsStepSettings>().Gravity;
        }

        _pointManager.BeginPoints();
        foreach (var trajectory in _trajectories)
        {
            if (!trajectory.Displayed)
                continue;

            float frequency = _pointBaseFrequency + (trajectory.Velocity.magnitude * _pointFrequencyMultiplier);
            float travelDuration = math.min(trajectory.CalculateTravelDuration(trajectory.Length, gravity), 4f);
            float pCount = travelDuration * frequency;

            for (float p = 0; p < pCount; p++)
            {
                Vector2 pointPosition = trajectory.CalculatePosition(p / frequency, gravity);
                float alpha = math.min(pCount - p, 1);
                _pointManager.SetPoint(pointPosition, alpha);
            }

            if (_pointManager.ReachedLimit)
                break;
        }
        _pointManager.EndPoints();
    }


    public TrajectoryHandle CreateTrajectory()
    {
        var traj = new Trajectory();
        _trajectories.Add(traj);
        return new TrajectoryHandle(this, traj);
    }

    private void DestroyTrajectory(Trajectory trajectory)
    {
        _trajectories.Remove(trajectory);
    }
}