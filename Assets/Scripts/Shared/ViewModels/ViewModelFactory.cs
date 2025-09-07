public static class ViewModelFactory {
    public static ViewModelRegistry<Entity, EntityViewModel> Entity = new(e => new EntityViewModel(e));
    public static ViewModelRegistry<GameState, GameStateViewModel> Game = new(e => new GameStateViewModel(e));
    public static ViewModelRegistry<ActiveBuff, ActiveBuffViewModel> ActiveBuff = new(e => new ActiveBuffViewModel(e));
}