##  渲染顺序
![](_v_images/20191101120657564_12445.png =524x)
## 模拟协程：
```CSharp
    IEnumerator SimulateCoroutine(IEnumerator itorFunc)
    {
        Stack<IEnumerator> stack = new Stack<IEnumerator>();
        stack.Push(itorFunc);

        while (stack.Count > 0)
        {
            IEnumerator itor = stack.Peek();
            bool finished = true;

            //*****当前有yield return xx就会为进入 e.g.: yield return 0;
            while (itor.MoveNext())
            {
                //*****是yield return IEnumerator就会进入 e.g.: yield return Task2(true);
                if (itor.Current is IEnumerator)
                {
                    stack.Push((IEnumerator)itor.Current);
                    finished = false;

                    break;
                }


                yield return itor.Current;
            }

            if (finished)
            {

                stack.Pop();
            }
        }
    }
```

## UI元素跟随3D物体



# 性能统计
## 自动注入统计代码
http://www.xuanyusong.com/archives/4525?replytocom=633768