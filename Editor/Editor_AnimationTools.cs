using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SangsTools
{
    public class AnimatonTools
    {
        private delegate void ChangeAnimaClipData(GameObject obj, ref ModelImporterClipAnimation anima);
        [MenuItem("Tools/Animation/ChangeAnimaNameAndLoop")]
        private static void ChangeAnimaNameAndLoop()
        {
            Debug.Log("ChangeAnimaNameAndLoop");
            ChangeAnimationData((GameObject obj, ref ModelImporterClipAnimation animaClip) =>
                {
                    if (obj == null || animaClip == null) return;
                    string modleName = obj.name;
                    string clipName = animaClip.name;
                    string[] arrName = modleName.Split('_');
                    if (arrName == null || arrName.Length == 0)
                        return;

                    string willName = "";
                    int num = 0;//动画的序号

                for (int i = arrName.Length - 1; i >= 0; i--)
                    {
                        willName = arrName[i] + "_" + willName;
                        if (int.TryParse(arrName[i], out num))
                        {
                        //数字就是动画的序号
                        continue;
                        }
                        else
                        {
                        //不是数字说明是动画名字
                        break;
                        }
                    }

                    if (willName.Length > 0 && willName[willName.Length - 1] == '_')
                        willName = willName.Substring(0, willName.Length - 1);

                    animaClip.name = willName;
                    animaClip.loopTime = true;
                    animaClip.loopPose = false;
                });
        }


        [MenuItem("Tools/Animation/ChangeAnimaName_EndName")]
        private static void ChangeAnimaName_EndName()
        {
            Debug.Log("ChangeAnimaName_EndName");
            ChangeAnimationData((GameObject obj, ref ModelImporterClipAnimation animaClip) =>
            {
                if (obj == null || animaClip == null) return;
                string modleName = obj.name;
                string clipName = animaClip.name;
                string[] arrName = modleName.Split('_');
                if (arrName == null || arrName.Length == 0)
                    return;

                string willName = "";
                int num = 0;//动画的序号

            for (int i = arrName.Length - 1; i >= 0; i--)
                {
                    willName = arrName[i] + "_" + willName;
                    if (int.TryParse(arrName[i], out num))
                    {
                    //数字就是动画的序号
                    continue;
                    }
                    else
                    {
                    //不是数字说明是动画名字
                    break;
                    }
                }

                if (willName.Length > 0 && willName[willName.Length - 1] == '_')
                    willName = willName.Substring(0, willName.Length - 1);

                animaClip.name = willName;
            });
        }

        [MenuItem("Tools/Animation/ChangeAnima Loop true")]
        private static void ChangeAnima_Loop_True()
        {
            Debug.Log("ChangeAnima_Loop_True");
            ChangeAnimationData((GameObject obj, ref ModelImporterClipAnimation animaClip) =>
            {
                if (obj == null || animaClip == null) return;
                animaClip.loopTime = true;
                animaClip.loopPose = false;
            });
        }

        [MenuItem("Tools/Animation/ChangeAnima Loop ByName")]
        private static void ChangeAnima_Loop_ByName()
        {
            Debug.Log("ChangeAnima_Loop_ByName");
            ChangeAnimationData((GameObject obj, ref ModelImporterClipAnimation animaClip) =>
            {
                if (obj == null || animaClip == null) return;
                bool bLoop = animaClip.name.Contains("Idle") || animaClip.name.Contains("Move");
                animaClip.loopTime = bLoop;
                animaClip.loopPose = false;
            });
        }
        private static void ChangeAnimationData(ChangeAnimaClipData changeEvent)
        {
            if (changeEvent == null) return;

            //遍历所有选中的资源
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                //ֻ只操作游戏对象
                if (!(obj is GameObject)) continue;
                //获取路径
                string path = AssetDatabase.GetAssetPath(obj as GameObject);
                //unity的模型加载器
                ModelImporter modelImporter = ModelImporter.GetAtPath(path) as ModelImporter;

                //说明不是模型
                if (!modelImporter)
                    continue;
                //没有动画
                if (modelImporter.defaultClipAnimations == null || modelImporter.defaultClipAnimations.Length == 0)
                    continue;

                ModelImporterClipAnimation[] arrClip = new ModelImporterClipAnimation[modelImporter.defaultClipAnimations.Length];
                for (int i = 0; i < modelImporter.defaultClipAnimations.Length; i++)
                {
                    ModelImporterClipAnimation clipAnimation;
                    if (modelImporter.clipAnimations != null && modelImporter.clipAnimations.Length > i)
                        clipAnimation = modelImporter.clipAnimations[i];
                    else
                        clipAnimation = modelImporter.defaultClipAnimations[i];

                    // clipAnimation.name = getNewName(obj.name, clipAnimation.name);
                    changeEvent(obj as GameObject, ref clipAnimation);

                    arrClip[i] = clipAnimation;
                }

                modelImporter.clipAnimations = arrClip;

                EditorUtility.SetDirty(modelImporter);
                modelImporter.SaveAndReimport();
            }
            AssetDatabase.SaveAssets();
        }
    }
}