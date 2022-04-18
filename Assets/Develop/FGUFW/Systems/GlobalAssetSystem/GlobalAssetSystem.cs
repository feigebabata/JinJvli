// using System;
// using UnityEngine;
// using UnityEngine.AddressableAssets;

// namespace FGUFW.Core
// {
//     public class GlobalAssetSystem : Singleton<GlobalAssetSystem>
//     {
//         [RuntimeInitializeOnLoadMethod]
//         static void runtimeInit()
//         {
//             I.ToString();
//         }

//         public override void Init()
//         {
//             GlobalMessenger.M.Add(GlobalMsgID.LoadAsset,LoadAsset);
//             GlobalMessenger.M.Add(GlobalMsgID.LoadScene,LoadScene);
//             GlobalMessenger.M.Add(GlobalMsgID.LoadPrefab,LoadPrefab);
//         }

//         public override void Dispose()
//         {
//             GlobalMessenger.M.Remove(GlobalMsgID.LoadAsset,LoadAsset);
//             GlobalMessenger.M.Remove(GlobalMsgID.LoadScene,LoadScene);
//             GlobalMessenger.M.Remove(GlobalMsgID.LoadPrefab,LoadPrefab);
//         }

//         private async void LoadAsset(object obj)
//         {
//             var loader = obj as AssetLoadHandel;
//             var result = await Addressables.LoadAssetAsync<UnityEngine.Object>(loader.Location).Task;
//             loader.Completed?.Invoke(result);
//         }

//         private async void LoadScene(object obj)
//         {
//             var loader = obj as SceneLoadHandel;
//             var result = await Addressables.LoadSceneAsync(loader.Location).Task;
//             loader.Completed?.Invoke();
//         }

//         private async void LoadPrefab(object obj)
//         {
//             var loader = obj as PrefabLoadHandel;
//             var result = await Addressables.InstantiateAsync(loader.Location,loader.Parent).Task;
//             loader.Completed?.Invoke(result);
//         }


//     }
// }