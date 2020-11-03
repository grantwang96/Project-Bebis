
namespace Winston {
    public static class ManagerMaster {
        // all dependencies here
        public static readonly ISceneController SceneController = new SceneController();
        public static readonly IPooledObjectManager PooledObjectManager = new PooledObjectManager();
        public static readonly IUIManager UIManager = new UIManager();
    }
}
