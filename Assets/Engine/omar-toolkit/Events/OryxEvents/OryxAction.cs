namespace Oryx
{
    public delegate void OryxAction();

    public delegate void OryxAction<T>(T arg);
    public delegate void OryxAction<T1,T2>(T1 arg, T2 arg2);
}