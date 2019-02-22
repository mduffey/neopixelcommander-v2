#include <stdlib.h> // for malloc and free

#include <FastLED.h>
#define NUM_STRIPS 4
#define NUM_LEDS_PER_STRIP 43
#define MESSAGE_RANGE 10  // Specific LEDs
#define MESSAGE_STRIP 20 // All LEDs on one strip
#define MESSAGE_UNIVERSAL 30 // All strips, all LEDs
#define MESSAGE_GRADIENT 40 // Specifies two colors, and the gradient will be managed by the Arduino
#define MESSAGE_STATUS 100  // Queries current settings, like whether the device is on
#define MESSAGE_SETTINGS 110 // Updates settings. For now, alters log frequency.
#define LOG_VERBOSE 10
#define LOG_ACTIONS 20
#define LOG_UNEXPECTED 30
#define LOG_NONE 40

#define STATUS_AVAILABLE 200
#define STATUS_DISABLED 100

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
int currentLoggingLevel = LOG_VERBOSE;

void loop() {
  int n = RawHID.recv(buffer, 0); // 0 timeout = do not wait
  if (n > 0) {
    Process();
  }
}

