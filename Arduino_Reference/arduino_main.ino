/*
 * Arduino 端參考程式
 * 113 學年度工科賽 電腦修護職類 第二站
 * 崗位號碼：01
 * 
 * 功能：
 * - 接收 PC 端 CPU Loading 資料
 * - 控制 WS2812 LED 顯示對應顏色
 * - EEPROM 資料寫入
 * - 回傳 LED 狀態給 PC
 */

#include <Adafruit_NeoPixel.h>
#include <EEPROM.h>

// WS2812 設定
#define LED_PIN    6
#define LED_COUNT  8
Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

// 通訊協定常數
const byte SOF = 0xAA;
const byte EOF_BYTE = 0x55;
const byte CMD_CPU_LOAD = 0x10;
const byte CMD_EEPROM = 0x20;

// 接收緩衝區
const int BUFFER_SIZE = 32;
byte rxBuffer[BUFFER_SIZE];
int rxIndex = 0;

// LED 狀態
struct LedState {
  byte r, g, b;
};
LedState ledStates[LED_COUNT];

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

void loop() {
  // 檢查序列埠資料
  while (Serial.available() > 0) {
    byte inByte = Serial.read();
    
    // 處理特殊控制字元
    if (inByte == 'c') {
      // 連線字元，可以回應 ACK
      sendAck();
      continue;
    } else if (inByte == 'b') {
      // 斷線字元，關閉所有 LED
      turnOffAllLeds();
      continue;
    }
    
    // 接收封包
    if (inByte == SOF) {
      rxIndex = 0;
      rxBuffer[rxIndex++] = inByte;
    } else if (rxIndex > 0) {
      rxBuffer[rxIndex++] = inByte;
      
      // 檢查是否接收完整封包
      if (inByte == EOF_BYTE) {
        processPacket();
        rxIndex = 0;
      }
      
      // 防止緩衝區溢位
      if (rxIndex >= BUFFER_SIZE) {
        rxIndex = 0;
      }
    }
  }
}

void processPacket() {
  // 驗證封包格式
  if (rxBuffer[0] != SOF) return;
  
  byte cmd = rxBuffer[1];
  byte len = rxBuffer[2];
  
  // 計算並驗證校驗碼
  byte checksum = 0;
  for (int i = 1; i < 3 + len; i++) {
    checksum += rxBuffer[i];
  }
  checksum &= 0xFF;
  
  byte receivedChecksum = rxBuffer[3 + len];
  if (checksum != receivedChecksum) {
    // 校驗碼錯誤
    return;
  }
  
  // 處理不同命令
  switch (cmd) {
    case CMD_CPU_LOAD:
      handleCpuLoad();
      break;
    case CMD_EEPROM:
      handleEeprom();
      break;
  }
}

void handleCpuLoad() {
  // 封包格式: [SOF][CMD][LEN][CPU][R][G][B][CHK][EOF]
  byte cpuUsage = rxBuffer[3];
  byte r = rxBuffer[4];
  byte g = rxBuffer[5];
  byte b = rxBuffer[6];
  
  // 更新所有 LED 為相同顏色
  for (int i = 0; i < LED_COUNT; i++) {
    strip.setPixelColor(i, strip.Color(r, g, b));
    ledStates[i] = {r, g, b};
  }
  strip.show();
  
  // 回傳 LED 狀態給 PC
  sendLedStatus();
}

void handleEeprom() {
  // 封包格式: [SOF][CMD][LEN][Data][CHK][EOF]
  byte data = rxBuffer[3];
  
  // 寫入 EEPROM 位址 0
  EEPROM.write(0, data);
  
  // 可選：回傳 ACK
  sendAck();
}

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

void sendAck() {
  // 簡單的 ACK 封包
  byte ack[] = {SOF, 0xFF, 0x00, 0xFF, EOF_BYTE};
  Serial.write(ack, 5);
}

void turnOffAllLeds() {
  for (int i = 0; i < LED_COUNT; i++) {
    strip.setPixelColor(i, strip.Color(0, 0, 0));
    ledStates[i] = {0, 0, 0};
  }
  strip.show();
}

/*
 * LED 顏色對照：
 * - 綠色 (0~50% CPU): RGB(0, 255, 0)
 * - 黃色 (51~84% CPU): RGB(255, 255, 0)
 * - 紅色 (?85% CPU): RGB(255, 0, 0)
 * - 關閉: RGB(0, 0, 0)
 * 
 * 序列埠設定：
 * - Baud Rate: 9600
 * - Data Bits: 8
 * - Parity: None
 * - Stop Bits: 1
 * 
 * 接線建議：
 * - WS2812 Data Pin → Arduino Pin 6
 * - WS2812 VCC → 5V
 * - WS2812 GND → GND
 * - HC-05 TX → Arduino RX
 * - HC-05 RX → Arduino TX (透過分壓電路)
 * - HC-05 VCC → 5V
 * - HC-05 GND → GND
 */
