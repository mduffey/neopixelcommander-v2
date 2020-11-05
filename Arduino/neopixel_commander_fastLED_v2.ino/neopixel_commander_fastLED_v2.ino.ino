#include <stdlib.h> // for malloc and free

#include <OctoWS2811.h>
#define NUM_STRIPS 4
#define NUM_LEDS_PER_STRIP 61

DMAMEM int displayMemory[NUM_LEDS_PER_STRIP * NUM_STRIPS * 3 / 4];
int drawingMemory[NUM_LEDS_PER_STRIP * NUM_STRIPS * 3 / 4];

const int config = WS2811_GRB | WS2811_800kHz;

#if defined(__MK20DX256__)
  OctoWS2811 leds(NUM_LEDS_PER_STRIP, displayMemory, drawingMemory, config);
#else
  byte pinList[NUM_STRIPS] = {9, 10, 11, 12};
  OctoWS2811 leds(NUM_LEDS_PER_STRIP, displayMemory, drawingMemory, config, NUM_STRIPS, pinList);
#endif



void setup() {
  leds.begin();
  leds.show();
}

void loop() {
  if (usb_serial_available() >= 4) {
     int pos = usb_serial_getchar();
     int red = usb_serial_getchar();
     int green = usb_serial_getchar();
     int blue = usb_serial_getchar();
     ParsePositionAndUpdateLED(pos, red, green, blue);
  }
}

void ParsePositionAndUpdateLED(int pos, int red, int green, int blue) {
  int strip = pos & 3;
  int led = pos >> 2;
  UpdateLED(strip, led, red, green, blue);
}

void UpdateLED(int strip, int led, int red, int green, int blue) {
  leds.setPixel(led + strip * NUM_LEDS_PER_STRIP, red, green, blue);
}
