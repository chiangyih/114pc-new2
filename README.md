# 113 學年度工科賽 電腦修護職類 第二站 PC 端程式
<img width="815" height="559" alt="image" src="https://github.com/user-attachments/assets/1f19055f-3daa-43f9-a769-72b4b7160c3c" />

## 專案簡介
本專案是專為 113 學年度全國各級學校工業類科學生技藝競賽電腦修護職類第二站需求所開發的 PC 端控制程式。

## 崗位號碼
**01** (奇數崗位)

## 版本資訊
- **目前版本**: v2.2
- **最後更新**: 2025-11-20
- **開發語言**: Visual Basic .NET 2022
- **目標框架**: .NET 8.0 Windows Forms

---

## 功能特色

###  已實現功能

#### P1 - 程式視窗標題列
- 顯示完整賽事資訊與崗位號碼

#### P2 - 藍牙模組名稱處理
- 自動計算藍牙模組名稱：`ODD-01-0001`
- 根據崗位號碼判斷奇偶性

#### P3 - COM Port 掃描偵測
- 即時動態顯示所有可用 COM Port
- 支援熱插拔自動更新（每 2 秒檢查一次）

#### P4 - COM Port 自動連線
- 開啟序列埠自動顯示「Connected」狀態
- 自動發送連線字元 'c' 給 Arduino

#### P5 - COM Port 中斷處理
- 關閉顯示「Disconnect」狀態
- 自動停用相關功能按鈕
- 發送斷線字元 'b' 給 Arduino

#### P6 - 連線狀態指示
- 即時顯示連線狀態（Connected/Disconnect）
- 使用顏色區分（綠色/紅色）

#### P7 - CPU Loading 監控
- 每秒更新一次 CPU 使用率
- 自動判斷顏色：
  - 0~50%：綠色
  - 51~84%：黃色
  - ?85%：紅色
- **傳輸格式：`LOAD數值\n`**（簡單文字格式）
- 同步更新 WS2812 LED 顯示

#### RAM Loading 監控（擴充功能）
- 每秒更新一次 RAM 使用率
- 自動判斷顏色（與 CPU 相同規則）
- 同步更新 WS2812 LED 顯示
- **傳輸格式：`LOAD數值\n`**（簡單文字格式）

#### P8 - 二進位輸入驗證
- 驗證 4 位元二進位輸入格式
- 錯誤時顯示「Not BIN Format」訊息
- 自動轉換為十進位

#### P9 - EEPROM 寫入
- Write 按鈕連動傳送數據至 MCU
- 使用二進位封包格式（保留原格式）

#### P10 - 程式退出
- 安全關閉所有連線與資源
- 清理計時器與序列埠

#### P11 - 全域訊息同步
- 即時顯示所有監控數據
- CPU/RAM Loading 每秒更新
- 連線狀態即時同步

#### P12 - WS2812 燈號同步顯示 ? 優化
- 繪製 8 顆圓形 LED 燈號（直徑 22 像素）
- 支援即時同步 Arduino WS2812 狀態
- 使用梯度漸層效果呈現真實感
- 支援 RGB 24-bit 顏色空間
- **CPU/RAM 監控時同步顯示顏色**
- **停止監控時自動重置為黑色**
- 斷線時燈號自動熄滅

---

## 系統需求

### 開發環境
- Visual Studio 2022
- .NET 8.0 Windows Forms
- VB.NET

### 套件相依
- System.IO.Ports (v8.0.0)
- System.Management (v8.0.0)

### 硬體需求
- 支援藍牙連線電腦（或 USB 轉序列埠）
- HC-05 藍牙模組或相容裝置
- Arduino 開發板（含連接 WS2812 LED）

---

## 程式架構

### 檔案結構

```
114pc-new2/
│
├── Form1.vb                        # 主視窗程式
├── Form1.Designer.vb               # UI 設計
│
├── Modules/
│   ├── SerialPortManager.vb        # 序列埠管理
│   ├── ComPortWatcher.vb           # COM Port 監控
│   ├── CpuLoadProvider.vb          # CPU/RAM 使用率監控
│   ├── EepromService.vb            # EEPROM 服務
│   ├── BleProtocol.vb              # 藍牙通訊協定
│   ├── LedDisplayManager.vb        # LED 顯示管理
│   ├── LedSyncService.vb           # LED 同步服務
│   └── TitleService.vb             # 標題服務
│
├── 114pc-new2.vbproj               # 專案檔
│
├── README.md                       # 本檔案
├── 使用手冊.md                     # 詳細操作說明
├── 專案完成報告.md                  # 開發紀錄
└── v2.2_優化更新總結.md             # 最新更新說明
```

---

## 通訊協定

### PC 端 → Arduino 端

#### ?? CPU/RAM Loading 監控（v2.2 簡化格式）
```
格式：LOAD數值\n
範例：LOAD45.2\n（CPU 使用率 45.2%）
      LOAD62.8\n（RAM 使用率 62.8%）
編碼：ASCII
長度：變動（數值位數不同）
```

**優點：**
- ? 格式簡單易讀
- ? 可直接在序列埠監視器觀察
- ? Arduino 解析容易
- ? 適合即時監控應用

**Arduino 接收範例：**
```cpp
if (Serial.available()) {
    String data = Serial.readStringUntil('\n');
    if (data.startsWith("LOAD")) {
        float value = data.substring(4).toFloat();
        // 處理接收到的數值
    }
}
```

#### EEPROM 寫入封包（二進位格式）
```
[SOF] [CMD] [LEN] [Data] [CHK] [EOF]
0xAA  0x20  0x01  value  sum   0x55
```

**格式說明：**
- SOF: 起始字元 (0xAA)
- CMD: 命令碼 (0x20 = EEPROM)
- LEN: 資料長度 (0x01)
- Data: 資料值 (0-15)
- CHK: 檢查碼 (sum & 0xFF)
- EOF: 結束字元 (0x55)

#### 控制字元
| 字元 | 功能 | Arduino 動作 |
|------|------|-------------|
| `c` (0x63) | 連線確認 | 回應 ACK |
| `b` (0x62) | 斷線通知 | 熄滅所有 LED |

---

## 顏色對照表

### CPU/RAM Loading 顏色規則

| 使用率範圍 | 顏色 | RGB 值 | WS2812 LED 顯示 |
|-----------|------|--------|----------------|
| 0% ~ 50% | ?? 綠色 | (0, 255, 0) | 8 顆 LED 全綠 |
| 51% ~ 84% | ?? 黃色 | (255, 255, 0) | 8 顆 LED 全黃 |
| 85% ~ 100% | ?? 紅色 | (255, 0, 0) | 8 顆 LED 全紅 |
| 停止/斷線 | ? 黑色 | (0, 0, 0) | 8 顆 LED 熄滅 |

---

## 使用說明

### 1. 連線設定
1. 啟動程式後，COM Port 清單會自動載入
2. 選擇正確的 COM Port（藍牙或 USB）
3. 點擊「Open」開啟連線
4. 連線成功後狀態顯示「Connected」

### 2. CPU 監控
1. 連線成功後，「Start」按鈕可用
2. 點擊「Start」開始監控 CPU 使用率
3. 每秒自動更新數值與顏色
4. LED 燈號同步顯示當前顏色
5. **自動發送 `LOAD數值\n` 格式至 Arduino**
6. 點擊「Stop」停止監控，LED 自動重置為黑色

### 3. RAM 監控
1. 點擊「Start」開始監控 RAM 使用率
2. 每秒自動更新數值
3. LED 燈號同步顯示當前顏色
4. **自動發送 `LOAD數值\n` 格式至 Arduino**
5. 點擊「Stop」停止監控，LED 自動重置為黑色

### 4. EEPROM 寫入
1. 在輸入欄輸入 4 位元二進位數值（例如：1010）
2. 點擊「Write」傳送
3. 程式會自動驗證格式並轉換為十進位
4. 封包傳送至 Arduino EEPROM

### 5. LED 燈號觀察
- 8 顆圓形 LED 自動同步顯示顏色
- CPU 或 RAM Loading 監控時，所有燈號同步變色
- 停止監控時燈號自動熄滅
- 斷線時燈號自動熄滅

---

## 序列埠設定

- **Baud Rate**: 9600 bps
- **Data Bits**: 8
- **Parity**: None
- **Stop Bits**: 1

---

## 注意事項

1. **序列埠獨占**：確保程式關閉前沒有其他程式佔用 COM Port
2. **藍牙配對**：使用前請先配對藍牙模組（預設 PIN：1234 或 0000）
3. **資源釋放**：關閉程式前請先關閉 COM Port
4. **CPU 監控精度**：首次讀取 CPU 使用率可能不準確
5. **LED 更新頻率**：每秒更新一次（同 CPU 監控同步）
6. **? 通訊格式**：CPU/RAM 使用 ASCII 文字格式，EEPROM 使用二進位格式
7. **? LED 重置**：停止監控或斷線時，LED 會自動重置為黑色

---

## 錯誤處理

- **COM Port 開啟失敗**：檢查是否被其他程式佔用
- **Not BIN Format**：輸入必須是 4 位元二進位（0 和 1）
- **連線中斷**：程式會自動停用相關功能並更新狀態
- **? Arduino 無回應**：確認已上傳正確的程式碼，支援 `LOAD數值\n` 格式

---

## 測試驗證

### 單元測試項目
1. ? COM Port 偵測與開啟
2. ? CPU Loading 讀取與顏色判斷
3. ? RAM Loading 讀取與顏色判斷
4. ? 二進位輸入驗證
5. ? 封包格式（ASCII + 二進位）
6. ? LED 顯示同步
7. ? LED 停止重置
8. ? 連線狀態管理

### 整合測試項目
1. ? PC ? Arduino 雙向通訊
2. ? WS2812 燈號同步（ASCII 格式）
3. ? EEPROM 寫入驗證（二進位格式）
4. ? 異常處理

---

## 版本歷程

### v2.2（最新）- 2025-01-XX
- ?? **簡化傳輸格式**：CPU/RAM 改用 `LOAD數值\n` 格式
- ??? **移除 Color 區塊**：簡化 CPU Loading Monitor UI
- ? **增強 RAM 功能**：新增顏色判斷和 LED 同步
- ?? **LED 自動重置**：停止監控時自動熄滅 LED
- ?? **UI 優化**：視窗高度縮小為 525px

### v2.1 - 2025-01-XX
- 支援 `#COLOR:RRGGBB` ASCII 字串格式
- 符合競賽規格要求

### v2.0
- 完成所有基本功能（P1-P12）
- 實現 WS2812 LED 同步顯示

---

## 相關文件

- ? `README.md` - 本檔案（專案總覽）
- ? `使用手冊.md` - 詳細操作說明
- ? `專案完成報告.md` - 開發紀錄
- ? `v2.2_優化更新總結.md` - **最新更新說明**
- ? `pc_spec_v3.md` - 功能需求規格

---

## 授權聲明

本程式僅供競賽及教育使用。

---

## 快速開始

### 最簡使用流程
```
1. 啟動程式
   ↓
2. 選擇 COM Port → 點擊 Open
   ↓
3. 等待「Connected」狀態
   ↓
4. 點擊 CPU 或 RAM 的 Start 按鈕
   ↓
5. 觀察數值、LED 顏色變化
   ↓
6. 完成後點擊 Stop → LED 自動熄滅
   ↓
7. 點擊 Close → 點擊 Exit
```

---

**崗位號碼：01**  
**藍牙模組：ODD-01-0001**  
**奇數崗位**  
**通訊格式：ASCII (CPU/RAM) + Binary (EEPROM)**  
**版本：v2.2**

---

*最後更新：2025-01-XX*  
*開發環境：Visual Studio 2022 + .NET 8.0*  
*目標競賽：113 學年度工科賽電腦修護職類第二站*
