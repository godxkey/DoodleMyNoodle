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

        public bool IsValid => _owner != null;
        public float GravityScale { get => _trajectory.GravityScale; set => _trajectory.GravityScale = value; }
        public bool Displayed { get => _trajectory.Displayed; set => _trajectory.Displayed = value; }
        public Vector2 StartPoint { get => _trajectory.StartPoint; set => _trajectory.StartPoint = value; }
        public Vector2 Velocity { get => _trajectory.Velocity; set => _trajectory.Velocity = value; }
        public float Length { get => _trajectory.Length; set => _trajectory.Length = value; }
        public float Radius { get => _trajectory.Radius; set => _trajectory.Radius = value; }

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
        public float GravityScale { get; set; }
        public float Radius { get; set; }

        public Trajectory()
        {
            ResetToDefault();
        }

        public void ResetToDefault()
        {
            StartPoint = Vector2.zero;
            Velocity = Vector2.zero;
            Length = 5;
            Displayed = true;
            GravityScale = 1;
            Radius = 1;
        }

        public float CalculateTravelDuration(float traveledDistance, Vector2 gravity)
            => mathX.Trajectory.TravelDurationApprox(Velocity, gravity * GravityScale, traveledDistance);

        public Vector2 CalculatePosition(float time, Vector2 gravity)
            => mathX.Trajectory.Position(StartPoint, Velocity, gravity * GravityScale, time);
    }

    private class LineManager
    {
        private readonly LineRenderer _prefab;
        private readonly Transform _container;
        private readonly List<LineRenderer> _lines = new List<LineRenderer>();

        private List<Vector3> _segments = new List<Vector3>();
        private int _lineIterator = 0;

        public LineManager(LineRenderer prefab, Transform container)
        {
            _prefab = prefab;
            _container = container;
        }

        public void BeginLines()
        {
            _lineIterator = 0;
        }

        public void SetSegment(Vector2 point)
        {
            _segments.Add(point);
        }

        public void FinishLine(float radius)
        {
            if (_segments.Count == 0)
                return;

            if (_lineIterator >= _lines.Count)
                SpawnLine();

            _lines[_lineIterator].positionCount = _segments.Count;
            _lines[_lineIterator].SetPositions(_segments.ToArray());
            _lines[_lineIterator].startWidth = radius * 2f;
            _lines[_lineIterator].endWidth = radius * 2f;
            _lines[_lineIterator].gameObject.SetActive(true);

            _lineIterator++;
            _segments.Clear();
        }

        public void EndLines()
        {
            while (_lineIterator < _lines.Count)
            {
                _lines[_lineIterator].gameObject.SetActive(false);
                _lineIterator++;
            }
        }

        private void SpawnLine()
        {
            LineRenderer instance = Instantiate(_prefab, _container);
            _lines.Add(instance);
        }
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
    [SerializeField] private float _pointFrequencySpeedMultiplier = 10;
    [SerializeField] private float _pointFrequencyGravScaleMultiplier = 10;
    [SerializeField] private int _pointCap = 400;
    [SerializeField] private float _basePointAlpha = 0.75f;
    [SerializeField] private List<Trajectory> _trajectories = new List<Trajectory>();
    [SerializeField] private LineRenderer _lineRendererPrefab = null;

    private PointManager _pointManager;
    private LineManager _lineManager;

    protected override void Awake()
    {
        base.Awake();

        _pointManager = new PointManager(_pointSpritePrefabs, _pointContainer, _pointCap);
        _lineManager = new LineManager(_lineRendererPrefab, _pointContainer);
    }

    protected override void OnGamePresentationUpdate()
    {
        Vector2 gravity = Vector2.down * 9.8f;

        if (SimWorld.HasSingleton<PhysicsStepSettings>())
        {
            gravity = SimWorld.GetSingleton<PhysicsStepSettings>().Gravity;
        }

        _pointManager.BeginPoints();
        _lineManager.BeginLines();

        foreach (var trajectory in _trajectories)
        {
            if (!trajectory.Displayed)
                continue;

            float frequency = _pointBaseFrequency + (trajectory.Velocity.magnitude * _pointFrequencySpeedMultiplier) + (trajectory.GravityScale * _pointFrequencyGravScaleMultiplier);
            float travelDuration = math.min(trajectory.CalculateTravelDuration(trajectory.Length, gravity), 4f);
            float pCount = travelDuration * frequency;

            for (float p = 0; p < pCount; p++)
            {
                Vector2 pointPosition = trajectory.CalculatePosition(p / frequency, gravity);
                float alpha = math.min(pCount - p, 1);
                _pointManager.SetPoint(pointPosition, alpha * _basePointAlpha);
                _lineManager.SetSegment(pointPosition);
            }

            _lineManager.FinishLine(trajectory.Radius);

            if (_pointManager.ReachedLimit)
                break;
        }

        _lineManager.EndLines();
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