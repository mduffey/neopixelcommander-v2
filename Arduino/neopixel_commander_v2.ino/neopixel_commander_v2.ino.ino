#include <stdlib.h> // for malloc and free

#include <FastLED.h>
#define NUM_STRIPS 4
#define NUM_LEDS_PER_STRIP 43
CRGB leds[NUM_STRIPS][NUM_LEDS_PER_STRIP];

void setup() {
  Serial.begin(9600);
  Serial.println(F("NeoPixel Commander v2"));
  FastLED.addLeds<NEOPIXEL, 16>(leds[0], NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 17>(leds[1], NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 14>(leds[2], NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 15>(leds[3], NUM_LEDS_PER_STRIP);
}

byte buffer[64];

void loop() {
  int n = RawHID.recv(buffer, 0); // 0 timeout = do not wait
  if (n > 0) {
    Process();
  }
}

