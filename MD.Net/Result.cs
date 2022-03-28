namespace MD.Net
{
    public class Result : IResult
    {
        public Result(string message, ResultStatus status)
        {
            this.Message = message;
            this.Status = status;
        }

        public string Message { get; private set; }

        public ResultStatus Status { get; private set; }

        public static IResult Success
        {
            get
            {
                return new Result(string.Empty, ResultStatus.Success);
            }
        }

        public static IResult Failure(string message)
        {
            return new Result(message, ResultStatus.Failure);
        }
    }
}
