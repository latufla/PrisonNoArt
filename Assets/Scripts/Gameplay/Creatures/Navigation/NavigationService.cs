using Unity.AI.Navigation;


namespace Honeylab.Gameplay.Creatures
{
    public class NavigationService
    {
        private readonly NavMeshSurface[] _surfaces;


        public NavigationService(NavMeshSurface[] surfaces)
        {
            _surfaces = surfaces;
        }


        public void Init()
        {
            Rebuild(_surfaces);
        }


        private static void Rebuild(NavMeshSurface[] surfaces)
        {
            foreach (NavMeshSurface surface in surfaces)
            {
                surface.BuildNavMesh();
            }
        }
    }
}
