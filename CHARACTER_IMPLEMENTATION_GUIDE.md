# WFRP Character Implementation Guide
*Complete guide for implementing character characteristics system*

## 🎯 Overview

You have a well-structured WFRP 4th Edition character sheet with:
- **Base characteristics** (SpinBox inputs: _wsInput, _bsInput, etc.)
- **Advances** (SpinBox inputs: _wsInputAdv, _bsInputAdv, etc.) 
- **Modifiers** (SpinBox inputs: _wsInputMod, _bsInputMod, etc.)
- **Current values** (Label displays: _wsInputCur, _bsInputCur, etc.)

## 📊 WFRP Characteristics System

### The Formula
```
Current = Base + Advances + Modifiers
```

**Example:**
- **Base WS**: 35 (starting value)
- **Advances**: +10 (from experience/career)
- **Modifiers**: -5 (from injury/condition)
- **Current WS**: 40 (35 + 10 - 5)

## 🏗️ Implementation Strategy

### 1. **Client-Side Calculation (RECOMMENDED)**

**Why Client-Side?**
- ✅ **Instant feedback** - no server calls needed
- ✅ **Better UX** - real-time updates as user types
- ✅ **Reduced server load** - calculations happen locally
- ✅ **Simpler logic** - just math, no database updates

**Implementation Steps:**

#### A. Connect Value Change Events
```csharp
private void SetupCharacteristicBindings()
{
    // WS (Weapon Skill)
    _wsInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("WS");
    _wsInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("WS");
    _wsInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("WS");
    
    // BS (Ballistic Skill) 
    _bsInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("BS");
    _bsInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("BS");
    _bsInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("BS");
    
    // Repeat for all 10 characteristics...
}
```

#### B. Create Update Method
```csharp
private void UpdateCurrentCharacteristic(string characteristic)
{
    switch (characteristic)
    {
        case "WS":
            _wsInputCur.Text = ((int)(_wsInput.Value + _wsInputAdv.Value + _wsInputMod.Value)).ToString();
            break;
        case "BS":
            _bsInputCur.Text = ((int)(_bsInput.Value + _bsInputAdv.Value + _bsInputMod.Value)).ToString();
            break;
        // Add all 10 characteristics...
    }
}
```

#### C. Call Setup in _Ready()
```csharp
public override void _Ready()
{
    SetupUI();
    SetupCharacteristicBindings(); // Add this line
    _httpRequest = GetNode<HttpRequest>("HTTPRequest");
    _httpRequest.RequestCompleted += OnRequestCompleted;
}
```

### 2. **Data Flow Architecture**

```
User Input (SpinBox) 
    ↓
ValueChanged Event 
    ↓
UpdateCurrentCharacteristic() 
    ↓
Update Label Display 
    ↓
Save to Database (OnSavePressed)
```

### 3. **Saving Strategy**

**Current Save Method Issues:**
- ❌ Only saves **base values** to database
- ❌ Missing **advances** and **modifiers**

**Database Schema Updates Needed:**
```sql
-- Add these columns to Character table
ALTER TABLE Characters ADD COLUMN WeaponSkillAdv INTEGER DEFAULT 0;
ALTER TABLE Characters ADD COLUMN WeaponSkillMod INTEGER DEFAULT 0;
ALTER TABLE Characters ADD COLUMN BallisticSkillAdv INTEGER DEFAULT 0;
ALTER TABLE Characters ADD COLUMN BallisticSkillMod INTEGER DEFAULT 0;
-- ... repeat for all characteristics
```

**Updated Save Method:**
```csharp
private void OnSavePressed()
{
    var characterData = new
    {
        id = _currentCharacterId,
        userId = _currentUserId,
        
        // Base characteristics
        weaponSkill = (int)_wsInput.Value,
        ballisticSkill = (int)_bsInput.Value,
        // ... other base values
        
        // NEW: Advances
        weaponSkillAdv = (int)_wsInputAdv.Value,
        ballisticSkillAdv = (int)_bsInputAdv.Value,
        // ... other advances
        
        // NEW: Modifiers  
        weaponSkillMod = (int)_wsInputMod.Value,
        ballisticSkillMod = (int)_bsInputMod.Value,
        // ... other modifiers
    };
    
    // Send to API...
}
```

## 🎮 Learning Path

### **Phase 1: Basic Implementation (Start Here!)**
1. **Add the event connections** for 2-3 characteristics (WS, BS, S)
2. **Create simple update method** for those characteristics
3. **Test** that labels update when SpinBox values change

### **Phase 2: Complete Client Logic**
1. **Extend to all 10 characteristics**
2. **Add validation** (negative values, max limits)
3. **Add visual feedback** (color coding for high/low values)

### **Phase 3: Database Integration** 
1. **Update Character.cs model** with Adv/Mod properties
2. **Update API endpoints** to handle new fields
3. **Update save/load methods** in CharacterSheetUI.cs

### **Phase 4: Advanced Features**
1. **Species bonuses** (Dwarfs get +20 Toughness, etc.)
2. **Career progression** (automatic advances)
3. **Temporary modifiers** (spells, injuries)

## 🛠️ Code Templates

### Template: Characteristic Update Method
```csharp
private Dictionary<string, (SpinBox Base, SpinBox Adv, SpinBox Mod, Label Current)> _characteristics;

private void InitializeCharacteristics()
{
    _characteristics = new()
    {
        ["WS"] = (_wsInput, _wsInputAdv, _wsInputMod, _wsInputCur),
        ["BS"] = (_bsInput, _bsInputAdv, _bsInputMod, _bsInputCur),
        ["S"] = (_sInput, _sInputAdv, _sInputMod, _sInputCur),
        ["T"] = (_tInput, _tInputAdv, _tInputMod, _tInputCur),
        ["I"] = (_iInput, _iInputAdv, _iInputMod, _iInputCur),
        ["AG"] = (_agInput, _agInputAdv, _agInputMod, _agInputCur),
        ["DEX"] = (_dexInput, _dexInputAdv, _dexInputMod, _dexInputCur),
        ["INT"] = (_intInput, _intInputAdv, _intInputMod, _intInputCur),
        ["WP"] = (_wpInput, _wpInputAdv, _wpInputMod, _wpInputCur),
        ["FEL"] = (_felInput, _felInputAdv, _felInputMod, _felInputCur)
    };
}

private void SetupCharacteristicBindings()
{
    foreach (var (key, (baseInput, advInput, modInput, currentLabel)) in _characteristics)
    {
        baseInput.ValueChanged += (double _) => UpdateCharacteristic(key);
        advInput.ValueChanged += (double _) => UpdateCharacteristic(key);
        modInput.ValueChanged += (double _) => UpdateCharacteristic(key);
    }
}

private void UpdateCharacteristic(string characteristic)
{
    if (_characteristics.TryGetValue(characteristic, out var controls))
    {
        var total = (int)(controls.Base.Value + controls.Adv.Value + controls.Mod.Value);
        controls.Current.Text = total.ToString();
        
        // Optional: Color coding
        if (total >= 70) controls.Current.Modulate = Colors.Green;
        else if (total <= 20) controls.Current.Modulate = Colors.Red;
        else controls.Current.Modulate = Colors.White;
    }
}
```

## 🚦 Best Practices

### **Input Validation**
```csharp
private void ValidateCharacteristicInput(SpinBox input, int min = 0, int max = 100)
{
    input.ValueChanged += (double value) => 
    {
        if (value < min) input.Value = min;
        if (value > max) input.Value = max;
    };
}
```

### **Performance Tips**
- ✅ **Use local variables** for calculations
- ✅ **Batch updates** when loading character data
- ✅ **Only update visible labels** 
- ❌ **Don't call API** on every value change


## 🎯 Quick Start Checklist

- [ ] **Phase 1**: Connect events for WS, BS, S characteristics
- [ ] **Test**: Change SpinBox values, verify labels update
- [ ] **Phase 2**: Extend to all 10 characteristics  
- [ ] **Add**: Input validation and visual feedback
- [ ] **Phase 3**: Update database schema and API
- [ ] **Test**: Save/load with advances and modifiers
- [ ] **Phase 4**: Add WFRP-specific features

## 💡 Pro Tips

1. **Start Small**: Implement 2-3 characteristics first, then scale up
2. **Test Early**: Make sure basic math works before adding complexity
3. **Use Dictionaries**: Avoid repetitive code with smart data structures
4. **Visual Feedback**: Color-code values to show good/bad characteristics
5. **WFRP Rules**: Remember characteristic bonuses are value/10 (rounded down)

Good luck with your implementation! Start with Phase 1 and build up gradually. The foundation you have is solid - now it's just connecting the pieces! 🎲

---
*Remember: This is a learning exercise, so take time to understand each piece before moving to the next!*