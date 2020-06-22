using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

namespace CCC.Operations
{
    public abstract class CoroutineOperation
    {
        public bool IsRunning { get; private set; }
        public bool HasSucceeded { get; private set; }
        public bool HasFailed => _hasRun && !IsRunning && !HasSucceeded;
        public Action<CoroutineOperation> OnTerminateCallback;
        public Action<CoroutineOperation> OnSucceedCallback;
        public Action<CoroutineOperation> OnFailCallback;
        public string Message { get; private set; }
        public bool LogTerminationMessage = true;

        public enum LogFlag
        {
            None = 0,
            Success = 1 << 0,
            NormalFailure = 1 << 1,
            AbnormalFailure = 1 << 2,
            All = Success | NormalFailure | AbnormalFailure
        }
        public LogFlag LogFlags = LogFlag.NormalFailure | LogFlag.AbnormalFailure;

        private bool _hasRun = false;
        private Coroutine _coroutine;
        private List<CoroutineOperation> _ongoingSubOperations;
        private readonly List<IDisposable> _disposeOnTerminate = new List<IDisposable>();

        public T DisposeOnTerminate<T>(T disposable) where T : IDisposable
        {
            _disposeOnTerminate.Add(disposable);

            return disposable;
        }

        public void Execute()
        {
            if (IsRunning)
            {
                Debug.LogError("Trying to execute an already running operation");
                return;
            }

            IsRunning = true;
            _hasRun = true;

            // Start operation routine
            IEnumerator startedRoutine = ExecuteRoutine();

            // tell unity to continue to run the routine
            // (we need to check if the operation is still running because 'ExecuteRoutine' might have terminated it immidiately)
            if (IsRunning)
            {
                _coroutine = CoroutineLauncherService.Instance.StartCoroutine(startedRoutine);
            }
        }

        public void TerminateWithSuccess(string message = null)
        {
            Terminate_Internal(success: true, message, LogFlag.Success);
        }

        public void TerminateWithNormalFailure(string message = null)
        {
            Terminate_Internal(success: false, message, LogFlag.NormalFailure);
        }

        public void TerminateWithAbnormalFailure(string message = null)
        {
            Terminate_Internal(success: false, message, LogFlag.AbnormalFailure);
        }

        void Terminate_Internal(bool success, string message, LogFlag requiredLogFlagToLog)
        {
            if (!IsRunning)
            {
                Debug.LogError("Trying to terminate an already terminated operation");
                return;
            }
            IsRunning = false;
            HasSucceeded = success;
            Message = message;

            // terminate ongoing sub operations
            if (_ongoingSubOperations != null)
            {
                for (int i = 0; i < _ongoingSubOperations.Count; i++)
                {
                    if (_ongoingSubOperations[i].IsRunning)
                        _ongoingSubOperations[i].Terminate_Internal(success, null, requiredLogFlagToLog);
                }
                _ongoingSubOperations.Clear();
            }

            if (success)
            {
                OnSucceed();
                OnSucceedCallback?.InvokeCatchException(this);
            }
            else
            {
                OnFail();
                OnFailCallback?.InvokeCatchException(this);
            }

            if (!string.IsNullOrEmpty(message) && (LogFlags & requiredLogFlagToLog) != LogFlag.None)
            {
                switch (requiredLogFlagToLog)
                {
                    case LogFlag.Success:
                        Log.Info(message);
                        break;
                    case LogFlag.NormalFailure:
                        Log.Info(message);
                        break;
                    
                    default:
                    case LogFlag.AbnormalFailure:
                        Log.Error(message);
                        break;
                }
            }

            // tell unity to stop maintaning and running the coroutine
            if (_coroutine != null)
            {
                CoroutineLauncherService.Instance?.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            for (int i = 0; i < _disposeOnTerminate.Count; i++)
            {
                _disposeOnTerminate[i].Dispose();
            }

            OnTerminate();
            OnTerminateCallback?.InvokeCatchException(this);
        }

        protected IEnumerator ExecuteSubOperationAndWaitForSuccess(CoroutineOperation subOp)
        {
            bool subOpLaunchedCorrectly = Pre_ExecuteSubOperation(subOp);

            if (!subOpLaunchedCorrectly)
            {
                Terminate_Internal(success: false, subOp.Message, LogFlag.None); // don't log sub-operation failure
                yield break;
            }

            while (!subOp.HasSucceeded)
            {
                // terminate if sub-operation fails
                if (subOp.HasFailed)
                {
                    Terminate_Internal(success: false, subOp.Message, LogFlag.None); // don't log sub-operation failure
                    break;
                }
                yield return null;
            }

            Post_ExecuteSubOperation(subOp);
        }

        protected IEnumerator ExecuteSubOperationAndWaitForTerminate(CoroutineOperation subOp)
        {
            bool subOpLaunchedCorrectly = Pre_ExecuteSubOperation(subOp);

            if (!subOpLaunchedCorrectly)
            {
                Terminate_Internal(success: false, subOp.Message, LogFlag.None); // don't log sub-operation failure
                yield break;
            }

            while (subOp.IsRunning)
            {
                yield return null;
            }

            Post_ExecuteSubOperation(subOp);
        }

        bool Pre_ExecuteSubOperation(CoroutineOperation op)
        {
            if(op == null)
            {
                return false;
            }

            // execute sub-operation if necessary
            if (!op._hasRun)
                op.Execute();

            if (_ongoingSubOperations == null)
                _ongoingSubOperations = new List<CoroutineOperation>();
            _ongoingSubOperations.Add(op);

            return true;
        }
        void Post_ExecuteSubOperation(CoroutineOperation op)
        {
            _ongoingSubOperations.Remove(op);
        }

        protected abstract IEnumerator ExecuteRoutine();
        protected virtual void OnSucceed() { }
        protected virtual void OnFail() { }
        protected virtual void OnTerminate() { }
    }
}