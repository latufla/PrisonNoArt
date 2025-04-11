using Cysharp.Threading.Tasks;
using System.Threading;


namespace Honeylab.Gameplay.Startup
{
    public interface ILevelStartup
    {
        UniTask InitAsync(CancellationToken ct);
        UniTask RunAsync(CancellationToken ct);
    }
}
