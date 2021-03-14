using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sangs.UGUITools
{
    [RequireComponent(typeof(CurvedTextData))]
    public class CurvedText : Text
    {
        private CurvedTextData _data;
        public CurvedTextData data
        {
            get
            {
                if (_data == null)
                    _data = GetComponent<CurvedTextData>();
                return _data;
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)//在Unity生成顶点数据后会调用这个函数
        {
            //toFill保存了Mesh的所有信息
            base.OnPopulateMesh(toFill);
            if (data.bCurved)
            {
                //在这个函数中改变顶点坐标，达到改变mesh形状的目的
                Curved(toFill);
            }
        }

        private void Curved(VertexHelper toFill)
        {
            if (!IsActive()) return;

            //遍历所有行
            int charIndex = 0;
            //TODO 文本框size不够自动换行的情况没处理
            string[] arrStrLine = text.Split('\n');

            for (int i = 0; i < cachedTextGenerator.lines.Count; i++)
            {
                UILineInfo lineInfo = cachedTextGenerator.lines[i];
                if (i + 1 < cachedTextGenerator.lines.Count)
                {
                    //不是最后一行
                    UILineInfo lineNext = cachedTextGenerator.lines[i + 1];
                    int curCharIndex = 0;//一行的第几个字
                    for (int j = lineInfo.startCharIdx; j < lineNext.startCharIdx - 1; j++)//遍历一行所有文字 ，下一行起点为界限
                    {
                        // Curved(toFill, j, curCharIndex++, i, lineNext.startCharIdx - 1 - lineInfo.startCharIdx);
                        Curved(toFill, ref charIndex, curCharIndex++, i, lineNext.startCharIdx - 1 - lineInfo.startCharIdx, arrStrLine[i]);
                    }
                }
                else
                {
                    int curCharIndex = 0;//一行的第几个字
                    for (int j = lineInfo.startCharIdx; j < cachedTextGenerator.characterCountVisible; j++)
                    {
                        // Curved(toFill, j, curCharIndex++, i, cachedTextGenerator.characterCountVisible - lineInfo.startCharIdx);
                        Curved(toFill, ref charIndex, curCharIndex++, i, cachedTextGenerator.characterCountVisible - lineInfo.startCharIdx, arrStrLine[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="charIndex">该字的总的序号</param>
        /// <param name="charXIndex">该字在这一行的序号</param>
        /// <param name="charYStep">当前是第几行</param>
        /// <param name="lineCount">这一行总的字数</param>
        private void Curved(VertexHelper helper, ref int charIndex, int charXIndex, int charYStep, int lineCount, string textLine)
        {
            if (textLine[charXIndex] == ' ' || textLine[charXIndex] == '\n') return;

            UIVertex lt = new UIVertex(); //lb 左下  lt左上  rt 右上 ，rb右下
            UIVertex rt = new UIVertex();
            UIVertex rb = new UIVertex();
            UIVertex lb = new UIVertex();

            try
            {
                //获取点的信息 index一行的第几个文字  ，一个文字4个点组成 
                helper.PopulateUIVertex(ref lt, charIndex * 4);
                helper.PopulateUIVertex(ref rt, charIndex * 4 + 1);
                helper.PopulateUIVertex(ref rb, charIndex * 4 + 2);
                helper.PopulateUIVertex(ref lb, charIndex * 4 + 3);
            }
            catch (System.Exception)
            {
                return;
            }

            //文字的中心
            Vector3 center = Vector3.Lerp(lt.position, rb.position, 0.5f);
            //曲面圆心坐标,局部坐标系中心点位zero
            Vector3 curvedCenter = Vector3.zero - Vector3.forward * data.fCurvedCenterDis;
            //文字到中心点的水平距离（负数在左边，正数在右边）
            float disToCenter = center.x;
            float rotateAngle = disToCenter / (2 * Mathf.PI * data.fCurvedCenterDis) * 360f;

            rotateAngle *= data.fDisCoefficient;//紧密系数

            //用来旋转的矩阵
            Quaternion rotate = Quaternion.AngleAxis(rotateAngle, transform.up);

            //先回到原点
            Vector3[] pos = new Vector3[]{
            lt.position-Vector3.right*disToCenter,
            rt.position-Vector3.right*disToCenter,
            rb.position-Vector3.right*disToCenter,
            lb.position-Vector3.right*disToCenter,
        };

            //取曲面中心点到4个顶点的向量
            Vector3 dir_LT = pos[0] - curvedCenter;
            Vector3 dir_RT = pos[1] - curvedCenter;
            Vector3 dir_RB = pos[2] - curvedCenter;
            Vector3 dir_LB = pos[3] - curvedCenter;

            //旋转向量，求旋转后点
            pos[0] = curvedCenter + (rotate * dir_LT);
            pos[1] = curvedCenter + (rotate * dir_RT);
            pos[2] = curvedCenter + (rotate * dir_RB);
            pos[3] = curvedCenter + (rotate * dir_LB);

            lt.position = pos[0];
            rt.position = pos[1];
            rb.position = pos[2];
            lb.position = pos[3];

            helper.SetUIVertex(lt, charIndex * 4);    //刷新4个顶点
            helper.SetUIVertex(rt, charIndex * 4 + 1);
            helper.SetUIVertex(rb, charIndex * 4 + 2);
            helper.SetUIVertex(lb, charIndex * 4 + 3);

            charIndex++;
        }
    }
}