namespace WaitAndChillReborn
{
    using Exiled.API.Interfaces;

    public sealed class Translation : ITranslation
    {
        public string TopMessage { get; set; } = "<size=40><color=yellow><b>The game will be starting soon, {seconds}</b></color></size>";

        public string BottomMessage { get; set; } = "<size=30><i>{players}</i></size>";

        public string ServerIsPaused { get; set; } = "Server is paused";

        public string RoundIsBeingStarted { get; set; } = "Round is being started";

        public string OneSecondRemain { get; set; } = "second remain";

        public string XSecondsRemains { get; set; } = "seconds remains";

        public string OnePlayerConnected { get; set; } = "player has connected";

        public string XPlayersConnected { get; set; } = "players have connected";
    }
}
