### 遍历可变参数

```lua
local mt = {}
--__call的第一参数是表自己
mt.__call = function(mytable,...)
    --输出所有参数
    for _,v in ipairs{...} do
        print(v)
    end
end

t = {}
setmetatable(t,mt)
--将t当作一个函数调用
t(1,2,3)
```