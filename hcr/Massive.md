## hotween
C:\Users\用户名\AppData\Roaming\Unity\Asset Store
![](_v_images/20191014095331725_6431.png =333x)

### sequence的join函数可以尝试并联效果，要等并联中的最长动画结束才能播放后续的tweener


## richtext
<color=#ff0000>0</color>/0

## 进度条
001 UISlider

--002 003是image
002 if self.img_percent then
                self.img_percent:SetSizeDeltaX(self.img_percent_size_x);
            end
            
003 self.img_pass_progress_bar:SetFillAmount(__fill_amount);

## 多账号登陆
![](_v_images/20190821162831323_2772.png =902x)
![](_v_images/20190821162630668_23210.png =582x)

![](_v_images/20190822120138219_17483.png =892x)
# 1. 换装
获取皮肤 3850 （红旗）

2400
3200

## 1.1. 设置玩家皮肤
![](_v_images/20190820160024014_17786.png)

# 2. 表格

通用奖励表：TBX.general_reward 

道具表？：TBX.goods_mapping

# 3. 定时器
## 3.1. UI定时器
`
function t:Timeout(delay,cb,repeatCount)
	if delay and cb and self.cs then
		repeatCount = repeatCount or  1;
        return self.cs:SetTimeout(delay,cb,repeatCount);
    end
    return 0;
end
`

## 3.2. 全局定时器



# 4. 打包后处理
runtimemaker

# 5. 位处理从0开始

```
local flag = 3;
local result1 = not CS.LuaBits.BitIsZero(flag, 0);
```

# 6. 乐变根据version code检测更新
![](_v_images/20190805100911958_26431.png)