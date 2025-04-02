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

    public static class StatusWinlost
    {
        public const byte Draw = 0;

        public const byte HalfWin = 1;

        public const byte Win = 2;

        public const byte HalfLoss = 3;

        public const byte Loss = 4;

        public const byte Refund = 6;
    }

    public static class StatusName
    {
        public const string Won = "WON";

        public const string Draw = "DRAW";

        public const string Lose = "LOSE";

        public const string Refund = "Refund";
    }
}