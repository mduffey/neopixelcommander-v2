#include <OctoWS2811.h>
#define NUM_STRIPS 4
#define NUM_LEDS_PER_STRIP 61
#define MESSAGE_CHECK 10
#define MESSAGE_SINGLE 20

DMAMEM int displayMemory[NUM_LEDS_PER_STRIP * 6];
int drawingMemory[NUM_LEDS_PER_STRIP * 6];

const int config = WS2811_GRB | WS2811_800kHz;

#if defined(__MK20DX256__)
  OctoWS2811 leds(NUM_LEDS_PER_STRIP, displayMemory, drawingMemory, config);
#else
  byte pinList[NUM_STRIPS] = {9, 10, 11, 12};
  OctoWS2811 leds(NUM_LEDS_PER_STRIP, displayMemory, drawingMemory, config, NUM_STRIPS, pinList);
#endif



void setup() {
  leds.begin();
  Serial.begin(9600);
}

byte buffer[1280];

void loop() {
  int available = Serial.available();
  if (available >= 5) {
    int count = available;
    if (available > 1280) {
      count = 1280;
    }
    Serial.readBytes(buffer, count);
    int index = 0;
    while (index + 4 < count) {
      int message = buffer[index];
      
      if (message == MESSAGE_CHECK) {
        Serial.write("YES\n");
      } else if (message == MESSAGE_SINGLE) {
        ParsePositionAndUpdateLED(buffer[index + 1], buffer[index + 2], buffer[index + 3], buffer[index + 4]);
      }
      index += 5;
    }
  }
  leds.show();
}

void ParsePositionAndUpdateLED(int pos, int red, int green, int blue) {
  int strip = pos & 3;
  int led = pos >> 2;
  UpdateLED(strip, led, red, green, blue);
}

void UpdateLED(int strip, int led, int red, int green, int blue) {
  leds.setPixel(led + strip * NUM_LEDS_PER_STRIP, red, green, blue);
}
