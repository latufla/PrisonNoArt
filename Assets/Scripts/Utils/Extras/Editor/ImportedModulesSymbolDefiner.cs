using UnityEditor;


namespace Honeylab.Utils.Editor
{
    [InitializeOnLoad]
    public class ImportedModulesSymbolDefiner
    {
        static ImportedModulesSymbolDefiner()
        {
            TypeExistenceSymbolDefiner[] definers =
            {
                new TypeExistenceSymbolDefiner("DG.Tweening.DOTween", "HONEYLAB_DOTWEEN_IMPORTED"),
                new TypeExistenceSymbolDefiner(
                    "Opsive.UltimateCharacterController.Character.UltimateCharacterLocomotion",
                    "HONEYLAB_OPSIVE_UCC_IMPORTED"),
                new TypeExistenceSymbolDefiner("UnityEngine.AddressableAssets.Addressables",
                    "HONEYLAB_ADDRESSABLES_IMPORTED"),
                new TypeExistenceSymbolDefiner("UniRx.Observable",
                    "HONEYLAB_UNIRX_IMPORTED"),
                new TypeExistenceSymbolDefiner("Zenject.ProjectContext",
                    "HONEYLAB_ZENJECT_IMPORTED"),
                new TypeExistenceSymbolDefiner("Cysharp.Threading.Tasks.UniTask",
                    "HONEYLAB_UNITASK_IMPORTED"),
                new TypeExistenceSymbolDefiner("Cinemachine.CinemachineBrain",
                    "HONEYLAB_CINEMACHINE_IMPORTED"),
                new TypeExistenceSymbolDefiner("MoreMountains.NiceVibrations.MMVibrationManager",
                    "HONEYLAB_NICEVIBRATIONS_IMPORTED")
            };

            foreach (TypeExistenceSymbolDefiner definer in definers)
            {
                definer.Run();
            }
        }
    }
}
