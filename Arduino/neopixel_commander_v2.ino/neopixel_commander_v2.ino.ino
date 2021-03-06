#include <Adafruit_NeoPixel.h>

#include <stdlib.h> // for malloc and free


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

Adafruit_NeoPixel strip1;
Adafruit_NeoPixel strip2;
Adafruit_NeoPixel strip3;
Adafruit_NeoPixel strip4;
Adafruit_NeoPixel* strips[NUM_STRIPS];

void setup() {
  Serial.begin(9600);
  Serial.println(F("NeoPixel Commander v2"));
  strip1 = Adafruit_NeoPixel(30, 14, NEO_GRB + NEO_KHZ800);
  strip2 = Adafruit_NeoPixel(30, 15, NEO_GRB + NEO_KHZ800);
  strip3 = Adafruit_NeoPixel(20, 16, NEO_GRB + NEO_KHZ800);
  strip4 = Adafruit_NeoPixel(20, 17, NEO_GRB + NEO_KHZ800);
  strip1.begin();
  strip2.begin();
  strip3.begin();
  strip4.begin();
  strip1.show();
  strip2.show();
  strip3.show();
  strip4.show();
  strips[0] = &strip1;
  strips[1] = &strip2;
  strips[2] = &strip3;
  strips[3] = &strip4;
}

byte buffer[64]; 
int currentLoggingLevel = LOG_VERBOSE;

void loop() {
  int n = RawHID.recv(buffer, 0); // 0 timeout = do not wait
  if (n > 0) {
    Process();
  }
}

