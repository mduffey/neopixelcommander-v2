void UpdateLEDs(int red, int green, int blue) {
    for(int strip = 0; strip < NUM_STRIPS; strip++) {
      UpdateLEDsForStrip(strip, red, green, blue);
    }
}

void UpdateLEDsForStrip(int strip, int red, int green, int blue) {
  for (int i = 0; i < NUM_LEDS_PER_STRIP; i++) {
    UpdateLED(strip, i, red, green, blue);
  }
}

void ParsePositionAndUpdateLED(int pos, int red, int green, int blue) {
  int strip = pos & 3;
  int led = pos >> 2;
  UpdateLED(strip, led, red, green, blue);
}

void UpdateLED(int strip, int led, int red, int green, int blue) {
  leds[strip][led].setRGB(red, green, blue);
  PrintLEDUpdate(strip, led, red, green, blue);
}
