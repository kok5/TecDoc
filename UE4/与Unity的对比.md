### 通过 GameObject / Actor 访问组件
MyActor.Tags.AddUnique(TEXT("MyTag"));

### 通过 GameObject / Actor 访问组件
**Unity**
MyComponent MyComp = gameObject.GetComponent<MyComponent>();
**UE4**
UMyComponent* MyComp = MyActor->FindComponentByClass<UMyComponent>();

### Instantiating GameObject / Spawning Actor

**Unity**
MyScriptableObject NewSO = ScriptableObject.CreateInstance<MyScriptableObject>();
**UE4**
UMyObject* NewObj = NewObject<UMyObject>();


### 销毁 GameObject / Actor
**Unity**
Destroy(MyGameObject);
**UE4**
MyActor->Destroy();

### 销毁 GameObject / Actor（1 秒延迟）
**Unity**
Destroy(MyGameObject, 1);
**UE4**
MyActor->SetLifeSpan(1);

### 禁用 GameObjects / Actors
**Unity**
MyGameObject.SetActive(false);
**UE4**
// Hides visible components
MyActor->SetActorHiddenInGame(true);

// Disables collision components
MyActor->SetActorEnableCollision(false);

// Stops the Actor from ticking
MyActor->SetActorTickEnabled(false);

    if endExp > startExp then
        if self.txt_fans_num then
            self.txt_fans_num:DONumber(self.ResultData.exp_base or 0, self.ResultData.gen_exp or 0, self.t4);
        end

        local tweener = CS.LuaCodeBridge.To(function ( t )        
            --self.txt_coin:SetText(""..math.floor( t ))
            local curLevel = self:__GetLvByExp(t);
            self:SetExpInfo(t, curLevel);
            if curLevel > (self.hasSetImageLvl or 0) then
                self:PlayLevelUpEffect(curLevel);
                self.hasSetImageLvl = curLevel;
            end
        end,  startExp, endExp, self.t4);
    end

```C++
APawn* AMyPlayerController::FindPawnCameraIsLookingAt()
{
    // You can use this to customize various properties about the trace
    FCollisionQueryParams Params;
    // Ignore the player's pawn
    Params.AddIgnoredActor(GetPawn());

    // The hit result gets populated by the line trace
    FHitResult Hit;

    // Raycast out from the camera, only collide with pawns (they are on the ECC_Pawn collision channel)
    FVector Start = PlayerCameraManager->GetCameraLocation();
    FVector End = Start + (PlayerCameraManager->GetCameraRotation().Vector() * 1000.0f);
    bool bHit = GetWorld()->LineTraceSingle(Hit, Start, End, ECC_Pawn, Params);

    if (bHit)
    {
        // Hit.Actor contains a weak pointer to the Actor that the trace hit
        return Cast<APawn>(Hit.Actor.Get());
    }

    return nullptr;
}
```
