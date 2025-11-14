# 💻 PC 端功能需求總表（崗位編號：01）
## 113 學年度 全國高級中等學校 工業類科學生技藝競賽
### 電腦修護職類 第二站 — 個人電腦 USB / 藍牙介面卡製作及控制
### PC 端開發語言：Visual Basic .NET 2022

---

## 🆕 修訂記錄
| 版本 | 日期 | 修訂內容 |
|------|------|----------|
| v2.0 | 2025-11-13 | 新增 P12 功能：WS2812 燈號同步顯示模組 |
| v1.0 | - | 初始版本 |

---

## 1️⃣ 功能需求總覽（依據題目原文 B-5～B-8 條）

| 編號 | 功能名稱 | 功能描述（依競賽題目） | 是否需與 MCU 通訊 | 備註 / 技術要點 |
|:--:|:--|:--|:--:|:--|
| **P1** | **程式標題列顯示** | 視窗標題列須顯示：<br>`113 學年度 工業類科學生技藝競賽 電腦修護職種 新化高工 第二站 崗位號碼：01` | ❌ 否 | 在 Form_Load 時設定固定文字 |
| **P2** | **藍牙模組名稱處理邏輯** | 將崗位號碼 `01` 轉為二進位（00000001），取最右位判斷奇偶：<br>奇數 → 顯示 `ODD-01-0001` | ✅ 是 | 命名邏輯：ODD-01-BBBB |
| **P3** | **COM Port 清單偵測與即時更新** | 顯示系統內所有可用 COM Port（USB / BLE 虛擬埠），可即時更新 | ❌ 否 | 使用 `SerialPort.GetPortNames()` 或 `ManagementEventWatcher` |
| **P4** | **COM Port Open 自動連線** | 開啟序列埠時自動顯示「Connected」，不需額外操作 ，並發送一個c字元給arduino| ✅ 是 | 成功開啟 SerialPort 後自動更新狀態 |
| **P5** | **COM Port Close 處理** | 關閉埠時顯示「Disconnect」，並停用：`Write`、`Now Time`、`CPU Loading`，並發送一個b字元給arduino | ✅ 是 | Close 事件需同步更新 UI 狀態 |
| **P6** | **連線狀態顯示** | 顯示目前藍牙連線狀態：Connected / Disconnect | ✅ 是 | 以 Label 或狀態列顯示 |
| **P7** | **CPU Loading 取得與顏色同步** | 取得 PC CPU 使用率 → 顯示數值與顏色：<br>0~50% 綠、51~84% 黃、≥85% 紅<br>並以藍芽封包傳送至 MCU 控制 WS2812 | ✅ 是 | 使用 `PerformanceCounter("Processor", "% Processor Time", "_Total")` |
| **補充**| **RAM Loading 取得**| 取得 PC RAM 使用率|
| **P8** | **資料傳輸：二進位輸入與 EEPROM 寫入** | 輸入四位二進位數值（0/1），若正確 → 轉十進位 → 傳送至 MCU 寫入 EEPROM；<br>若格式錯誤 → 清空欄位並彈出 `Not BIN Format` | ✅ 是 | 寫入前需正規表示式驗證 `^[01]{4}$` |
| **P9** | **Write 按鈕行為** | 按下後立即封裝封包並傳送至 MCU | ✅ 是 | 與 P8 結合實現 |
| **P10** | **Exit 按鈕行為** | 關閉程式、釋放序列埠資源 | ❌ 否 | 使用 `Application.Exit()` |
| **P11** | **畫面資訊同步顯示** | 即時顯示 CPU Loading 數值、顏色、COM Port 狀態、RAM Loading 數值等 | ❌ 否 | 更新頻率建議每秒 1 次 |
| **🆕 P12** | **🔴 WS2812 燈號同步顯示** | UI 上顯示 8 個圓形燈號（直徑 5mm），即時同步 Arduino 端 WS2812 的顏色與效果 | ✅ 是 | 詳見下方「WS2812 同步顯示模組規格」 |

---

## 🆕 2️⃣ WS2812 燈號同步顯示模組規格

### 📐 UI 設計規格
| 項目 | 規格 |
|------|------|
| **燈號數量** | 8 顆 |
| **燈號形狀** | 圓形（Circle） |
| **燈號直徑** | 約 22 像素 @ 96 DPI） |
| **燈號間距** | 建議 2-3mm（約 8-12 像素） |
| **排列方式** | 建議水平或垂直一字排開 |
| **預設顏色** | 黑色（關閉狀態）或深灰色 |
| **邊框** | 可選：淺灰色邊框區分燈號 |

### 🎨 顏色同步規則
| 情境 | WS2812 硬體顯示 | VB UI 同步顯示 |
|------|-----------------|----------------|
| **CPU Loading 0~50%** | 綠色 (0, 255, 0) | 8 顆燈全部顯示綠色 |
| **CPU Loading 51~84%** | 黃色 (255, 255, 0) | 8 顆燈全部顯示黃色 |
| **CPU Loading ≥85%** | 紅色 (255, 0, 0) | 8 顆燈全部顯示紅色 |
| **連線中斷** | 燈號關閉 | 8 顆燈全部顯示黑色/深灰 |
| **自訂效果** | 依 MCU 回傳資料 | 依序列埠接收到的 RGB 值更新 |




### 📡 資料同步機制

#### Arduino → PC 資料格式（建議）
```
[SOF] [CMD] [LEN] [LED0_R] [LED0_G] [LED0_B] ... [LED7_R] [LED7_G] [LED7_B] [CHK] [EOF]
```

| 欄位 | 說明 | 範例 |
|------|------|------|
| SOF | 起始字元 | 0xAA |
| CMD | 命令碼（LED 更新） | 0x10 |
| LEN | 資料長度 | 24 (8顆 × 3 色彩) |
| LED_RGB | 每顆燈的 RGB 值 | R,G,B (0-255) |
| CHK | 校驗碼 | (SUM) & 0xFF |
| EOF | 結束字元 | 0x55 |

#### PC 端接收與更新流程
```vb
' SerialPort DataReceived 事件
Private Sub SerialPort1_DataReceived(sender As Object, e As SerialDataReceivedEventArgs)
    Try
        Dim buffer As Byte() = New Byte(SerialPort1.BytesToRead - 1) {}
        SerialPort1.Read(buffer, 0, buffer.Length)
        
        ' 解析封包
        If buffer(0) = &HAA AndAlso buffer(1) = &H10 Then
            ' 更新 8 顆燈號
            For i As Integer = 0 To 7
                Dim r As Byte = buffer(3 + i * 3)
                Dim g As Byte = buffer(4 + i * 3)
                Dim b As Byte = buffer(5 + i * 3)
                Dim color As Color = Color.FromArgb(r, g, b)
                
                ' 在 UI 執行緒更新
                Me.Invoke(Sub() DrawCircleLed(ledPictures(i), color))
            Next
        End If
    Catch ex As Exception
        ' 錯誤處理
    End Try
End Sub
```

---

## 3️⃣ 操作行為邏輯（更新版）

| 行為情境 | 系統應執行動作 |
|-----------|----------------|
| 程式啟動 | 顯示標題列與崗位號碼 `01`，列出所有 COM Port，狀態為 Disconnect，**8 顆燈號顯示黑色/關閉** |
| 選擇 Port 並開啟 | 自動顯示 Connected，啟用 Write / Start 按鈕，**燈號準備接收資料** |
| 關閉 Port | 顯示 Disconnect，停用所有操作按鈕，**8 顆燈號恢復黑色** |
| 按下 StartCPU | 啟動 CPU Loading 監測計時器（每秒更新一次），**同步更新燈號顏色** |
| 按下 StartRAM | 啟動 RAM Loading 監測計時器（每秒更新一次） |
| 傳送 Loading 至 MCU | 封裝格式 `[SOF][CMD][LEN][Data][CHK][EOF]`，內容包含 Loading% 與 RGB 顏色，**VB 同步更新燈號** |
| **🆕 接收 WS2812 狀態** | **解析 Arduino 回傳的 LED RGB 資料，同步更新 UI 上 8 顆燈號顏色** |
| 寫入 EEPROM | 驗證輸入後轉十進位，傳送封包給 MCU 寫入 EEPROM |
| 結束程式 | 關閉所有連線與埠，釋放資源，**清除燈號顯示** |

---

## 4️⃣ 模組分層架構建議（更新版）

| 模組名稱 | 功能 |
|-----------|------|
| `SerialPortManager` | 控制藍牙 / USB 序列埠開關與傳輸 |
| `ComPortWatcher` | 偵測 COM Port 熱插拔與更新清單 |
| `CpuLoadProvider` | 取得 CPU Loading 值並傳送至 MCU |
| `EepromService` | 驗證輸入、轉換十進位與資料封包 |
| `UiController` | 控制畫面更新、狀態切換、按鈕管理 |
| `TitleService` | 設定標題列文字與崗位號碼 |
| `BleProtocol` | 負責封包打包與 Checksum 驗證 |
| **🆕 `LedDisplayManager`** | **管理 8 顆圓形燈號的繪製與顏色更新** |
| **🆕 `LedSyncService`** | **解析 Arduino WS2812 資料並同步至 UI** |

---

## 5️⃣ 額外系統需求（更新版）

| 項目 | 說明 |
|------|------|
| 更新頻率 | CPU Loading 每 1 秒更新一次 |
| 更新頻率 | RAM Loading 每 1 秒更新一次 |
| **🆕 LED 更新頻率** | **即時更新（接收到資料立即刷新），最高 30 FPS** |
| 封包完整性 | 建議使用簡易校驗 `(SUM of data) & 0xFF` |
| 錯誤處理 | 連線中斷 → 自動顯示 Disconnect，**燈號全滅**；輸入錯誤 → 彈出警告 |
| 顏色對應表 | 0~50% 綠 / 51~84% 黃 / ≥85% 紅 |
| 預設 Baud Rate | 9600 bps（HC-05 標準速率） |
| **🆕 燈號顏色深度** | **RGB 24-bit (每色 8-bit, 0-255)** |
| **🆕 燈號繪製品質** | **使用反鋸齒（AntiAlias）確保圓形平滑** |

---

## 6️⃣ 整體通訊對應關係（更新版）

| PC 端動作 | 傳送資料 | MCU 響應 | 備註 |
|------------|-----------|-----------|------|
| 開啟連線 | `CMD_OPEN` | 回傳 `ACK` + **初始 LED 狀態** | BLE 連線成功 |
| 傳送 CPU Loading | `CMD_LOAD <VAL>` | 顯示顏色於 WS2812 + **回傳 LED RGB 陣列** | 實時更新 |
| **🆕 接收 LED 狀態** | **無（被動接收）** | **定期或狀態改變時主動推送 8 顆 LED 的 RGB 值** | **PC 同步顯示** |
| 寫入 EEPROM | `CMD_EEPROM <DEC>` | EEPROM 寫入成功 | 需回 ACK |
| 關閉連線 | `CMD_CLOSE` | 無 | 結束通訊 |
| 程式結束 | 自動關閉 SerialPort | 無 | 釋放資源 |

---


**版本：** v2.0  
**崗位號碼：** 01  
**修訂日期：** 2025-11-13  
**撰寫者：** Visual Basic .NET 2022 專案規格書  
**對應題目：** 113 年全國高級中等學校工業類科技藝競賽 — 電腦修護職類 第二站  
**🆕 新增功能：** WS2812 燈號同步顯示模組
