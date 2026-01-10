namespace Oryx
{
    public delegate void GameAction();

    public delegate void GameAction<T>(T arg);
    
    public delegate void GameAction<T,TU>(T arg0, TU arg1);

}