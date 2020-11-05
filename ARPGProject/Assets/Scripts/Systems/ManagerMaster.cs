using Bebis;

namespace Winston {
    public static class ManagerMaster {
        // all dependencies here
        public static readonly ISceneController SceneController = new SceneController();
        public static readonly IPooledObjectManager PooledObjectManager = new PooledObjectManager();
        public static readonly IUIManager UIManager = new UIManager();
        public static readonly ICharactersManager CharactersManager = new CharactersManager();
        public static readonly ILevelDataManager LevelDataManager = new LevelDataManager();
    }
}
