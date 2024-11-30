using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public class GuideEditorWindow : EditorWindow {
        public string content;

        public static GuideEditorWindow Generate(string content) {
            var window = CreateWindow<GuideEditorWindow>();
            window.content = content;
            window.position = new Rect(100, 100, 600, 800); // 设置初始尺寸
            window.Show();
            return window;
        }

        private void OnGUI() {
            GUILayout.TextArea(this.content);
        }

        [MenuItem("Hsenl/指引/构建步骤")]
        private static void BuildProcess() {
            Generate("" +
                     "1、Hsenl/Compile and Copy..\n" +
                     "2、Yoo Assets构建\n" +
                     "3、打包\n" +
                     "注意: " +
                     "1、Launch场景里的Play Mode在打包时应该考虑设置为需要的选项" +
                     "2、记得把测试用的快捷键删除" +
                     "如果我们的热更程序集发生了改变, 比如修改了引用, 记得执行 HybridCLR/Generate/All\n" +
                     "如果运行时, 有关于泛型missing的报错, 看他缺的什么, 把其dll添加patch metadata list中,\n" +
                     "    list在HybridCLRCompileEditor脚本的CopyMetadataAotDll函数中\n" +
                     "如果是Build项目的话, 记得把Scene In Build列表除Launch以外的场景勾掉(非必须)\n" +
                     "" +
                     "" +
                     "");
        }
        
        [MenuItem("Hsenl/指引/热更步骤")]
        private static void HotUpdateProcess() {
            Generate("" +
                     "1、在编辑器无法测试代码热更, 只能打包才能测试代码热更\n" +
                     "2、除了代码, 其他资源都能在编辑器测试热更, 可以在本地搭个Web用于测试联网热更\n" +
                     "3、或者也可以把Launch和Entry场景里的热更模式改为offline模式, 用StreamingAssets文件夹里的资源来模拟热更" +
                     "");
        }

        [MenuItem("Hsenl/指引/手动项目迁移")]
        private static void Transfer() {
            Generate("手动迁移框架" +
                     "->  在unity根目录复制Luban文件夹" + "\n" +
                     "->  安装input system包" + "\n" +
                     "->  安装ai navigation包" + "\n" +
                     "->  安装hybrid clr" + "\n" +
                     "->  安装yoo asset" + "\n" +
                     "->  设置layer" + "\n" +
                     "->  添加一个text pro ui, 触发自动添加资源" + "\n" +
                     "->  复制plugins文件夹" + "\n" +
                     "->  复制scripts文件夹" + "\n" +
                     "->  创建Resources文件夹" + "\n" +
                     "->  复制art" + "\n" +
                     "->  复制bundles文件夹" + "\n" +
                     "->  复制scenes文件夹" + "\n" +
                     "->  执行 Hsenl/ScriptsGenerator/清空并生成Hsenl组件的Mono版 (非必要, 项目应该自带了)" + "\n" +
                     "->  Editor里面勾选Enter Play Mode Options, 下面两个子选项勾掉 (非必要)" + "\n" +
                     "" + "\n" +
                     "" + "\n" +
                     "热更流程: (总共分三部分 1、完全不热更的部分 2、入口 3、完全热更部分)" + "\n" +
                     "->  Launch脚本和Launch场景随打包一起打包, 所以是完全无法热更的部分, Launch只会做一件事, 就是尝试加载entry包." + "\n" +
                     "    包里需要名为Entry.dll的库, 需要库里有个名为Entry的类, 类里需要有个名为Start的静态函数, Launch会调用这个函数, 作为游戏的入口" + "\n" +
                     "    " + "\n" +
                     "->  EntryPackage这一步相当于是缓冲, 这一步存在的理由" + "\n" +
                     "    - 可以更灵活的定制化更新策略, 比如1.0版本, 游戏都是整包更新, 2.0, 我更新了entry包, 后续游戏就可以分包更新了" + "\n" +
                     "    - entry包一般都非常小, 可以无察觉的更新entry包" + "\n" +
                     "" + "\n" +
                     "->  完全热更部分, 这一步的所有内容都可以完全热更" + "\n" +
                     "->  理论上, 除了HybridCLR本体, 其他的程序集我们都能热更, 比如那些插件, yoo asset、ai nav、input system, 后续可以考虑把这些都搞成热更的");
        }
    }
}