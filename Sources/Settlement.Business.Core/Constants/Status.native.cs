namespace Settlement.Business.Core.Constants;

public static class Status
{
    public static class StatusId
    {
        public const byte Running = 0;

        public const byte Waiting = 1;

        public const byte Reject = 101;

        public const byte Void = 102;

        public const byte Refund = 103;

        public const byte Draw = 111;

        public const byte Won = 112;

        public const byte Lose = 113;
    }

    public static class StatusName
    {
        public const string Won = "WON";

        public const string Draw = "DRAW";

        public const string Lose = "LOSE";
    }
}