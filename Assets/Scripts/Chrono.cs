public class Chrono
{
    public readonly int Minutes = 0;
    public readonly int Seconds = 0;
    public bool Finished => Minutes <= 0 && Seconds <= 0;

    private bool _stopped = true;
    public Chrono(int minutes, int seconds)
    {
        Minutes = minutes;
        Seconds = seconds;
    }
    public void Stop()
    {
        _stopped = true;
    }
    public void Play()
    {
        _stopped = false;
    }
    #region OPERATOR OVERLOADS (+, -, <, >, ++, --)
    public static Chrono operator +(Chrono a, Chrono b) {
        int newSeconds = (a.Seconds + b.Seconds);
        return new Chrono(a.Minutes + b.Minutes + newSeconds / 60, newSeconds % 60);
    }
    public static bool operator <(Chrono a, Chrono b) {
        if (a.Minutes < b.Minutes)
            return true;
        else if (a.Minutes > b.Minutes)
            return false;
        else
            return a.Seconds < b.Seconds;
    }
    public static bool operator >(Chrono a, Chrono b) {
        if (a.Minutes > b.Minutes)
            return true;
        else if (a.Minutes < b.Minutes)
            return false;
        else
            return a.Seconds > b.Seconds;
    }
    public static Chrono operator -(Chrono a, Chrono b) {
        if (b > a)
            return new Chrono(0, 0);
        int newSeconds = (a.Seconds - b.Seconds);
        int newMinutes = (a.Minutes - b.Minutes);
        if(newSeconds < 0)
        {
            newSeconds += 60;
            --newMinutes;
        }
        return new Chrono(newMinutes, newSeconds);
    }

    public static Chrono operator ++(Chrono a)
    {
        a += new Chrono(0, 1);
        return a;
    }
    public static Chrono operator --(Chrono a)
    {
        if (a._stopped)
            return a;
        a -= new Chrono(0, 1);
        return a;
    }
    #endregion
}
