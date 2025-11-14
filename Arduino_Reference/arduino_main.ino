/*
 * Arduino 端參考程式（更新版）
 * 113 學年度工科賽 電腦修護職類 第二站
 * 崗位號碼：01
 * 
 * 功能：
 * - 接收 PC 端 CPU Loading 資料（ASCII 字串格式）
 * - 控制 WS2812 LED 顯示對應顏色
 * - EEPROM 資料寫入（二進位格式）
 * - 回傳 LED 狀態給 PC
 * 
 * 更新：支援 #COLOR:RRGGBB ASCII 字串格式
 */

#include <Adafruit_NeoPixel.h>
#include <EEPROM.h>

// ========================================
// WS2812 設定
// ========================================
#define LED_PIN    6
#define LED_COUNT  8
Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

// ========================================
// 通訊協定常數（二進位格式）
// ========================================
const byte SOF = 0xAA;
const byte EOF_BYTE = 0x55;
const byte CMD_CPU_LOAD = 0x10;
const byte CMD_EEPROM = 0x20;

// ========================================
// 接收緩衝區
// ========================================
const int BUFFER_SIZE = 64;
char rxBuffer[BUFFER_SIZE];
int rxIndex = 0;

byte binaryBuffer[32];
int binaryIndex = 0;

// ========================================
// LED 狀態
// ========================================
struct LedState {
  byte r, g, b;
};
LedState ledStates[LED_COUNT];

// ========================================
// 設定函式
// ========================================
void setup() {
  Serial.begin(9600);
  
  // 初始化 WS2812
  strip.begin();
  strip.show(); // 初始化所有燈號為關閉
  
  // 初始化 LED 狀態陣列
  for (int i = 0; i < LED_COUNT; i++) {
    ledStates[i] = {0, 0, 0};
  }
}

// ========================================
// 主迴圈
// ========================================
void loop() {
  // 檢查序列埠資料
  while (Serial.available() > 0) {
    char inChar = Serial.read();
    
    // ----------------------------------------
    // 處理特殊控制字元
    // ----------------------------------------
    if (inChar == 'c') {
      // 連線字元，回應 ACK
      sendAck();
      continue;
    } else if (inChar == 'b') {
      // 斷線字元，關閉所有 LED
      turnOffAllLeds();
      continue;
    }
    
    // ----------------------------------------
    // 檢測 ASCII 命令（#COLOR:RRGGBB）
    // ----------------------------------------
    if (inChar == '#') {
      // 開始接收新的 ASCII 命令
      rxIndex = 0;
      rxBuffer[rxIndex++] = inChar;
    } else if (rxIndex > 0 && rxIndex < BUFFER_SIZE - 1) {
      // 繼續接收字元
      rxBuffer[rxIndex++] = inChar;
      
      // 檢查是否接收到完整的顏色命令（14 個字元：#COLOR:RRGGBB）
      if (rxIndex == 14) {
        rxBuffer[rxIndex] = '\0'; // 結束字串
        processColorCommand();
        rxIndex = 0;
      }
    }
    
    // ----------------------------------------
    // 檢測二進位封包（EEPROM 寫入）
    // ----------------------------------------
    if ((byte)inChar == SOF) {
      binaryIndex = 0;
      binaryBuffer[binaryIndex++] = (byte)inChar;
    } else if (binaryIndex > 0) {
      binaryBuffer[binaryIndex++] = (byte)inChar;
      
      // 檢查是否接收完整封包
      if ((byte)inChar == EOF_BYTE) {
        processBinaryPacket();
        binaryIndex = 0;
      }
      
      // 防止緩衝區溢位
      if (binaryIndex >= 32) {
        binaryIndex = 0;
      }
    }
  }
}

// ========================================
// 處理顏色命令（ASCII 格式）
// 格式：#COLOR:RRGGBB
// 範例：#COLOR:FF0000 (紅色)
// ========================================
void processColorCommand() {
  // 驗證命令格式
  if (strncmp(rxBuffer, "#COLOR:", 7) != 0) {
    return; // 格式錯誤
  }
  
  // 提取 RRGGBB 部分（從第 7 個字元開始）
  char hexStr[7];
  strncpy(hexStr, rxBuffer + 7, 6);
  hexStr[6] = '\0';
  
  // 解析 16 進位顏色碼
  long colorValue = strtol(hexStr, NULL, 16);
  byte r = (colorValue >> 16) & 0xFF;
  byte g = (colorValue >> 8) & 0xFF;
  byte b = colorValue & 0xFF;
  
  // 更新所有 LED 為相同顏色
  for (int i = 0; i < LED_COUNT; i++) {
    strip.setPixelColor(i, strip.Color(r, g, b));
    ledStates[i] = {r, g, b};
  }
  strip.show();
  
  // 可選：回傳 LED 狀態給 PC
  // sendLedStatus();
}

// ========================================
// 處理二進位封包（EEPROM 寫入）
// ========================================
void processBinaryPacket() {
  // 驗證封包格式
  if (binaryBuffer[0] != SOF) return;
  
  byte cmd = binaryBuffer[1];
  byte len = binaryBuffer[2];
  
  // 計算並驗證校驗碼
  byte checksum = 0;
  for (int i = 1; i < 3 + len; i++) {
    checksum += binaryBuffer[i];
  }
  checksum &= 0xFF;
  
  byte receivedChecksum = binaryBuffer[3 + len];
  if (checksum != receivedChecksum) {
    // 校驗碼錯誤
    return;
  }
  
  // 處理不同命令
  switch (cmd) {
    case CMD_EEPROM:
      handleEeprom();
      break;
  }
}

// ========================================
// 處理 EEPROM 寫入
// ========================================
void handleEeprom() {
  // 封包格式: [SOF][CMD][LEN][Data][CHK][EOF]
  byte data = binaryBuffer[3];
  
  // 寫入 EEPROM 位址 0
  EEPROM.write(0, data);
  
  // 回傳 ACK
  sendAck();
}

// ========================================
// 回傳 LED 狀態（二進位格式）
// ========================================
void sendLedStatus() {
  // 封包格式: [SOF][CMD][LEN][LED0_R][LED0_G][LED0_B]...[LED7_R][LED7_G][LED7_B][CHK][EOF]
  byte packet[28]; // SOF + CMD + LEN + 24 bytes RGB + CHK + EOF
  int index = 0;
  
  packet[index++] = SOF;
  packet[index++] = CMD_CPU_LOAD;
  packet[index++] = 24; // 8 LEDs × 3 colors
  
  // 填入所有 LED 的 RGB 值
  for (int i = 0; i < LED_COUNT; i++) {
    packet[index++] = ledStates[i].r;
    packet[index++] = ledStates[i].g;
    packet[index++] = ledStates[i].b;
  }
  
  // 計算校驗碼
  byte checksum = 0;
  for (int i = 1; i < index; i++) {
    checksum += packet[i];
  }
  checksum &= 0xFF;
  packet[index++] = checksum;
  packet[index++] = EOF_BYTE;
  
  // 傳送封包
  Serial.write(packet, index);
}

// ========================================
// 發送 ACK 回應
// ========================================
void sendAck() {
  // 簡單的 ACK 封包
  byte ack[] = {SOF, 0xFF, 0x00, 0xFF, EOF_BYTE};
  Serial.write(ack, 5);
}

// ========================================
// 關閉所有 LED
// ========================================
void turnOffAllLeds() {
  for (int i = 0; i < LED_COUNT; i++) {
    strip.setPixelColor(i, strip.Color(0, 0, 0));
    ledStates[i] = {0, 0, 0};
  }
  strip.show();
}

/*
 * ========================================
 * 通訊格式說明
 * ========================================
 * 
 * 1. CPU Loading 顏色控制（ASCII 字串）
 *    格式：#COLOR:RRGGBB
 *    範例：
 *      #COLOR:00FF00  → 綠色 (0~50% CPU)
 *      #COLOR:FFFF00  → 黃色 (51~84% CPU)
 *      #COLOR:FF0000  → 紅色 (?85% CPU)
 *      #COLOR:000000  → 關閉
 * 
 * 2. EEPROM 寫入（二進位格式）
 *    格式：[SOF][CMD][LEN][Data][CHK][EOF]
 *    範例：AA 20 01 0A CHK 55
 * 
 * 3. 控制字元
 *    'c' → 連線確認
 *    'b' → 斷線，關閉所有 LED
 * 
 * ========================================
 * LED 顏色對照
 * ========================================
 * - 綠色 (0~50% CPU):   #COLOR:00FF00
 * - 黃色 (51~84% CPU):  #COLOR:FFFF00
 * - 紅色 (?85% CPU):    #COLOR:FF0000
 * - 關閉:               #COLOR:000000
 * 
 * ========================================
 * 序列埠設定
 * ========================================
 * - Baud Rate: 9600
 * - Data Bits: 8
 * - Parity: None
 * - Stop Bits: 1
 * 
 * ========================================
 * 接線建議
 * ========================================
 * - WS2812 Data Pin → Arduino Pin 6
 * - WS2812 VCC → 5V
 * - WS2812 GND → GND
 * - HC-05 TX → Arduino RX
 * - HC-05 RX → Arduino TX (透過分壓電路)
 * - HC-05 VCC → 5V
 * - HC-05 GND → GND
 */
