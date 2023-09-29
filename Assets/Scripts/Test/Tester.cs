using System;
using System.Collections.Generic;
using System.Linq;
using Hsenl;
using MemoryPack;
using MemoryPack.Formatters;
using ProtoBuf;
using SimpleJSON;
using UnityEngine;
using Component = Hsenl.Component;
using Object = Hsenl.Object;
using Transform = Hsenl.Transform;

namespace Test {
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class TestAbility : TestSubstantive {
        [MemoryPackOrder(10)]
        public int configId;
    }

    [MemoryPackable(GenerateType.CircularReference)]
    public partial class TestCard : TestSubstantive {
        [MemoryPackOrder(10)]
        public TestSubstantive substantive;
    }

    public class Tester : MonoBehaviour {
        public TestSub1 testSub1;

        private void OnEnable() {
            // await ResourcesManager.LoadBundleAsync(Constant.ConfigBundleName);
            // // 加载配置文件
            // Tables.Instance = new Tables(s => {
            //     var textAsset = (TextAsset)ResourcesManager.GetAsset(Constant.ConfigBundleName, $"{s}");
            //     return JSON.Parse(textAsset.text);
            // });
            //
            // { // 关于循环引用与MemoryPackFormatterProvider.Register的关系
            //     // 结果: 即便TestSubstantive去掉了 abstract, 也得注册. 不知道为什么
            //     MemoryPackFormatterProvider.Register(new DynamicUnionFormatter<TestObject>(
            //         (0, typeof(TestEntity)),
            //         (1, typeof(TestComponent)),
            //         (2, typeof(TestSubstantive)),
            //         (3, typeof(TestAbility)),
            //         (4, typeof(TestCard))
            //     ));
            //
            //     MemoryPackFormatterProvider.Register(new DynamicUnionFormatter<TestComponent>(
            //         (0, typeof(TestSubstantive)),
            //         (1, typeof(TestAbility)),
            //         (2, typeof(TestCard))
            //     ));
            //
            //     MemoryPackFormatterProvider.Register(new DynamicUnionFormatter<TestSubstantive>(
            //         (1, typeof(TestAbility)),
            //         (2, typeof(TestCard))
            //     ));
            //
            //     var cardentity = new TestEntity();
            //     var card = new TestCard();
            //     cardentity.components.Add(card);
            //
            //     var abientity = new TestEntity();
            //     var ability = new TestAbility();
            //     abientity.components.Add(ability);
            //     card.substantive = ability;
            //     cardentity.entities.Add(abientity);
            //
            //     var bin = MemoryPackSerializer.Serialize((TestObject)cardentity);
            //     var newEntity = MemoryPackSerializer.Deserialize<TestObject>(bin) as TestEntity;
            //     var abi1 = newEntity?.entities[0].components[0] as TestAbility;
            //     var abi2 = (newEntity?.components[0] as TestCard)?.substantive as TestAbility;
            //     abi1.configId = 2;
            //     Debug.Log(abi2.configId);
            // }
        }

        private void Start() {
            SceneManager.LoadScene("main", LoadSceneMode.Single);
            Numerator.InitNumerator(3);
        }

        private void Update() {
            
        }
    }
}