using System.Collections;
using System.Threading;

namespace CCC.Operations
{
    public class ReadTextFromDiskOperation : CoroutineOperation
    {
        string _filePath;

        public string TextResult;

        public ReadTextFromDiskOperation(string filePath)
        {
            _filePath = filePath;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            string errorMessage = null;

            // get data
            Thread loadThread = new Thread(() =>
            {
                try
                {
                    TextResult = System.IO.File.ReadAllText(_filePath);
                }
                catch (System.Exception e)
                {
                    errorMessage = e.Message;
                }
            });

            yield return loadThread.StartAndWaitForComplete();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                TerminateWithAbnormalFailure($"Failed to load text from file {_filePath} : {errorMessage}");
                yield break;
            }

            TerminateWithSuccess();
        }
    }
}