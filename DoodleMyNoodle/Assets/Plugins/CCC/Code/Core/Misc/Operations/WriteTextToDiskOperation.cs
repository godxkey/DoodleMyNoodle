using System.Collections;
using System.Threading;

namespace CCC.Operations
{
    public class WriteTextToDiskOperation : CoroutineOperation
    {
        string _filePath;
        string _text;

        public WriteTextToDiskOperation(string text, string filePath)
        {
            _filePath = filePath;
            _text = text;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            string errorMessage = null;

            // save the data
            Thread thread = new Thread(() =>
            {
                try
                {
                    System.IO.File.WriteAllText(_filePath, _text);
                }
                catch (System.Exception e)
                {
                    errorMessage = e.Message;
                }
            });

            yield return thread.StartAndWaitForComplete();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                TerminateWithAbnormalFailure($"Failed to save text to file {_filePath} : {errorMessage}");
                yield break;
            }

            TerminateWithSuccess();
        }
    }

    public class WriteBytesToDiskOperation : CoroutineOperation
    {
        string _filePath;
        byte[] _data;

        public WriteBytesToDiskOperation(byte[] data, string filePath)
        {
            _filePath = filePath;
            _data = data;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            string errorMessage = null;

            // save the data
            Thread thread = new Thread(() =>
            {
                try
                {
                    System.IO.File.WriteAllBytes(_filePath, _data);
                }
                catch (System.Exception e)
                {
                    errorMessage = e.Message;
                }
            });

            yield return thread.StartAndWaitForComplete();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                TerminateWithAbnormalFailure($"Failed to save text to file {_filePath} : {errorMessage}");
                yield break;
            }

            TerminateWithSuccess();
        }
    }
}