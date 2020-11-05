#include <stdlib.h> // for malloc and free

#include <FastLED.h>
#define NUM_STRIPS 4
#define NUM_LEDS_PER_STRIP 61
#define CORRECTION 0xFFFFFF

CRGB leds[NUM_STRIPS][NUM_LEDS_PER_STRIP];

void setup() {
  FastLED.addLeds<WS2812, 2, GRB>(leds[0], NUM_LEDS_PER_STRIP);
  FastLED.addLeds<WS2812, 14, GRB>(leds[1], NUM_LEDS_PER_STRIP);
  FastLED.addLeds<WS2812, 7, GRB>(leds[2], NUM_LEDS_PER_STRIP);
  FastLED.addLeds<WS2812, 8, GRB>(leds[3], NUM_LEDS_PER_STRIP);
}

int ledIndex = 0;
int ledValues[8][3] = {
  { 128, 0, 0 },
  { 0, 128, 0 },
  { 0, 0, 128 },
  { 128, 0, 128 },
  { 128, 83, 0 },
  { 0, 128, 115 },
  { 128, 128, 128 },
  { 0, 0, 0 }
};

void loop() {
  UpdateLEDs(ledValues[ledIndex][0], ledValues[ledIndex][1], ledValues[ledIndex][2]);
  FastLED.show();
  ledIndex++;
  if (ledIndex == 8) {
    ledIndex = 0;
  }
  delay(1200);
}
