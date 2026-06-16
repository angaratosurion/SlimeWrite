namespace SlimeWrite
{
    public static class AndroidSaveFileDialogHandler
    {
        private static TaskCompletionSource<string?> _tcs;

        public static void SetTask(TaskCompletionSource<string?> tcs)
        {
            _tcs = tcs;
        }

        public static void HandleResult(Uri? uri)
        {
            _tcs?.TrySetResult(uri?.ToString());
        }
    }
}