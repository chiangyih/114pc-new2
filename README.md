# 113 學年度工科賽 電腦修護職類 第二站 PC 端程式

## 專案說明
本專案是根據 113 學年度全國高級中等學校工業類科學生技藝競賽電腦修護職類第二站的需求所開發的 PC 端控制程式。

## 崗位號碼
**01** (新化高工)

## 功能特色

### ? 已實現功能

#### P1 - 程式標題列顯示
- 視窗標題顯示完整賽事資訊與崗位號碼

#### P2 - 藍牙模組名稱處理
- 自動計算並顯示藍牙模組名稱：`ODD-01-0001`
- 依據崗位號碼二進位最右位判斷奇偶

#### P3 - COM Port 清單偵測
- 即時偵測並顯示所有可用 COM Port
- 支援熱插拔自動更新（每 2 秒檢查一次）

#### P4 - COM Port 自動連線
- 開啟序列埠時自動顯示「Connected」狀態
- 自動發送連線字元 'c' 給 Arduino

#### P5 - COM Port 關閉處理
- 關閉時顯示「Disconnect」狀態
- 自動停用相關功能按鈕
- 發送斷線字元 'b' 給 Arduino

#### P6 - 連線狀態顯示
- 即時顯示連線狀態（Connected/Disconnect）
- 使用顏色區分（綠色/紅色）

#### P7 - CPU Loading 監控
- 每秒更新一次 CPU 使用率
- 自動判斷顏色：
  - 0~50%：綠色
  - 51~84%：黃色
  - ?85%：紅色
- 透過藍牙傳送至 MCU 控制 WS2812（**使用 ASCII 格式**）

#### P8 - 二進位輸入驗證
- 驗證 4 位元二進位輸入格式
- 錯誤時顯示「Not BIN Format」訊息
- 自動轉換為十進位

#### P9 - EEPROM 寫入
- Write 按鈕封裝並傳送資料至 MCU
- 使用二進位封包格式

#### P10 - 程式退出
- 安全關閉所有連線與資源
- 清理計時器與序列埠

#### P11 - 畫面資訊同步
- 即時顯示所有監控數值
- CPU/RAM Loading 每秒更新
- 顏色指示器同步更新

#### P12 - WS2812 燈號同步顯示 ? NEW
- 顯示 8 顆圓形 LED 燈號（直徑 22 像素）
- 支援即時同步 Arduino WS2812 狀態
- 使用反鋸齒技術繪製平滑圓形
- 支援 RGB 24-bit 顏色深度
- 連線中斷時燈號自動熄滅

## 系統需求

### 開發環境
- Visual Studio 2022
- .NET 8.0 Windows Forms
- VB.NET

### 必要套件
- System.IO.Ports (v8.0.0)
- System.Management (v8.0.0)

### 硬體需求
- 支援藍牙的電腦（或 USB 轉序列埠）
- HC-05 藍牙模組或相容設備
- Arduino 開發板（配備 WS2812 LED）

## 程式架構

### 模組化設計

```
114pc-new2/
├── Form1.vb                        # 主視窗程式
├── Form1.Designer.vb               # UI 設計
├── Modules/
│   ├── SerialPortManager.vb        # 序列埠管理
│   ├── ComPortWatcher.vb           # COM Port 監控
│   ├── CpuLoadProvider.vb          # CPU/RAM 使用率監控
│   ├── EepromService.vb            # EEPROM 服務
│   ├── BleProtocol.vb              # 藍牙通訊協定
│   ├── LedDisplayManager.vb        # LED 顯示管理
│   ├── LedSyncService.vb           # LED 同步服務
│   └── TitleService.vb             # 標題服務
└── 114pc-new2.vbproj               # 專案檔
```

## 通訊協定

### PC → Arduino 封包格式

#### ?? CPU Loading 顏色控制（ASCII 字串格式）
```
格式：#COLOR:RRGGBB
範例：#COLOR:FF0000 (紅色)
```

| CPU 使用率 | 顏色 | 命令字串 | 說明 |
|-----------|------|---------|------|
| 0% ~ 50% | 綠色 | `#COLOR:00FF00` | R=0, G=255, B=0 |
| 51% ~ 84% | 黃色 | `#COLOR:FFFF00` | R=255, G=255, B=0 |
| 85% ~ 100% | 紅色 | `#COLOR:FF0000` | R=255, G=0, B=0 |
| 斷線 | 關閉 | `#COLOR:000000` | R=0, G=0, B=0 |

**格式說明：**
- 固定 14 個 ASCII 字元
- `#COLOR:` 為固定前綴（7 個字元）
- `RRGGBB` 為 6 位 16 進位顏色碼
- 無結束符號，接收滿 14 個字元即解析

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
- CHK: 校驗碼 (sum & 0xFF)
- EOF: 結束字元 (0x55)

#### 控制字元
| 字元 | 功能 | Arduino 動作 |
|------|------|-------------|
| `c` (0x63) | 連線確認 | 回傳 ACK |
| `b` (0x62) | 斷線通知 | 關閉所有 LED |

### Arduino → PC 封包格式

#### LED 狀態更新封包（二進位格式，可選）
```
[SOF] [CMD] [LEN] [LED0_RGB] ... [LED7_RGB] [CHK] [EOF]
0xAA  0x10  0x18  R G B         R G B        sum   0x55
```

**說明：** Arduino 可選擇性回傳 LED 狀態，PC 端 UI 主要根據自身計算的顏色同步顯示。

## 使用說明

### 1. 連線設定
1. 啟動程式後，COM Port 列表會自動載入
2. 選擇對應的 COM Port（藍牙或 USB）
3. 點擊「Open」開啟連線
4. 連線成功後狀態顯示「Connected」

### 2. CPU 監控
1. 連線成功後，「Start」按鈕啟用
2. 點擊「Start」開始監控 CPU 使用率
3. 每秒自動更新數值與顏色
4. LED 燈號同步顯示對應顏色
5. **自動發送 `#COLOR:RRGGBB` 格式命令至 Arduino**

### 3. RAM 監控
1. 點擊「Start」開始監控 RAM 使用率
2. 每秒自動更新數值

### 4. EEPROM 寫入
1. 在輸入框中輸入 4 位元二進位數值（例如：1010）
2. 點擊「Write」按鈕
3. 程式會自動驗證格式並轉換為十進位
4. 封包傳送至 Arduino EEPROM

### 5. LED 燈號顯示
- 8 顆圓形 LED 自動同步顯示顏色
- CPU Loading 改變時，所有燈號同步變色
- 斷線時燈號自動熄滅
- 使用 ASCII 命令控制 Arduino WS2812

## 序列埠設定

- **Baud Rate**: 9600 bps
- **Data Bits**: 8
- **Parity**: None
- **Stop Bits**: 1

## 顏色對照表

| CPU 使用率 | 顏色 | RGB 值 | ASCII 命令 |
|-----------|------|--------|-----------|
| 0% ~ 50% | 綠色 | (0, 255, 0) | `#COLOR:00FF00` |
| 51% ~ 84% | 黃色 | (255, 255, 0) | `#COLOR:FFFF00` |
| 85% ~ 100% | 紅色 | (255, 0, 0) | `#COLOR:FF0000` |

## 注意事項

1. **序列埠權限**：確保程式有足夠權限存取 COM Port
2. **藍牙配對**：使用前須先配對藍牙模組（預設 PIN：1234 或 0000）
3. **資源釋放**：關閉程式前建議先關閉 COM Port
4. **CPU 監控延遲**：首次讀取 CPU 使用率可能不準確
5. **LED 更新頻率**：每秒更新一次（與 CPU 監控同步）
6. **?? 通訊格式**：WS2812 控制使用 ASCII 字串格式，EEPROM 使用二進位格式

## 錯誤處理

- **COM Port 開啟失敗**：檢查埠是否被其他程式占用
- **Not BIN Format**：輸入必須為 4 位元二進位（0 或 1）
- **連線中斷**：程式會自動偵測並更新狀態
- **?? Arduino 無反應**：確認已上傳新版支援 `#COLOR:RRGGBB` 的程式

## 測試建議

### 單元測試項目
1. ? COM Port 偵測與開啟
2. ? CPU Loading 讀取與顏色判斷
3. ? 二進位輸入驗證
4. ? 封包格式（ASCII + 二進位）
5. ? LED 顯示同步
6. ? 連線狀態切換

### 整合測試項目
1. ? PC ? Arduino 雙向通訊
2. ? WS2812 燈號同步（ASCII 格式）
3. ? EEPROM 寫入驗證（二進位格式）
4. ? 熱插拔處理

## 版本資訊

- **版本**: v2.1
- **日期**: 2025-01-XX
- **開發者**: Visual Basic .NET 2022
- **對應賽事**: 113 學年度工科賽 電腦修護職類
- **?? 更新**: 支援 `#COLOR:RRGGBB` ASCII 字串格式

## 相關文件

- ?? `通訊格式更新說明.md` - 詳細的格式變更說明
- ?? `使用手冊.md` - 完整的使用指南
- ?? `專案完成報告.md` - 開發成果報告
- ?? `pc_spec_v3.md` - 功能需求規格書
- ?? `Arduino_Reference\arduino_main.ino` - Arduino 端程式

## 授權聲明

本程式僅供教育與競賽使用。

---

**崗位號碼：01**  
**藍牙模組：ODD-01-0001**  
**新化高工**  
**通訊格式：ASCII (#COLOR:RRGGBB) + Binary (EEPROM)**
