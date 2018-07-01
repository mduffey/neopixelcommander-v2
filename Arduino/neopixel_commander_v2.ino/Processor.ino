#define RANGE 10  // Specific LEDs
#define STRIP 20 // All LEDs on one strip
#define UNIVERSAL 30 // All strips, all LEDs
#define GRADIENT 40 // Specifies two colors, and the gradient will be managed by the Arduino


void Process() {
  // Various control codes are checked on the first byte.
  // Maximum possible value for last index, highest strip is around 172, so 200, 210, etc. are control codes.
  if (buffer[0] == RANGE) {
    Serial.println(F("Range"));
    for (int i = 1; i < 61; i += 4) {
      if (buffer[i] == 255) {
        break;
      }
      UpdateLED(buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3]);
    }
  }
  else if (buffer[0] == STRIP) {
    Serial.println(F("Strip"));
    int strip = buffer[1];
    UpdateLEDsForStrip(strip, buffer[2], buffer[3], buffer[4]);
  }
  else if (buffer[0] == UNIVERSAL) {
    Serial.println(F("Universal"));
    UpdateLEDs(buffer[1], buffer[2], buffer[3]);
  }
  else if (buffer[0] == GRADIENT) {
    Serial.println(F("Gradient"));
    UpdateLEDs(0, 0, 0);
  } 
  else {
    Serial.print(F("Unsupported: "));
    Serial.println(buffer[0]);
  }
  FastLED.show();
  
}

void UpdateLEDs(int red, int green, int blue) {
    for(int strip = 0; strip < NUM_STRIPS; strip++) {
      Serial.print(F("Updating strip: "));
      Serial.println(strip);
      UpdateLEDsForStrip(strip, red, green, blue);
    }
}

void UpdateLEDsForStrip(int strip, int red, int green, int blue) {
  for (int i = 0; i < NUM_LEDS_PER_STRIP; i++) {
    Serial.print(F("Updating LED: "));
    Serial.print(i);
    Serial.print(F(". Colors: "));
    Serial.print(red);
    Serial.print(", ");
    Serial.print(green);
    Serial.print(", ");
    Serial.println(blue);
    leds[strip][i].setRGB(red, green, blue);
  }
}

void UpdateLED(int pos, int red, int green, int blue) {
  int strip = pos & 3;
  int led = pos >> 2;
  leds[strip][led].setRGB(red, green, blue);
}
