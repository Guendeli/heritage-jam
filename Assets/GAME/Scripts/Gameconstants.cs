using Oryx;

namespace GAME.Scripts
{
    public static class Gameconstants
    {
        public const string MAIN_INVENTORY_NAME = "MainInventory";
        
        public static GameEvent<int> OnLevelUp = new GameEvent<int>();
    }
}